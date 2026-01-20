namespace CreateCover.Models;

public class ParameterDescriptor
{
    public string? Name { get; set; }
    public ElementId? Id { get; set; }
    public StorageType StorageType { get; set; }
    public bool IsInstance { get; set; }
    public bool IsShared { get; set; }

    public override string? ToString() => Name;
}