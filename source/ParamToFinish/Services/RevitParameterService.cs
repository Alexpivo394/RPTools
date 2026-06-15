namespace ParamToFinish.Services;

public sealed class RevitParameterService
{
    public string? GetParameterValue(
        Element element,
        string? parameterName)
    {
        var parameter = GetParameter(
            element,
            parameterName);

        return parameter?.AsString();
    }

    public void SetParameterValue(
        Element element,
        string? parameterName,
        string value)
    {
        var parameter = GetParameter(
            element,
            parameterName);

        if (parameter == null)
            return;

        if (parameter.IsReadOnly)
            return;

        parameter.Set(value);
    }

    private static Parameter? GetParameter(
        Element element,
        string? parameterName)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
            return null;

        var parameter = element.LookupParameter(parameterName);

        if (parameter != null)
            return parameter;

        var type = element.Document.GetElement(
            element.GetTypeId());

        return type?.LookupParameter(parameterName);
    }
}