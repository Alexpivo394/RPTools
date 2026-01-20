namespace CreateCover.Services;

public static class DocumentExtention
{
    public static void RunInTransaction(this Document doc, string name, Action action)
    {
        using var tr = new Transaction(doc, name);
        tr.Start();
        action();
        tr.Commit();
    }

    public static T RunInTransaction<T>(this Document doc, string name, Func<T> func)
    {
        using var tr = new Transaction(doc, name);
        tr.Start();
        var result = func();
        tr.Commit();
        return result;
    }
}