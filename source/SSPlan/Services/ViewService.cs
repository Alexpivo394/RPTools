using Autodesk.Revit.UI;

namespace SSPlan.Services;

public class ViewService
{
    private readonly UIDocument _uiDoc;

    public ViewService(UIDocument uiDoc)
    {
        _uiDoc = uiDoc;
    }

    /// <summary>
    /// Проверяет, является ли активный вид чертежным и возвращает его.
    /// Если не чертежный — возвращает null.
    /// </summary>
    public View? GetActiveDraftingOrSheetView()
    {
        if (_uiDoc == null || _uiDoc.ActiveView == null)
            return null;

        View? activeView = _uiDoc.ActiveView;


        if (activeView is ViewDrafting)
        {
            return activeView;
        }

        return null;
    }
}