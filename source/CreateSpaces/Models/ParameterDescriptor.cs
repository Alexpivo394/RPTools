namespace CreateSpaces.Models;

public class ParameterDescriptor
{
    public string Name { get; set; }
    public StorageType StorageType { get; set; }
    public bool IsShared { get; set; }
    public bool IsReadOnly { get; set; }
    public bool IsInstance { get; set; }
    public BuiltInParameter? BuiltInParameter { get; set; }

    public override string? ToString() => Name;
}