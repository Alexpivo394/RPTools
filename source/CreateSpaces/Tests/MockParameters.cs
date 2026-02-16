using CreateSpaces.Models;

namespace CreateSpaces.Tests;

public static class MockParameters
{
    public static List<ParameterDescriptor> GetRoomParameters()
    {
        return new()
        {
            new ParameterDescriptor
            {
                Name = "Номер помещения",
                StorageType = StorageType.String,
                IsInstance = true,
                IsShared = false
            },
            new ParameterDescriptor
            {
                Name = "Назначение",
                StorageType = StorageType.String,
                IsInstance = true,
                IsShared = true
            },
            new ParameterDescriptor
            {
                Name = "Площадь",
                StorageType = StorageType.Double,
                IsInstance = true,
                IsShared = false
            },
            new ParameterDescriptor
            {
                Name = "Площадь",
                StorageType = StorageType.Double,
                IsInstance = true,
                IsShared = false
            },
            new ParameterDescriptor
            {
                Name = "Площадь",
                StorageType = StorageType.Double,
                IsInstance = true,
                IsShared = false
            },
            new ParameterDescriptor
            {
                Name = "Площадь",
                StorageType = StorageType.Double,
                IsInstance = true,
                IsShared = false
            },
            new ParameterDescriptor
            {
                Name = "Площадь",
                StorageType = StorageType.Double,
                IsInstance = true,
                IsShared = false
            },
            new ParameterDescriptor
            {
                Name = "Площадь",
                StorageType = StorageType.Double,
                IsInstance = true,
                IsShared = false
            },
            new ParameterDescriptor
            {
                Name = "Площадь",
                StorageType = StorageType.Double,
                IsInstance = true,
                IsShared = false
            },
            new ParameterDescriptor
            {
                Name = "Площадь",
                StorageType = StorageType.Double,
                IsInstance = true,
                IsShared = false
            }
        };
    }

    public static List<ParameterDescriptor> GetSpaceParameters()
    {
        return new()
        {
            new ParameterDescriptor
            {
                Name = "Space_Number",
                StorageType = StorageType.String,
                IsInstance = true,
                IsShared = true
            },
            new ParameterDescriptor
            {
                Name = "Space_Function",
                StorageType = StorageType.String,
                IsInstance = true,
                IsShared = true
            },
            new ParameterDescriptor
            {
                Name = "Space_Area",
                StorageType = StorageType.Double,
                IsInstance = true,
                IsShared = false
            },
            new ParameterDescriptor
            {
                Name = "Space_Area",
                StorageType = StorageType.Double,
                IsInstance = true,
                IsShared = false
            },
            new ParameterDescriptor
            {
                Name = "Space_Area",
                StorageType = StorageType.Double,
                IsInstance = true,
                IsShared = false
            },
            new ParameterDescriptor
            {
                Name = "Space_Area",
                StorageType = StorageType.Double,
                IsInstance = true,
                IsShared = false
            },
            new ParameterDescriptor
            {
                Name = "Space_Area",
                StorageType = StorageType.Double,
                IsInstance = true,
                IsShared = false
            },
            new ParameterDescriptor
            {
                Name = "Space_Area",
                StorageType = StorageType.Double,
                IsInstance = true,
                IsShared = false
            },
            new ParameterDescriptor
            {
                Name = "Space_Area",
                StorageType = StorageType.Double,
                IsInstance = true,
                IsShared = false
            },
            new ParameterDescriptor
            {
                Name = "Space_Area",
                StorageType = StorageType.Double,
                IsInstance = true,
                IsShared = false
            },
            new ParameterDescriptor
            {
                Name = "Space_Area",
                StorageType = StorageType.Double,
                IsInstance = true,
                IsShared = false
            }
        };
    }
}
