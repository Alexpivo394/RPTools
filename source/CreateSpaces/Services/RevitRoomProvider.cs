using Autodesk.Revit.DB.Architecture;
using CreateSpaces.Models;

namespace CreateSpaces.Services;

public class RevitRoomProvider
{
    private readonly Document _doc;

    public RevitRoomProvider(Document document)
    {
        _doc = document ?? throw new ArgumentNullException(nameof(document));
    }
    
    public IReadOnlyList<Room?> GetRoomsFromLink(LinkDescriptor link)
    {
        if (link == null || string.IsNullOrEmpty(link.Name))
            return Array.Empty<Room>();
        
        var linkInstance = new FilteredElementCollector(_doc)
            .OfClass(typeof(RevitLinkInstance))
            .Cast<RevitLinkInstance>()
            .FirstOrDefault(x => x.GetLinkDocument()?.Title == link.Name);

        if (linkInstance == null)
            return Array.Empty<Room>();

        var linkedDoc = linkInstance.GetLinkDocument();
        if (linkedDoc == null)
            return Array.Empty<Room>();
        
        List<Room?> rooms = new FilteredElementCollector(linkedDoc)
            .OfCategory(BuiltInCategory.OST_Rooms)
            .WhereElementIsNotElementType()
            .Cast<Room>()
            .ToList()!;

        return rooms;
    }
}