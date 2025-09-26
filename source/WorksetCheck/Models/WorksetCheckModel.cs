using Autodesk.Revit.UI;
using WorksetCheck.Services;

namespace WorksetCheck.Models;

public class WorksetCheckModel
{
    private ExternalCommandData _commandData { get; }

    public WorksetCheckModel(ExternalCommandData commandData)
    {
        _commandData = commandData;
    }

    public void CheckWorksets(string filePath, string worksetLinksName, string worksetAxesAndLevelsName, string worksetHolesName)
    {
        Logger log = new();
        CheckService checkService = new();
        OpenModelService openModelService = new();
        
        log.StartLog();

        var doc = openModelService.OpenDocumentAsDetach(_commandData, filePath);
        
        var report = checkService.CheckWorksets(doc, worksetLinksName, worksetAxesAndLevelsName, worksetHolesName);
        
        if (report.Any())
        {
            
        }
        else
        {
            
        }
    }
}