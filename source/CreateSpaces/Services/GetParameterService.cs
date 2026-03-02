using Autodesk.Revit.DB.Architecture;
using CreateSpaces.Models;

namespace CreateSpaces.Services;

public class GetParameterService
{
    private readonly Document _doc;

    public GetParameterService(Document doc)
    {
        _doc = doc;
    }

    public IReadOnlyList<ParameterDescriptor> GetFromRoom(Room? room)
    {
        if (room == null)
            throw new ArgumentNullException(nameof(room));

        return CollectFromElement(room);
    }

    public IReadOnlyList<ParameterDescriptor> GetFromTemporarySpace()
    {
        var level = new FilteredElementCollector(_doc)
            .OfClass(typeof(Level))
            .Cast<Level>()
            .FirstOrDefault();

        if (level == null)
            return Array.Empty<ParameterDescriptor>();

        using var t = new Transaction(_doc, "Temp Space");
        t.Start();

        // Координата любая валидная
        var point = new UV(0, 0);

        var space = _doc.Create.NewSpace(level, point);
        
        var result = CollectFromElement(space);

        t.RollBack();

        return result;
    }


    private IReadOnlyList<ParameterDescriptor> CollectFromElement(Element? element)
    {
        var dict = new Dictionary<string, ParameterDescriptor>();

        foreach (Parameter param in element?.Parameters!)
        {
            if (param.Definition == null)
                continue;

            if (param.StorageType == StorageType.None)
                continue;

            var name = param.Definition.Name;

            if (dict.ContainsKey(name))
                continue;

            var builtIn = param.Definition is InternalDefinition internalDef
                ? internalDef.BuiltInParameter
                : (BuiltInParameter?)null;

            dict[name] = new ParameterDescriptor
            {
                Name = name,
                StorageType = param.StorageType,
                IsShared = param.IsShared,
                IsReadOnly = param.IsReadOnly,
                IsInstance = true,
                BuiltInParameter = builtIn
            };
        }

        return dict.Values
            .OrderBy(x => x.Name)
            .ToList();
    }
}