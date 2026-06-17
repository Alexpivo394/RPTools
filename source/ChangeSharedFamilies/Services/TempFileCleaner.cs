using System.IO;

namespace ChangeSharedFamilies.Services;

internal static class TempFileCleaner
{
    public static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch
        {
            // Temp-файл не должен ронять всю команду.
        }
    }
}
