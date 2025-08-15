using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using RPToolsUI.Models;
using RPToolsUI.Services;

namespace QuantityCheck.Commands;

[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class StartupCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        // Добавить что-то, возможно DI

        string dildo = ToadDialogService.Show(
            "ЗАГЛОТ",
            "Сообщенькаете?",
            DialogButtons.OK,
            DialogIcon.Info
        );
            
        
        return Result.Succeeded;
    }
}