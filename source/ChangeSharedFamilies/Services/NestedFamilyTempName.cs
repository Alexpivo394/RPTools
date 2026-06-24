using Autodesk.Revit.DB;
using System.IO;

namespace ChangeSharedFamilies.Services;

internal static class NestedFamilyTempName
{
    public static string CreateUnique(Document document, string oldFamilyName)
    {
        var existingNames = new HashSet<string>(
            new FilteredElementCollector(document)
                .OfClass(typeof(Family))
                .Cast<Family>()
                .Select(x => x.Name),
            StringComparer.OrdinalIgnoreCase);

        var safeBaseName = MakeSafeFileName(oldFamilyName);
        var baseTempName = safeBaseName + "__NON_SHARED_TMP";

        var tempName = baseTempName;
        var index = 1;

        while (existingNames.Contains(tempName))
        {
            tempName = baseTempName + "_" + index;
            index++;
        }

        return tempName;
    }

    private static string MakeSafeFileName(string name)
    {
        foreach (var invalidChar in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(invalidChar, '_');
        }

        return name;
    }
}
