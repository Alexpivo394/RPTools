namespace CreateSpaces.Models;

public class ParameterMappingConfig
{
    public List<ParameterMappingItem> Items { get; set; } = new();
}

public class ParameterMappingItem
{
    public string? SpaceParameterName { get; set; } = string.Empty;
    public string? RoomParameterName { get; set; }
}

