using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using SSPlan.ViewModels;

namespace SSPlan.Models;

    public class AnnotationPlacementModel
    {
        public Result PlaceAnnotations(
            Document doc,
            View view,
            PanelItem panelItem,
            FamilyItem familyItem,
            int offsetXmm,
            int offsetYmm)
        {
            try
            {
                // Получаем элементы из моделей
                Element panel = doc.GetElement(panelItem.Id);
                FamilySymbol cameraFamilySymbol = familyItem.Symbol;

                // Получаем подключенные цепи к панели
                List<ElectricalSystem> connectedCircuits = GetConnectedCircuits(doc, panel);
                
                if (!connectedCircuits.Any())
                    throw new InvalidOperationException("Нет подключённых цепей к выбранной панели");

                // Активируем семейство
                ActivateFamilySymbol(doc, cameraFamilySymbol);

                // Размещаем семейства на виде
                PlaceFamilyInstances(doc, view, cameraFamilySymbol, connectedCircuits, offsetXmm, offsetYmm);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при размещении аннотаций: {ex.Message}", ex);
            }
        }

        private List<ElectricalSystem> GetConnectedCircuits(Document doc, Element panel)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(ElectricalSystem))
                .Cast<ElectricalSystem>()
                .Where(esys => esys.BaseEquipment != null && esys.BaseEquipment.Id == panel.Id)
                .ToList();
        }

        private void ActivateFamilySymbol(Document doc, FamilySymbol familySymbol)
        {
            using (Transaction t = new Transaction(doc, "Активировать семейство"))
            {
                t.Start();
                if (!familySymbol.IsActive)
                {
                    familySymbol.Activate();
                }
                t.Commit();
            }
        }

        private void PlaceFamilyInstances(
            Document doc,
            View view,
            FamilySymbol familySymbol,
            List<ElectricalSystem> circuits,
            int offsetXmm,
            int offsetYmm)
        {
            using (Transaction t = new Transaction(doc, "Разместить семейства на чертежном виде"))
            {
                t.Start();

                double offsetX = offsetXmm / 304.8; // Конвертация мм в футы
                double offsetY = offsetYmm / 304.8;
                double currentX = offsetX;
                double currentY = offsetY;

                foreach (var circuit in circuits)
                {
                    Element? connectedElement = GetConnectedElement(circuit);
                    
                    if (connectedElement == null)
                        continue;

                    // Размещение семейства
                    XYZ placementPoint = new XYZ(currentX, currentY, 0);
                    FamilyInstance instance = doc.Create.NewFamilyInstance(
                        placementPoint,
                        familySymbol,
                        view
                    );

                    // Установка параметров
                    SetInstanceParameters(doc, instance, circuit, connectedElement);

                    // Смещение для следующего экземпляра
                    currentX += offsetX;
                    currentY += offsetY;
                }

                t.Commit();
            }
        }

        private Element? GetConnectedElement(ElectricalSystem circuit)
        {
            if (circuit.Elements != null && circuit.Elements.Size > 0)
            {
                return circuit.Elements.Cast<Element>().FirstOrDefault();
            }
            return null;
        }

        private void SetInstanceParameters(Document doc, FamilyInstance instance, ElectricalSystem circuit, Element? connectedElement)
        {
            string circuitNumber = circuit.Name ?? "N/A";
            double? wireLength = circuit.LookupParameter("TSL_Длина проводника")?.AsDouble();

            // Получаем параметры из подключенного элемента
            string spaceNumber = GetParamAsString(doc, connectedElement, "Номер пространства");
            string floorNumber = GetParamAsString(doc, connectedElement, "ADSK_Этаж");
            string buildNumber = GetParamAsString(doc, connectedElement, "ADSK_Номер здания");
            string panelName = GetParamAsString(doc, connectedElement, "Имя панели");
            string visibility3 = GetParamAsString(doc, connectedElement, "Видимость_3");

            // Параметры из типа элемента
            ElementType? type = doc.GetElement(connectedElement?.GetTypeId()) as ElementType;
            string visibility1 = GetVisibility1Parameter(type);
            string visibility2 = GetVisibility2Parameter(type);
            string textField = type?.LookupParameter("Текстовое поле")?.AsString() ?? "N/A";

            // Устанавливаем параметры
            SetParameter(instance, "Номер цепи", circuitNumber);
            SetParameter(instance, "Номер пространства", spaceNumber);
            SetParameter(instance, "Видимость_1", visibility1);
            SetParameter(instance, "Видимость_2", visibility2);
            SetParameter(instance, "Видимость_3", visibility3);
            SetParameter(instance, "Текстовое поле", textField);
            SetParameter(instance, "ADSK_Этаж", floorNumber);
            SetParameter(instance, "ADSK_Номер здания", buildNumber);
            SetParameter(instance, "Имя панели", panelName);

            // Устанавливаем параметр длины проводника
            SetWireLengthParameter(instance, wireLength);
        }

        private string GetVisibility1Parameter(ElementType? type)
        {
            Parameter? vis1Param = type?.LookupParameter("Видимость_1");
            if (vis1Param != null && vis1Param.StorageType == StorageType.Integer)
                return vis1Param.AsInteger().ToString();
            return "N/A";
        }

        private string GetVisibility2Parameter(ElementType? type)
        {
            Parameter? vis2Param = type?.LookupParameter("Видимость_2");
            if (vis2Param != null && vis2Param.StorageType == StorageType.Integer)
                return vis2Param.AsInteger() == 1 ? "Да" : "Нет";
            return "N/A";
        }

        private void SetParameter(FamilyInstance instance, string paramName, string value)
        {
            Parameter p = instance.LookupParameter(paramName);
            if (p != null && !p.IsReadOnly)
            {
                if (p.StorageType == StorageType.String)
                {
                    p.Set(value);
                }
                else if (p.StorageType == StorageType.Integer)
                {
                    if (value == "Да") p.Set(1);
                    else if (value == "Нет") p.Set(0);
                    else if (int.TryParse(value, out int intVal))
                        p.Set(intVal);
                }
            }
        }

        private void SetWireLengthParameter(FamilyInstance instance, double? wireLength)
        {
            if (!wireLength.HasValue) return;

            Parameter wireLengthParam = instance.LookupParameter("TSL_Длина проводника");
            if (wireLengthParam != null && !wireLengthParam.IsReadOnly &&
                wireLengthParam.StorageType == StorageType.Double)
            {
                wireLengthParam.Set(wireLength.Value);
            }
        }

        private string GetParamAsString(Document doc, Element? elem, string paramName)
        {
            if (elem == null) return "N/A";

            Parameter? param = elem.LookupParameter(paramName);

            if (param == null || !param.HasValue)
            {
                ElementType? type = doc.GetElement(elem.GetTypeId()) as ElementType;
                param = type?.LookupParameter(paramName);
            }

            if (param == null || !param.HasValue)
                return "N/A";

            switch (param.StorageType)
            {
                case StorageType.String:
                    return param.AsString() ?? "N/A";

                case StorageType.Integer:
#if REVIT2022_OR_GREATER
                    var dataType = param.Definition.GetDataType();
                    if (dataType == SpecTypeId.Boolean.YesNo)
                    {
                        return param.AsInteger() == 1 ? "Да" : "Нет";
                    }
                    else
                    {
                        return param.AsInteger().ToString();
                    }
#else
        return param.Definition.ParameterType == ParameterType.YesNo
            ? (param.AsInteger() == 1 ? "Да" : "Нет")
            : param.AsInteger().ToString();
#endif

                case StorageType.Double:
                    return param.AsValueString() ?? param.AsDouble().ToString("F2");

                case StorageType.ElementId:
                    ElementId id = param.AsElementId();
                    if (id.IntegerValue < 0) return "—";
                    Element referenced = doc.GetElement(id);
                    return referenced?.Name ?? id.IntegerValue.ToString();

                default:
                    return "N/A";
            }

        }
    }