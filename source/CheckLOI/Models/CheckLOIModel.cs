using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Windows;

namespace CheckLOI.Models
{
    public class CheckLOIModel
    {
        public async Task<ProcessingResult> ProcessModelAsync(
            ExternalCommandData commandData,
            List<string> parameters,
            List<BuiltInCategory> selectedCategories,
            bool exportToExistingFile,
            string modelPath,
            string viewName = "Navisworks",
            string worksetKeyword = "00")
        {
            try
            {
                if (string.IsNullOrEmpty(modelPath))
                {
                    return new ProcessingResult
                    {
                        IsSuccess = false,
                        Message = "Путь к модели не указан."
                    };
                }

                // Open document in background thread
                var doc = OpenDocumentAsDetach(commandData, modelPath, worksetKeyword);
                if (doc == null)
                {
                    return new ProcessingResult
                    {
                        IsSuccess = false,
                        Message = "Не удалось открыть модель."
                    };
                }

                // Find the specified view
                var view = FindViewByName(doc, viewName);
                if (view == null)
                {
                    doc.Close(false);
                    return new ProcessingResult
                    {
                        IsSuccess = false,
                        Message = $"Вид '{viewName}' не найден."
                    };
                }

                // Process elements in background thread
                var elementsByCategory = ProcessElements(doc, selectedCategories, view);
                if (!elementsByCategory.Any())
                {
                    doc.Close(false);
                    return new ProcessingResult
                    {
                        IsSuccess = false,
                        Message = $"Не найдены элементы из выбранных категорий в виде '{viewName}'."
                    };
                }

                var processedDocTitle = ProcessDocTitle(doc.Title);

                // Generate reports
                await Task.Run(() =>
                {
                    if (exportToExistingFile)
                    {
                        UpdateExistingExcelReport(elementsByCategory, parameters, processedDocTitle);
                    }
                    GenerateNewExcelReport(elementsByCategory, parameters, processedDocTitle);
                });

                doc.Close(false);

                return new ProcessingResult
                {
                    IsSuccess = true,
                    Message = "Отчет успешно создан на рабочем столе."
                };
            }
            catch (Exception ex)
            {
                return new ProcessingResult
                {
                    IsSuccess = false,
                    Message = $"Произошла ошибка: {ex.Message}"
                };
            }
        }

        private Document OpenDocumentAsDetach(ExternalCommandData commandData, string filePath, string badNameWorkset)
        {
            var app = commandData.Application;
            var modelPathServ = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
            var controlledApp = app.Application;

            var openOptions = new OpenOptions();

            try
            {
                openOptions.DetachFromCentralOption = DetachFromCentralOption.DetachAndPreserveWorksets;
                var worksetConfiguration =
                    new WorksetConfiguration(WorksetConfigurationOption.CloseAllWorksets);

                IList<WorksetPreview> worksets = WorksharingUtils.GetUserWorksetInfo(modelPathServ);
                IList<WorksetId> worksetIds;
                if (string.IsNullOrEmpty(badNameWorkset))
                {
                    worksetIds = worksets.Select(workset => workset.Id).ToList();
                }
                else
                {
                    worksetIds = worksets
                        .Where(workset => !workset.Name.ToLower().Contains(badNameWorkset.ToLower()))
                        .Select(workset => workset.Id)
                        .ToList();
                }
                worksetConfiguration.Open(worksetIds);

                openOptions.SetOpenWorksetsConfiguration(worksetConfiguration);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            FailureProcessorOpenDocument failureProcessor = null;
            Document openDoc = null;

            try
            {
                failureProcessor = new FailureProcessorOpenDocument();

                controlledApp.FailuresProcessing += failureProcessor.ApplicationOnFailuresProcessing;
                app.DialogBoxShowing += failureProcessor.UIApplicationOnDialogBoxShowing;

                using (var transGroup = new TransactionGroup(app.ActiveUIDocument.Document, "Open Document Group"))
                {
                    transGroup.Start();

                    using (var transaction = new Transaction(app.ActiveUIDocument.Document, "Open Document"))
                    {
                        transaction.Start();
                        openDoc = controlledApp.OpenDocumentFile(modelPathServ, openOptions);
                        transaction.Commit();
                    }

                    transGroup.Assimilate();
                }

                if (openDoc != null && openDoc.IsValidObject) return openDoc;

                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                throw;
            }
            finally
            {
                if (failureProcessor != null)
                {
                    controlledApp.FailuresProcessing -= failureProcessor.ApplicationOnFailuresProcessing;
                    app.DialogBoxShowing -= failureProcessor.UIApplicationOnDialogBoxShowing;
                }
            }
        }

        private View3D FindViewByName(Document doc, string viewName)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(View3D))
                .Cast<View3D>()
                .FirstOrDefault(v => v.Name == viewName);
        }

        private Dictionary<string, List<Element>> ProcessElements(Document doc, List<BuiltInCategory> selectedCategories, Autodesk.Revit.DB.View view)
        {
            var collector = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .WherePasses(new ElementMulticategoryFilter(selectedCategories))
                .Where(e => e.IsHidden(view) == false);

            var elementsByCategory = new Dictionary<string, List<Element>>();
            foreach (var element in collector)
            {
                var category = element.Category?.Name ?? "Без категории";
                if (!elementsByCategory.ContainsKey(category)) elementsByCategory[category] = new List<Element>();
                elementsByCategory[category].Add(element);
            }

            return elementsByCategory;
        }

        private string ProcessDocTitle(string docTitle)
        {
            var lastUnderscoreIndex = docTitle.LastIndexOf('_');
            return lastUnderscoreIndex > -1 ? docTitle.Substring(0, lastUnderscoreIndex) : docTitle;
        }

        private void GenerateNewExcelReport(Dictionary<string, List<Element>> elementsByCategory,
            List<string> parameters, string docTitle)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Report");

                worksheet.Cells[1, 1].Value = "Категория";
                worksheet.Cells[1, 2].Value = "Имя элемента";
                worksheet.Cells[1, 3].Value = "ID элемента";

                for (var i = 0; i < parameters.Count; i++) worksheet.Cells[1, 4 + i].Value = parameters[i];

                var row = 2;
                int redCount = 0, yellowCount = 0, greenCount = 0;

                foreach (var category in elementsByCategory)
                    foreach (var element in category.Value)
                    {
                        worksheet.Cells[row, 1].Value = category.Key;
                        worksheet.Cells[row, 2].Value = element.Name;
                        worksheet.Cells[row, 3].Value = element.Id.IntegerValue;

                        for (var i = 0; i < parameters.Count; i++)
                        {
                            var param = element.LookupParameter(parameters[i]);
                            try
                            {
                                if (param == null)
                                {
                                    var typeId = element.GetTypeId();
                                    if (typeId != ElementId.InvalidElementId)
                                    {
                                        var elementType = element.Document.GetElement(typeId) as ElementType;
                                        if (elementType != null) param = elementType.LookupParameter(parameters[i]);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                            }

                            var cell = worksheet.Cells[row, 4 + i];
                            if (param == null)
                            {
                                cell.Value = "N/A";
                                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                                redCount++;
                            }
                            else if (param.StorageType == StorageType.String)
                            {
                                cell.Value = param.AsString();
                            }
                            else if (param.StorageType == StorageType.Double)
                            {
                                cell.Value = param.AsDouble().ToString();
                            }
                            else if (param.StorageType == StorageType.Integer)
                            {
                                cell.Value = param.AsInteger().ToString();
                            }
                            else
                            {
                                cell.Value = param.AsValueString();
                            }

                            if (cell.Value == null ||
                                (cell.Value is string && string.IsNullOrWhiteSpace((string)cell.Value)))
                            {
                                cell.Value = string.Empty;
                                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                                yellowCount++;
                            }
                            else if ((string)cell.Value == "N/A")
                            {
                                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                            }
                            else
                            {
                                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);
                                greenCount++;
                            }
                        }

                        row++;
                    }

                var totalCount = redCount + yellowCount + greenCount;
                worksheet.Cells[row + 1, 1].Value = "Итоги";
                worksheet.Cells[row + 2, 1].Value = "Отсутствующих параметров";
                worksheet.Cells[row + 2, 2].Value = redCount;
                worksheet.Cells[row + 3, 1].Value = "Пустых параметров";
                worksheet.Cells[row + 3, 2].Value = yellowCount;
                worksheet.Cells[row + 4, 1].Value = "Заполненных параметров";
                worksheet.Cells[row + 4, 2].Value = greenCount;
                worksheet.Cells[row + 5, 1].Value = "Всего параметров";
                worksheet.Cells[row + 5, 2].Value = totalCount;

                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var fileName = $"{docTitle}_LOIreport.xlsx";
                var filePath = Path.Combine(desktopPath, fileName);

                var file = new FileInfo(filePath);
                package.SaveAs(file);
            }
        }

        private void UpdateExistingExcelReport(Dictionary<string, List<Element>> elementsByCategory,
            List<string> parameters, string docTitle)
        {
            int redCount = 0, yellowCount = 0, greenCount = 0;
            var totalCount = 0;

            foreach (var category in elementsByCategory)
                foreach (var element in category.Value)
                    foreach (var paramName in parameters)
                    {
                        var param = element.LookupParameter(paramName);
                        try
                        {
                            if (param == null)
                            {
                                var typeId = element.GetTypeId();
                                if (typeId != ElementId.InvalidElementId)
                                {
                                    var elementType = element.Document.GetElement(typeId) as ElementType;
                                    if (elementType != null) param = elementType.LookupParameter(paramName);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }

                        if (param == null)
                            redCount++;
                        else if (param.StorageType == StorageType.String && string.IsNullOrWhiteSpace(param.AsString()))
                            yellowCount++;
                        else if (param.StorageType == StorageType.Double && param.AsDouble() == 0)
                            yellowCount++;
                        else if (param.StorageType == StorageType.Integer && param.AsInteger() == 0)
                            yellowCount++;
                        else
                            greenCount++;
                    }

            totalCount = redCount + yellowCount + greenCount;

            var networkFilePath = @"Y:\13-BIM (разработка)\07_Bim отдел\Проекты и модели.xlsx";
            var fileInfo = new FileInfo(networkFilePath);

            using (var package = new ExcelPackage(fileInfo))
            {
                if (package.Workbook.Worksheets.Count == 0)
                {
                    return;
                }

                var worksheet = package.Workbook.Worksheets.First();

                var modelFound = false;
                var row = 2;

                while (worksheet.Cells[row, 5].Value != null)
                {
                    if (worksheet.Cells[row, 5].Value.ToString() == docTitle)
                    {
                        worksheet.Cells[row, 13].Value = totalCount;
                        worksheet.Cells[row, 14].Value = greenCount;
                        worksheet.Cells[row, 15].Value = yellowCount;
                        worksheet.Cells[row, 16].Value = redCount;
                        modelFound = true;
                        break;
                    }

                    row++;
                }

                if (modelFound)
                {
                    package.Save();
                }
            }
        }
    }
}