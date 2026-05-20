using WriteDash.Models;

namespace WriteDash.Services;

public class RevitParameterService
{
    public List<ParameterDescriptor> GetProjectParameters(Document doc)
    {
        var result = new List<ParameterDescriptor>();

        var bindings = doc.ParameterBindings;

        var iterator = bindings.ForwardIterator();

        iterator.Reset();

        while (iterator.MoveNext())
        {
            if (iterator.Key is not Definition definition)
                continue;

            result.Add(new ParameterDescriptor
            {
                Name = definition.Name
            });
        }

        return result
            .OrderBy(x => x.Name)
            .ToList();
    }
}