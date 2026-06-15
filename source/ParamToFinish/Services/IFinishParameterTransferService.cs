using ParamToFinish.Models;

namespace ParamToFinish.Services;

public interface IFinishParameterTransferService
{
    void Transfer(
        ParameterDescriptor? selectedWallParameter,
        ParameterDescriptor? selectedFinishParameter, bool allModel, string filter);
}