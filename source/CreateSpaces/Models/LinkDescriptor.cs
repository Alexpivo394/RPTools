namespace CreateSpaces.Models;

public class LinkDescriptor
{
    public string? Name { get; set; }
    
    public override string? ToString() => Name;
}