#nullable enable
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using OfficeOpenXml;
using RPToolsUI.Models;
using RPToolsUI.Services;

namespace WarmSync;

[Transaction(TransactionMode.Manual)]
public class WriteFromExcel : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        UIApplication uiapp = commandData.Application;
        UIDocument uidoc = uiapp.ActiveUIDocument;
        Document doc = uidoc.Document;

        string excelPath = "";

        var openFileDialog = new OpenFileDialog
        {
            Filter = "Excel Files (*.xlsx; *.xls)|*.xlsx;*.xls|All files (*.*)|*.*",
            Title = "Выберите Excel файл",
            DefaultExt = ".xlsx",
            CheckFileExists = true,
            CheckPathExists = true,
            Multiselect = false,
            RestoreDirectory = true
        };

        if (openFileDialog.ShowDialog() == true) excelPath = openFileDialog.FileName;

        // ✨ ЛОГГЕР
        Logger logger = new Logger();
        string logPath = Path.Combine(Path.GetDirectoryName(excelPath)!, "WarmSyncLog.txt");
        logger.StartLog(logPath);

        logger.Log($"Выбран файл: {excelPath}");

        ExcelPackage.License.SetNonCommercialPersonal("RPTools");

        int updated = 0;
        int errors = 0;

        using (var package = new ExcelPackage(new FileInfo(excelPath)))
        {
            var ws = package.Workbook.Worksheets[0];
            int rowCount = ws.Dimension.Rows;

            using (Transaction t = new Transaction(doc, "Запись параметров"))
            {
                t.Start();

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var idVal = ws.Cells[row, 1].Value;
                        var tempVal = ws.Cells[row, 7].Value;
                        var heatVal = ws.Cells[row, 8].Value;

                        if (idVal == null)
                        {
                            logger.Log($"Строка {row}: ID пустой — пропускаю.");
                            continue;
                        }

                        if (!int.TryParse(idVal.ToString(), out int idInt))
                        {
                            logger.Log($"Строка {row}: ID '{idVal}' — не число.");
                            errors++;
                            continue;
                        }

                        ElementId elemId = new ElementId(idInt);
                        Element? el = doc.GetElement(elemId);

                        if (el == null)
                        {
                            logger.Log($"Строка {row}: элемент с ID {idInt} не найден.");
                            errors++;
                            continue;
                        }

                        if (!SetParam(el, "ADSK_Температура в помещении", tempVal, row, logger)) errors++;
                        if (!SetParam(el, "ADSK_Теплопотери", heatVal, row, logger)) errors++;

                        updated++;
                    }
                    catch (Exception ex)
                    {
                        errors++;
                        logger.LogError($"Строка {row}: Все сломалось(((", ex);
                    }
                }

                t.Commit();
            }
        }

        var dial = ToadDialogService.Show("Результат", $"Обновлено: {updated}\nОшибок: {errors}\n\nЛог:\n{logPath}",
            DialogButtons.OK, DialogIcon.Info);

        return Result.Succeeded;
    }

    private bool SetParam(Element el, string paramName, object? value, int row, Logger logger)
{
    Parameter? p = el.LookupParameter(paramName);

    if (p == null)
    {
        logger.Log($"Строка {row}: У элемента ID {el.Id} нет параметра '{paramName}'");
        return false;
    }

    if (value == null)
    {
        logger.Log($"Строка {row}: '{paramName}' пустое — пропускаю.");
        return true;
    }

    string stringVal = value.ToString()!;

    try
    {
        switch (p.StorageType)
        {
            case StorageType.Double:
                if (!double.TryParse(stringVal, NumberStyles.Any, CultureInfo.InvariantCulture, out double dbl))
                {
                    logger.Log($"Строка {row}: '{paramName}' не число: '{stringVal}'");
                    return false;
                }
                
                if (p.Definition.GetDataType() == SpecTypeId.HeatingLoad)
                {
                    // Конвертируем Вт в Btu/h явно, если Revit не делает это автоматически
                    double valueInBtuPerHour = dbl * 10.7639;
                    p.Set(valueInBtuPerHour);
                    logger.Log($"ID {el.Id}: {paramName} = {dbl} Вт → {valueInBtuPerHour} Bт/фт2");
                }
                else if (p.Definition.GetDataType() == SpecTypeId.HvacTemperature)
                {
                    // Excel даёт °C, Revit internal = K
                    double internalVal = dbl + 273.15;
                    p.Set(internalVal);
                    logger.Log($"ID {el.Id}: {paramName} = {dbl}°C → internal {internalVal}K");
                }
                else
                {
                    // Любой другой double-параметр
                    p.Set(dbl);
                    logger.Log($"ID {el.Id}: {paramName} = {dbl}");
                }
                break;

            case StorageType.Integer:
                if (!int.TryParse(stringVal, out int i))
                {
                    logger.Log($"Строка {row}: '{paramName}' не int: '{stringVal}'");
                    return false;
                }

                p.Set(i);
                logger.Log($"ID {el.Id}: {paramName} = {i}");
                break;

            case StorageType.String:
                p.Set(stringVal);
                logger.Log($"ID {el.Id}: {paramName} = '{stringVal}'");
                break;

            default:
                logger.Log($"Строка {row}: Неподдерживаемый StorageType для '{paramName}'");
                return false;
        }

        return true;
    }
    catch (Exception ex)
    {
        logger.LogError($"Строка {row}: ошибка записи '{paramName}'", ex);
        return false;
    }
}
}