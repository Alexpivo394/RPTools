using Autodesk.Revit.DB;
using System.Linq;
using Autodesk.Revit.DB.ExtensibleStorage;
using CreateSpaces.Models;

namespace CreateSpaces.Services;

public class RevitLinkProvider
{
    public IReadOnlyList<LinkDescriptor> GetLinks(Document document)
    {
        if (document == null)
            throw new ArgumentNullException(nameof(document));

        return new FilteredElementCollector(document)
            .OfClass(typeof(RevitLinkInstance))
            .Cast<RevitLinkInstance>()
            .Select(x => x.GetLinkDocument())
            .Where(x => x != null)
            .GroupBy(x => x!.Title)
            .Select(g => new LinkDescriptor
            {
                Name = g.Key
            })
            .ToList();

    }
    
    public RevitLinkInstance? GetByName(Document doc, string? name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        return new FilteredElementCollector(doc)
            .OfClass(typeof(RevitLinkInstance))
            .Cast<RevitLinkInstance>()
            .FirstOrDefault(x => x.Name.Contains(name));
    }
}
