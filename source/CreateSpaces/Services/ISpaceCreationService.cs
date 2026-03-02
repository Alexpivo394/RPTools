using CreateSpaces.Models;
using CreateSpaces.ViewModels;

namespace CreateSpaces.Services;

public interface ISpaceCreationService
{
    SpaceCreationResult CreateSpaces(
        LinkDescriptor? linkDescriptor,
        IEnumerable<ParameterMappingModel> mappings,
        bool createSpaces);
}