using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

namespace ReinforcementByColor;

[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class ReinforcementByColorCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var uiDoc = commandData.Application.ActiveUIDocument;
        var doc = uiDoc.Document;
        
        
        return Result.Succeeded;
    }
}