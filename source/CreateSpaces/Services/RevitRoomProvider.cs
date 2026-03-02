using Autodesk.Revit.DB.Architecture;
using CreateSpaces.Models;

namespace CreateSpaces.Services;

public class RevitRoomProvider
{
    private readonly Document _doc;
    private IReadOnlyList<Room?>?  _rooms;

    public RevitRoomProvider(Document document)
    {
        _doc = document ?? throw new ArgumentNullException(nameof(document));
    }

    public void Initialize(LinkDescriptor link)
    {
        if (link == null || string.IsNullOrEmpty(link.Name))
            _rooms = Array.Empty<Room>();
        
        var linkInstance = new FilteredElementCollector(_doc)
            .OfClass(typeof(RevitLinkInstance))
            .Cast<RevitLinkInstance>()
            .FirstOrDefault(x => x.GetLinkDocument()?.Title == link?.Name);

        if (linkInstance == null)
            _rooms = Array.Empty<Room>();

        var linkedDoc = linkInstance?.GetLinkDocument();
        if (linkedDoc == null)
            _rooms = Array.Empty<Room>();
        
        List<Room?>? rooms = new FilteredElementCollector(linkedDoc)
            .OfCategory(BuiltInCategory.OST_Rooms)
            .WhereElementIsNotElementType()
            .Cast<Room>()
            .ToList()!;
        
        _rooms = rooms;
    }
    
    public IReadOnlyList<Room?>? GetRoomsFromLink()
    {
        return _rooms;
    }

    public bool DetectRooms()
    {
        if (_rooms!.Count == 0)
            return false;
        
        return true;
    }
}