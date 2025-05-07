using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

namespace WorkingSet.Models;


public class WorkingSetModel
{
    

    private readonly string _excelFilePath;

    public WorkingSetModel(string excelFilePath)
    {
        _excelFilePath = excelFilePath;
    }

    // Получение списка разделов из Excel
    public List<string> GetSections()
    {
        ExcelPackage.License.SetNonCommercialPersonal("RPTools");

        var sections = new List<string>();
        using (var package = new ExcelPackage(new FileInfo(_excelFilePath)))
        {
            foreach (var worksheet in package.Workbook.Worksheets)
            {
                sections.Add(worksheet.Name);
            }
        }
        return sections;
    }

    // Получение рабочих наборов из конкретного раздела
    public List<string> GetWorksetsFromSection(string sectionName)
    {
        var worksets = new List<string>();

        ExcelPackage.License.SetNonCommercialPersonal("RPTools");

        using (var package = new ExcelPackage(new FileInfo(_excelFilePath)))
        {
            var worksheet = package.Workbook.Worksheets[sectionName];

            if (worksheet == null) return worksets;

            var row = 1;
            while (true)
            {
                var worksetName = worksheet.Cells[row, 1].Text;
                if (string.IsNullOrEmpty(worksetName)) break;

                worksets.Add(worksetName);
                row++;
            }
        }

        return worksets;
    }
}