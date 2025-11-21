using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using RPToolsUI.Models;
using RPToolsUI.Services;

namespace WarmSync;

[Transaction(TransactionMode.Manual)]
public class CreateSpaces : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var uiapp = commandData.Application;
        var uidoc = uiapp.ActiveUIDocument;
        var doc = uidoc.Document;

        try
        {
            // Получаем линк доки
            var links = new FilteredElementCollector(doc)
                .OfClass(typeof(RevitLinkInstance))
                .Cast<RevitLinkInstance>()
                .ToList();

            List<Document> linkDocs = new List<Document>();
            foreach (var link in links)
            {
                var linkDoc = link.GetLinkDocument();
                if (linkDoc != null)
                    linkDocs.Add(linkDoc);
            }

            // Комнаты из линков
            List<SpatialElement> rooms = new();
            foreach (var linkDoc in linkDocs)
            {
                var r = new FilteredElementCollector(linkDoc)
                    .OfCategory(BuiltInCategory.OST_Rooms)
                    .WhereElementIsNotElementType()
                    .Cast<SpatialElement>()
                    .Where(x => x.Area > 0)
                    .ToList();

                rooms.AddRange(r);
            }

            // Уровни и существующие пространства
            var levels = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Levels)
                .WhereElementIsNotElementType()
                .Cast<Level>()
                .ToList();

            var spaces = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_MEPSpaces)
                .WhereElementIsNotElementType()
                .Cast<Space>()
                .ToList();

            using (Transaction t = new Transaction(doc, "Create Spaces from Rooms"))
            {
                t.Start();

                foreach (var room in rooms)
                {
                    var roomLocation = room.Location as LocationPoint;
                    if (roomLocation == null) continue;

                    XYZ pt = roomLocation.Point;
                    bool exists = spaces.Any(space => space.IsPointInSpace(new XYZ(pt.X, pt.Y, pt.Z + 1)));

                    if (exists) continue;

                    // Находим уровень
                    double elev = Math.Round(room.Level.Elevation);
                    var targetLevel = levels.FirstOrDefault(x => Math.Round(x.Elevation) == elev);
                    if (targetLevel == null) continue;

                    UV uv = new UV(pt.X, pt.Y);

                    // Создаем Space
                    var newSpace = doc.Create.NewSpace(targetLevel, uv);

                    newSpace.get_Parameter(BuiltInParameter.ROOM_NAME).Set(room.get_Parameter(BuiltInParameter.ROOM_NAME).AsString());
                    newSpace.get_Parameter(BuiltInParameter.ROOM_NUMBER).Set(room.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString());
                    newSpace.get_Parameter(BuiltInParameter.ROOM_UPPER_OFFSET).Set(room.get_Parameter(BuiltInParameter.ROOM_UPPER_OFFSET).AsDouble());
                }

                t.Commit();
            }
            
            var dial = ToadDialogService.Show(
                "Успех!",
                "Пространства успешно созданы",
                DialogButtons.OK,
                DialogIcon.Info
            );

            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            var dial = ToadDialogService.Show(
                "Ошибка",
                $"Ошибка при создании пространств: {ex.Message}",
                DialogButtons.OK,
                DialogIcon.Info
            );
            return Result.Failed;
        }
    }
}
