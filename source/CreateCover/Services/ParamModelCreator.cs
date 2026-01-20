using CreateCover.Models;

namespace CreateCover.Services;

public class ParamModelCreator
{
    private FamilySymbol? _familySymbol;
    private Element _element;
    private GetParamService _getParamService;
    
    public ParamModelCreator(FamilySymbol? familySymbol, Element element, GetParamService getParamService)
    {
        _familySymbol = familySymbol;
        _element = element;
        _getParamService = getParamService;
    }

    public ParamModel Create()
    {
        var model = new ParamModel();
        
        var trayParams = _getParamService.GetFromElement(_element);
        
        foreach (var p in trayParams)
        {
            model.TrayParams?.Add(p);
        }
        
        var coverParams = _getParamService.GetFromSymbol(_familySymbol);

        foreach (var p in coverParams)
        {
            model.CoverParams?.Add(p);
        }
        
        return model;
    }
}