using Autodesk.Revit.DB;

namespace ChangeSharedFamilies.Services;

internal class InstanceParameterBindingService
{
    private readonly Document _familyDocument;

    public InstanceParameterBindingService(Document familyDocument)
    {
        _familyDocument = familyDocument;
    }

    public List<NestedInstanceBindingSnapshot> Capture(
        List<FamilyInstance> nestedInstances)
    {
        var result = new List<NestedInstanceBindingSnapshot>();
        var familyManager = _familyDocument.FamilyManager;

        foreach (var instance in nestedInstances)
        {
            var snapshot = new NestedInstanceBindingSnapshot
            {
                InstanceId = instance.Id,
                OldSymbolName = instance.Symbol.Name
            };

            foreach (Parameter elementParameter in instance.Parameters)
            {
                if (elementParameter?.Definition == null)
                    continue;

                FamilyParameter associatedFamilyParameter;

                try
                {
                    associatedFamilyParameter =
                        familyManager.GetAssociatedFamilyParameter(elementParameter);
                }
                catch
                {
                    continue;
                }

                if (associatedFamilyParameter == null)
                    continue;

                snapshot.Bindings.Add(new ParameterBindingSnapshot
                {
                    ElementParameterId = elementParameter.Id,
                    ElementParameterName = elementParameter.Definition.Name,
                    ElementParameterStorageType = elementParameter.StorageType,
                    FamilyParameter = associatedFamilyParameter
                });
            }

            result.Add(snapshot);
        }

        return result;
    }

    public bool IsControlledByFamilyTypeLabel(
        NestedInstanceBindingSnapshot instanceSnapshot,
        List<FamilyTypeValueSnapshot> familyTypeValueSnapshots)
    {
        if (instanceSnapshot.Bindings.Count == 0)
            return false;

        if (familyTypeValueSnapshots.Count == 0)
            return false;

        foreach (var binding in instanceSnapshot.Bindings)
        {
            foreach (var valueSnapshot in familyTypeValueSnapshots)
            {
                if (FamilyParameterIdentity.IsSame(
                        binding.FamilyParameter,
                        valueSnapshot.FamilyParameter))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void Restore(List<NestedInstanceBindingSnapshot> snapshots)
    {
        var familyManager = _familyDocument.FamilyManager;

        foreach (var snapshot in snapshots)
        {
            if (snapshot.Bindings.Count == 0)
                continue;

            var instance = _familyDocument.GetElement(snapshot.InstanceId) as FamilyInstance;

            if (instance == null)
                continue;

            foreach (var binding in snapshot.Bindings)
            {
                var elementParameter = FindElementParameterByBindingSnapshot(
                    instance,
                    binding);

                if (elementParameter == null)
                    continue;

                FamilyParameter currentAssociatedParameter = null;

                try
                {
                    currentAssociatedParameter =
                        familyManager.GetAssociatedFamilyParameter(elementParameter);
                }
                catch
                {
                    // Если Revit не отдал текущую привязку, ниже попробуем привязать заново.
                }

                if (FamilyParameterIdentity.IsSame(
                        currentAssociatedParameter,
                        binding.FamilyParameter))
                {
                    continue;
                }

                try
                {
                    familyManager.AssociateElementParameterToFamilyParameter(
                        elementParameter,
                        binding.FamilyParameter);
                }
                catch (Exception exception)
                {
                    throw new InvalidOperationException(
                        $"Не удалось восстановить метку параметра '{binding.ElementParameterName}' " +
                        $"для вложенного экземпляра '{snapshot.OldSymbolName}'. " +
                        $"Причина: {exception.Message}",
                        exception);
                }
            }
        }
    }

    private static Parameter FindElementParameterByBindingSnapshot(
        Element element,
        ParameterBindingSnapshot snapshot)
    {
        if (snapshot.ElementParameterId != null &&
            snapshot.ElementParameterId != ElementId.InvalidElementId &&
            snapshot.ElementParameterId.IntegerValue < 0)
        {
            var builtInParameter =
                (BuiltInParameter)snapshot.ElementParameterId.IntegerValue;

            var parameter = element.get_Parameter(builtInParameter);

            if (parameter != null)
                return parameter;
        }

        foreach (Parameter parameter in element.Parameters)
        {
            if (parameter?.Definition == null)
                continue;

            if (!string.Equals(
                    parameter.Definition.Name,
                    snapshot.ElementParameterName,
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (parameter.StorageType != snapshot.ElementParameterStorageType)
                continue;

            return parameter;
        }

        return null;
    }
}
