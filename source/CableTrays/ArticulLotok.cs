//Command running revit application

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ArticulLotok;

[Regeneration(RegenerationOption.Manual)]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        //Task Dialog =>
        var taskDialog = new TaskDialog("Task dialog");
        //Set application =>
        var uiapp = commandData.Application;
        var uidoc = uiapp.ActiveUIDocument;
        var app = uiapp.Application;
        var doc = uidoc.Document;

        var lotkiperforirovannye = new FilteredElementCollector(doc)
            .OfCategory(BuiltInCategory.OST_CableTray).WhereElementIsNotElementType()
            .Where(lotok =>
                doc.GetElement(lotok.GetTypeId()).LookupParameter("ADSK_Наименование (по типу)").AsString() ==
                "Лоток перфорированный")
            .ToList();
        var lotki = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_CableTray)
            .WhereElementIsNotElementType()
            .Where(lotok =>
                doc.GetElement(lotok.GetTypeId()).LookupParameter("ADSK_Наименование (по типу)").AsString() ==
                "Лоток неперфорированный")
            .ToList();
        var provolocka = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_CableTray)
            .WhereElementIsNotElementType()
            .Where(lotok =>
                doc.GetElement(lotok.GetTypeId()).LookupParameter("ADSK_Наименование (по типу)").AsString() ==
                "Лоток проволочный")
            .ToList();

        var alllotki = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_CableTray)
            .WhereElementIsNotElementType()
            .Where(lotok =>
                doc.GetElement(lotok.GetTypeId()).LookupParameter("ADSK_Наименование (по типу)").AsString() !=
                "Лоток лестничный")
            .ToList();
        var stairs = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_CableTray)
            .WhereElementIsNotElementType()
            .Where(lotok =>
                doc.GetElement(lotok.GetTypeId()).LookupParameter("ADSK_Наименование (по типу)").AsString() ==
                "Лоток лестничный")
            .ToList();
        var alllotkirealall = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_CableTray)
            .WhereElementIsNotElementType()
            .ToList();

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

        var t0 = new Transaction(doc);


        //обнуление наименования и кода изделия
        foreach (var lotok in alllotkirealall)
            if (lotok.LookupParameter("ADSK_Исполнение").AsInteger() != 0)
            {
                t0.Start("Modify artikule0");
                lotok.LookupParameter("ADSK_Код изделия").Set("");
                lotok.LookupParameter("ADSK_Наименование").Set("");
                t0.Commit();
            }

        //лотки обычные
        foreach (var lotok in lotki)
        {
            var width = lotok.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM).AsValueString();
            var height = lotok.get_Parameter(BuiltInParameter.RBS_CABLETRAY_HEIGHT_PARAM).AsValueString();
            //проверка на ручное заполнение
            if (lotok.LookupParameter("ADSK_Исполнение").AsInteger() != 0)
            {
                artikul = "ЗНАЧЕНИЕ НЕ НАЙДЕНО"; //обнуление артикула 
                name = "Лоток неперфорированный, ВАРИАНТ ИСПОЛНЕНИЯ НЕ НАЙДЕН, ПОПРАВЬ"; //обнуление наименования 

                if (height == shirina50)
                {
                    if (width == shirina50) artikul = "35020";
                    if (width == shirina100) artikul = "35022";
                    if (width == shirina150) artikul = "35023";
                    if (width == shirina200) artikul = "35024";
                    if (width == shirina300) artikul = "35025";
                    if (width == shirina400) artikul = "35026";
                    if (width == shirina500) artikul = "35027";
                    if (width == shirina600) artikul = "35028";
                }

                if (height == shirina100)
                {
                    if (width == shirina100) artikul = "35101";
                    if (width == shirina150) artikul = "35102";
                    if (width == shirina200) artikul = "35103";
                    if (width == shirina300) artikul = "35104";
                    if (width == shirina400) artikul = "35105";
                    if (width == shirina500) artikul = "35106";
                    if (width == shirina600) artikul = "35107";
                }

                //если в списке выше есть значение артикула, то лоток попадает сюда, здесь заполняется исполнение для наименования и СУФФИКС для артикула
                if (artikul != "ЗНАЧЕНИЕ НЕ НАЙДЕНО")
                {
                    name = "Лоток неперфорированный " + width + "x" + height;
                    if (lotok.LookupParameter("ADSK_Исполнение").AsInteger() == 1)
                        name = name + ", оцинковка горячим способом по методу Сендзимира";
                    if (lotok.LookupParameter("ADSK_Исполнение").AsInteger() == 2)
                    {
                        artikul = artikul + "HDZ";
                        name = name + ", горячего цинкования";
                    }

                    if (lotok.LookupParameter("ADSK_Исполнение").AsInteger() == 3)
                    {
                        artikul = artikul + "INOX";
                        name = name + ", из нержавеющей стали";
                    }
                }

                t0.Start("Modify artikule1");

                lotok.LookupParameter("ADSK_Код изделия").Set(artikul);
                lotok.LookupParameter("ADSK_Наименование").Set(name);

                t0.Commit();
            }
            else //случай ручного заполнения наименования
            {
                t0.Start("Modify artikule2");

                lotok.LookupParameter("ADSK_Наименование").Set("Заполняй сам, ты же этого хотел");

                t0.Commit();
            }
        }

        //лотки перфорированные
        foreach (var lotok in lotkiperforirovannye)
        {
            var width = lotok.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM).AsValueString();
            var height = lotok.get_Parameter(BuiltInParameter.RBS_CABLETRAY_HEIGHT_PARAM).AsValueString();

            if (lotok.LookupParameter("ADSK_Исполнение").AsInteger() != 0)
            {
                artikul = "ЗНАЧЕНИЕ НЕ НАЙДЕНО";
                name = "Лоток перфорированный, ВАРИАНТ ИСПОЛНЕНИЯ НЕ НАЙДЕН, ПОПРАВЬ";
                if (height == shirina50)
                {
                    if (width == shirina50) artikul = "35260";
                    if (width == shirina100) artikul = "35262";
                    if (width == shirina150) artikul = "35263";
                    if (width == shirina200) artikul = "35264";
                    if (width == shirina300) artikul = "35265";
                    if (width == shirina400) artikul = "35266";
                    if (width == shirina500) artikul = "35267";
                    if (width == shirina600) artikul = "35268";
                }

                if (height == shirina100)
                {
                    if (width == shirina100) artikul = "35341";
                    if (width == shirina150) artikul = "35342";
                    if (width == shirina200) artikul = "35343";
                    if (width == shirina300) artikul = "35344";
                    if (width == shirina400) artikul = "35345";
                    if (width == shirina500) artikul = "35346";
                    if (width == shirina600) artikul = "35347";
                }

                if (artikul != "ЗНАЧЕНИЕ НЕ НАЙДЕНО")
                {
                    name = "Лоток перфорированный " + width + "x" + height;
                    if (lotok.LookupParameter("ADSK_Исполнение").AsInteger() == 1)
                        name = name + ", оцинковка горячим способом по методу Сендзимира";
                    if (lotok.LookupParameter("ADSK_Исполнение").AsInteger() == 2)
                    {
                        artikul = artikul + "HDZ";
                        name = name + ", горячего цинкования";
                    }

                    if (lotok.LookupParameter("ADSK_Исполнение").AsInteger() == 3)
                    {
                        artikul = artikul + "INOX";
                        name = name + ", из нержавеющей стали";
                    }
                }

                t0.Start("Modify artikule2");

                lotok.LookupParameter("ADSK_Код изделия").Set(artikul);
                lotok.LookupParameter("ADSK_Наименование").Set(name);

                t0.Commit();
            }
            else
            {
                t0.Start("Modify artikule2");

                lotok.LookupParameter("ADSK_Наименование").Set("Заполняй сам, ты же этого хотел");

                t0.Commit();
            }
        }

        //лотки лестничные
        foreach (var lotok in stairs)
        {
            var width = lotok.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM).AsValueString();
            var height = lotok.get_Parameter(BuiltInParameter.RBS_CABLETRAY_HEIGHT_PARAM).AsValueString();

            if (lotok.LookupParameter("ADSK_Исполнение").AsInteger() != 0)
            {
                artikul = "ЗНАЧЕНИЕ НЕ НАЙДЕНО";
                name = "Лоток лестничный, ВАРИАНТ ИСПОЛНЕНИЯ НЕ НАЙДЕН, ПОПРАВЬ";
                if (height == shirina50)
                {
                    if (width == shirina100) artikul = "LL5010";
                    if (width == shirina200) artikul = "LL5020";
                    if (width == shirina300) artikul = "LL5030";
                    if (width == shirina400) artikul = "LL5040";
                    if (width == shirina500) artikul = "LL5050";
                    if (width == shirina600) artikul = "LL5060";
                }

                if (height == shirina80)
                {
                    if (width == shirina200) artikul = "LL8020";
                    if (width == shirina300) artikul = "LL8030";
                    if (width == shirina400) artikul = "LL8040";
                    if (width == shirina500) artikul = "LL8050";
                    if (width == shirina600) artikul = "LL8060";
                }

                if (height == shirina100)
                {
                    if (width == shirina100) artikul = "LL1010";
                    if (width == shirina200) artikul = "LL1020";
                    if (width == shirina300) artikul = "LL1030";
                    if (width == shirina400) artikul = "LL1040";
                    if (width == shirina500) artikul = "LL1050";
                    if (width == shirina600) artikul = "LL1060";
                }

                if (artikul != "ЗНАЧЕНИЕ НЕ НАЙДЕНО")
                {
                    name = "Лоток лестничный " + width + "x" + height;
                    if (lotok.LookupParameter("ADSK_Исполнение").AsInteger() == 1)
                        name = name + ", оцинковка горячим способом по методу Сендзимира";
                    if (lotok.LookupParameter("ADSK_Исполнение").AsInteger() == 2)
                    {
                        artikul = artikul + "HDZ";
                        name = name + ", горячего цинкования";
                    }

                    if (lotok.LookupParameter("ADSK_Исполнение").AsInteger() == 3)
                    {
                        artikul = artikul + "INOX";
                        name = name + ", из нержавеющей стали";
                    }
                }

                t0.Start("Modify artikule3");

                lotok.LookupParameter("ADSK_Код изделия").Set(artikul);
                lotok.LookupParameter("ADSK_Наименование").Set(name);

                t0.Commit();
            }
            else
            {
                t0.Start("Modify artikule2");

                lotok.LookupParameter("ADSK_Наименование").Set("Заполняй сам, ты же этого хотел");

                t0.Commit();
            }
        }

        //лотки проволочные
        foreach (var lotok in provolocka)
        {
            var width = lotok.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM).AsValueString();
            var height = lotok.get_Parameter(BuiltInParameter.RBS_CABLETRAY_HEIGHT_PARAM).AsValueString();

            if (lotok.LookupParameter("ADSK_Исполнение").AsInteger() != 0)
            {
                artikul = "ЗНАЧЕНИЕ НЕ НАЙДЕНО";
                name = "Лоток проволочный, ВАРИАНТ ИСПОЛНЕНИЯ НЕ НАЙДЕН, ПОПРАВЬ";
                if (height == shirina50)
                {
                    if (width == shirina50) artikul = "FC5005";
                    if (width == shirina100) artikul = "FC5010";
                    if (width == shirina150) artikul = "FC5015";
                    if (width == shirina200) artikul = "FC5020";
                    if (width == shirina300) artikul = "FC5030";
                    if (width == shirina400) artikul = "FC5040";
                    if (width == shirina500) artikul = "FC5050";
                    if (width == shirina600) artikul = "FC5060";
                }

                if (height == shirina80)
                {
                    if (width == shirina80) artikul = "FC8080";
                    if (width == shirina100) artikul = "FC8010";
                    if (width == shirina150) artikul = "FC8015";
                    if (width == shirina200) artikul = "FC8020";
                    if (width == shirina300) artikul = "FC8030";
                    if (width == shirina400) artikul = "FC8040";
                    if (width == shirina500) artikul = "FC8050";
                    if (width == shirina600) artikul = "FC8060";
                }

                if (height == shirina100)
                {
                    if (width == shirina100) artikul = "FC1010";
                    if (width == shirina150) artikul = "FC1015";
                    if (width == shirina200) artikul = "FC1020";
                    if (width == shirina300) artikul = "FC1030";
                    if (width == shirina400) artikul = "FC1040";
                    if (width == shirina500) artikul = "FC1050";
                    if (width == shirina600) artikul = "FC1060";
                }

                if (artikul != "ЗНАЧЕНИЕ НЕ НАЙДЕНО")
                {
                    name = "Лоток проволочный " + width + "x" + height;
                    if (lotok.LookupParameter("ADSK_Исполнение").AsInteger() == 1)
                        name = name + ", оцинковка горячим способом по методу Сендзимира";
                    if (lotok.LookupParameter("ADSK_Исполнение").AsInteger() == 2)
                    {
                        artikul = artikul + "HDZ";
                        name = name + ", горячего цинкования";
                    }

                    if (lotok.LookupParameter("ADSK_Исполнение").AsInteger() == 3)
                    {
                        artikul = artikul + "INOX";
                        name = name + ", из нержавеющей стали";
                    }
                }

                t0.Start("Modify artikule3");

                lotok.LookupParameter("ADSK_Код изделия").Set(artikul);
                lotok.LookupParameter("ADSK_Наименование").Set(name);

                t0.Commit();
            }
            else
            {
                t0.Start("Modify artikule2");

                lotok.LookupParameter("ADSK_Наименование").Set("Заполняй сам, ты же этого хотел");

                t0.Commit();
            }
        }

        foreach (var lotok in alllotkirealall)
        {
            var length = lotok.LookupParameter("Длина").AsDouble();
#if REVIT2021_OR_GREATER
            var reallength =
                Math.Round(UnitUtils.ConvertFromInternalUnits(length, UnitTypeId.Millimeters) / 100) / 10;
#else
            var reallength =
                Math.Round(UnitUtils.ConvertFromInternalUnits(length, DisplayUnitType.DUT_MILLIMETERS) / 100) / 10;
#endif
            t0.Start("Modify lenght");

            lotok.LookupParameter("ADSK_Количество").Set(reallength);

            t0.Commit();
        }

        taskDialog.MainContent = "Готовенько!";
        taskDialog.Show();
        return Result.Succeeded;
    }
}