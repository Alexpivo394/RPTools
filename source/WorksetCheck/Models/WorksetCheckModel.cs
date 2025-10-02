using Autodesk.Revit.UI;
using RPToolsUI.Models;
using RPToolsUI.Services;
using WorksetCheck.Services;

namespace WorksetCheck.Models;

public class WorksetCheckModel
{
    private ExternalCommandData _commandData { get; }

    public WorksetCheckModel(ExternalCommandData commandData)
    {
        _commandData = commandData;
    }

    public void CheckWorksets(string filePath, string worksetLinksName, string worksetAxesAndLevelsName,
        string worksetHolesName)
    {
        Logger log = new();
        CheckService checkService = new();
        OpenModelService openModelService = new();

        var doc = openModelService.OpenDocumentAsDetach(_commandData, filePath);

        log.StartLog(doc?.Title);

        var report = checkService.CheckWorksets(doc, worksetLinksName, worksetAxesAndLevelsName, worksetHolesName);

        if (report.Any())
        {
            log.LogList(report);
        }
        else
        {
            log.Log("ОШИБКА: Ошибка получения отчета.");
        }
    }
}