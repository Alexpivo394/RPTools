using Nice3point.Revit.Toolkit;
using ToadTools.UI.Models;
using ToadTools.UI.Services;
using WorksetCheck.Services;

namespace WorksetCheck.Models;

public class WorksetCheckModel
{
    public void CheckWorksets(string filePath, string worksetLinksName, string worksetAxesAndLevelsName,
        string worksetHolesName)
    {
        Logger log = new();
        CheckService checkService = new();
        OpenModelService openModelService = new();

        var doc = openModelService.OpenDocumentAsDetach(RevitContext.UiApplication!, filePath);

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