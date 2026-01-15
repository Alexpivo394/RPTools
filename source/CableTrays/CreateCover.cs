using System.Diagnostics;
using System.IO;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using RPToolsUI.Models;
using RPToolsUI.Services;

namespace CreateCover;

[Regeneration(RegenerationOption.Manual)]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : IExternalCommand
{
    private const string CoverFamilyName = "RP_Крышка"; // Family name

    private const string CoverFamilyPath =
        @"Y:\12-BIM\Стандарт\Общие семейства и мануалы ко всем шаблонам\Семейства ЭОМ, СС\Соединители лотка\RP_Крышка.rfa"; // Srver path

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "CreateCoverLog.txt");

        try
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var app = uiapp.Application;
            var doc = uidoc.Document;

            var coverParamName = "ADSK_Крышка";
            var articulParamName = "ADSK_Исполнение";
            
            var selection = uidoc.Selection
                .GetElementIds()
                .Select(id => doc.GetElement(id))
                .OfType<CableTray>()
                .ToList();


            List<CableTray> trays = new List<CableTray>();


            if (selection.Count == 0)
            {
                trays = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_CableTray)
                    .WhereElementIsNotElementType()
                    .Cast<CableTray>()
                    .ToList();
            }
            else
            {
                trays = selection.ToList();
            }
            
            
            var coverSymbol = GetOrLoadFamilySymbol(doc, CoverFamilyName, CoverFamilyPath);

            if (coverSymbol == null)
            {
                var errMsg = $"Не удалось найти или загрузить семейство {CoverFamilyName}";
                var dial1 = ToadDialogService.Show(
                    "ЧОТА НЕ ТАК(((",
                    "С загрузкой семейства какая-то шляпа",
                    DialogButtons.OK,
                    DialogIcon.Error
                );
                return Result.Failed;
            }
            
            var collector = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance))
                .WhereElementIsNotElementType();

            using (var tx = new Transaction(doc, "Удалить крышки"))
            {
                tx.Start();

                var idsToDelete = new List<ElementId>();

                foreach (var elem in collector)
                {
                    if (elem is FamilyInstance fi)
                    {
                        var family = fi.Symbol.Family;
                        if (family.Name == CoverFamilyName)
                        {
                            idsToDelete.Add(fi.Id);
                        }
                    }
                }
                
                if (idsToDelete.Count > 0)
                {
                    doc.Delete(idsToDelete);
                }

                tx.Commit();
            }
            
            using (var trGr = new TransactionGroup(doc, "Добавление крышек"))
            {
                trGr.Start();

                if (!coverSymbol.IsActive)
                {
                    coverSymbol.Activate();
                }


                
                foreach (var tray in trays)
                {
                    try
                    {
                        var coverParam = tray.LookupParameter(coverParamName);
                        if (coverParam == null || coverParam.AsInteger() != 1) continue;
                        

                        var width = tray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM).AsDouble();
                        var height = tray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_HEIGHT_PARAM).AsDouble();

                        if (tray.Location is not LocationCurve loc)
                        {
                            continue;
                        }

                        var length = loc.Curve.Length;

                        var start = loc.Curve.GetEndPoint(0);
                        var end = loc.Curve.GetEndPoint(1);
                        var midpoint = (start + end) / 2;

                        FamilyInstance instance = doc.RunInTransaction("Создание крышек",
                            () => doc.Create.NewFamilyInstance(midpoint, coverSymbol, StructuralType.NonStructural));

                        var line = loc?.Curve as Line;

                        var trayDirection = line.Direction.Normalize();

                        RotateCupOnTrayDirection(doc, tray, instance, trayDirection);
                        RotateCupByTrayNormal(doc, tray, instance, trayDirection);

                        doc.RunInTransaction("Установка размера крышек", () =>
                        {
                            SetParam(instance, "ADSK_Размер_Ширина", width);
                            SetParam(instance, "ADSK_Размер_Высота", height);
                            SetParam(instance, "ADSK_Размер_Длина", length);
                        });

                        var ispol = tray.LookupParameter(articulParamName).AsInteger();
                        
                        var widthStr = tray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM).AsValueString();
                        var heightStr = tray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_HEIGHT_PARAM).AsValueString();

                        var artikul = "";
                        var shirina50 = "50";
                        var shirina80 = "80";
                        var shirina100 = "100";
                        var shirina150 = "150";
                        var shirina200 = "200";
                        var shirina300 = "300";
                        var shirina400 = "400";
                        var shirina500 = "500";
                        var shirina600 = "600";
                        var name = "имя не найдено";

                        if (ispol == 1)
                        {
                            if (widthStr == shirina50) artikul = "35520";
                            if (widthStr == shirina80) artikul = "35521";
                            if (widthStr == shirina100) artikul = "35522";
                            if (widthStr == shirina150) artikul = "35523";
                            if (widthStr == shirina200) artikul = "35524";
                            if (widthStr == shirina300) artikul = "35525";
                            if (widthStr == shirina400) artikul = "35526";
                            if (widthStr == shirina500) artikul = "35527";
                            if (widthStr == shirina600) artikul = "35528";
                        }
                        else if (ispol == 2)
                        {
                            if (widthStr == shirina50) artikul = "35520HDZ";
                            if (widthStr == shirina80) artikul = "35521HDZ";
                            if (widthStr == shirina100) artikul = "35522HDZ";
                            if (widthStr == shirina150) artikul = "35523HDZ";
                            if (widthStr == shirina200) artikul = "35524HDZ";
                            if (widthStr == shirina300) artikul = "35525HDZ";
                            if (widthStr == shirina400) artikul = "35526HDZ";
                            if (widthStr == shirina500) artikul = "35527HDZ";
                            if (widthStr == shirina600) artikul = "35528HDZ";
                        }
                        else if (ispol == 3)
                        {
                            if (widthStr == shirina50) artikul = "35520ZL";
                            if (widthStr == shirina80) artikul = "35521ZL";
                            if (widthStr == shirina100) artikul = "35522ZL";
                            if (widthStr == shirina150) artikul = "35523ZL";
                            if (widthStr == shirina200) artikul = "35524ZL";
                            if (widthStr == shirina300) artikul = "35525ZL";
                            if (widthStr == shirina400) artikul = "35526ZL";
                            if (widthStr == shirina500) artikul = "35527ZL";
                            if (widthStr == shirina600) artikul = "35528ZL";
                        }
                        else
                        {
                            artikul = "Исполнение не найдено";
                        }

                        if (artikul != "Исполнение не найдено")
                        {
                            name = "Крышка на прямой элемент " + widthStr + "x" + heightStr;
                            if (ispol == 1) name = name + ", оцинковка горячим способом по методу Сендзимира";
                            if (ispol == 2)
                            {
                                name = name + ", горячего цинкования";
                            }

                            if (ispol == 3)
                            {
                                name = name + ", из нержавеющей стали";
                            }
                        }
                        else
                        {
                            name = "Исполнение не найдено";
                        }

                        doc.RunInTransaction("Заполнение артикула", () =>
                        {
                            instance.LookupParameter("ADSK_Код изделия").Set(artikul);
                            instance.LookupParameter("ADSK_Наименование").Set(name);
                            instance.LookupParameter(articulParamName).Set(ispol);
                        });
                        
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }

                trGr.Commit();
            }

            var dial = ToadDialogService.Show(
                "Успех!",
                "Крышки созданы.",
                DialogButtons.OK,
                DialogIcon.Info
            );
            return Result.Succeeded;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            return Result.Failed;
        }
    }

    private void SetParam(FamilyInstance instance, string paramName, double valueInternal)
    {
        var p = instance.LookupParameter(paramName);
        if (p != null && !p.IsReadOnly)
        {
            p.Set(valueInternal);
        }
    }

    private FamilySymbol GetOrLoadFamilySymbol(Document doc, string familyName, string familyPath)
    {
        try
        {
            // 1. Ищем символ с таким FamilyName
            var existingSymbol = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .FirstOrDefault(f => f.FamilyName.Equals(familyName, StringComparison.OrdinalIgnoreCase));

            if (existingSymbol != null)
                return existingSymbol;

            // 2. Проверяем путь
            if (!File.Exists(familyPath))
            {
                var dial = ToadDialogService.Show(
                    "Ошибка!",
                    $"Файл семейства не найден по пути:\n{familyPath}",
                    DialogButtons.OK,
                    DialogIcon.Error
                );
                return null;
            }

            // 3. Загружаем семейство

            Family family = null;
            
            using (var t = new Transaction(doc, "Загрузка семейства"))
            {
                t.Start();

                if (!doc.LoadFamily(familyPath, out family))
                {
                    var dial = ToadDialogService.Show(
                        "Ошибка!",
                        $"Не удалось загрузить семейство {familyPath}",
                        DialogButtons.OK,
                        DialogIcon.Error
                    );
                    return null;
                }

                t.Commit();
            }

            // 4. Получаем все типы в семействе
            var symbols = family.GetFamilySymbolIds()
                .Select(id => doc.GetElement(id) as FamilySymbol)
                .Where(s => s != null)
                .ToList();

            if (symbols.Count == 0)
            {
                var dial = ToadDialogService.Show(
                    "Ошибка!",
                    $"В семействе '{familyName}' нет типов!",
                    DialogButtons.OK,
                    DialogIcon.Error
                );
                return null;
            }

            // 5. Берём первый символ (или ищем по имени типа, если нужно)
            var loadedSymbol = symbols.First();

            // 6. Активируем, если неактивный
            if (!loadedSymbol.IsActive)
            {
                using (var t = new Transaction(doc, "Активация типа"))
                {
                    t.Start();
                    loadedSymbol.Activate();
                    t.Commit();
                }
            }

            return loadedSymbol;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return null;
        }
    }


    private void RotateCupByTrayNormal(Document doc, CableTray tray, FamilyInstance famInstance, XYZ trayDirection)
    {
        var locPoint = famInstance.Location as LocationPoint;
        var instanceDirection2 = famInstance.HandOrientation;
        var facingOrientation = famInstance.FacingOrientation;
        
        var cupNormal = instanceDirection2.CrossProduct(facingOrientation).Normalize();
        var trayNormal = tray.CurveNormal;

        var normal2 = cupNormal.CrossProduct(trayNormal).Normalize();
        if (normal2.IsAlmostEqualTo(XYZ.Zero))
        {
            normal2 = trayDirection.Normalize();
        }

        var angle2 = trayNormal.AngleOnPlaneTo(cupNormal, trayDirection);
        
        double sign = Math.Sign(trayDirection.DotProduct(cupNormal.CrossProduct(trayNormal)));
        if (sign == 0) 
            sign = 1;
        
        var axe2 = Line.CreateBound(locPoint.Point, locPoint.Point + normal2);
        
        doc.RunInTransaction("Поворот1", () =>
            ElementTransformUtils.RotateElement(doc, famInstance.Id, axe2, -angle2 * sign)
        );
    }


    private void RotateCupOnTrayDirection(Document doc, CableTray tray, FamilyInstance famInstance, XYZ trayDirection)
    {
        var locPoint = famInstance.Location as LocationPoint;

        var instanceDirection = famInstance.HandOrientation;

        // нормаль к плоскости в которой лежат направления короба и крышки
        var normal = trayDirection.CrossProduct(instanceDirection).Normalize();
        if (normal.IsAlmostEqualTo(XYZ.Zero))
        {
            normal = instanceDirection.Z == 1 ? XYZ.BasisX : XYZ.BasisZ;
        }

        // угол между коробом и крышкой
        var angle = trayDirection.AngleOnPlaneTo(instanceDirection, normal);

        var axe = Line.CreateBound(locPoint.Point, locPoint.Point + normal);

        doc.RunInTransaction("Поворот2", () => ElementTransformUtils.RotateElement(doc, famInstance.Id, axe, -angle));
    }
}

public static class DocumentExtention
{
    public static void RunInTransaction(this Document doc, string name, Action action)
    {
        using var tr = new Transaction(doc, name);
        tr.Start();
        action();
        tr.Commit();
    }

    public static T RunInTransaction<T>(this Document doc, string name, Func<T> func)
    {
        using var tr = new Transaction(doc, name);
        tr.Start();
        var result = func();
        tr.Commit();
        return result;
    }
}