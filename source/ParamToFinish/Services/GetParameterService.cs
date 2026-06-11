using ParamToFinish.Models;

namespace ParamToFinish.Services;

public class GetParameterService
{
    public List<ParameterDescriptor> GetWallParameters(Document document)
    {
        var wall = new FilteredElementCollector(document)
            .OfClass(typeof(Wall))
            .Cast<Wall>()
            .FirstOrDefault();

        if (wall == null)
            return [];

        var parameterNames = new HashSet<string?>();

        foreach (Parameter parameter in wall.Parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameter.Definition?.Name))
                parameterNames.Add(parameter.Definition?.Name);
        }

        var wallType = document.GetElement(wall.GetTypeId());

        foreach (Parameter parameter in wallType.Parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameter.Definition?.Name))
                parameterNames.Add(parameter.Definition?.Name);
        }

        return parameterNames
            .OrderBy(x => x)
            .Select(x => new ParameterDescriptor
            {
                Name = x
            })
            .ToList();
    }
}