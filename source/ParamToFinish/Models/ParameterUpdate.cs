namespace ParamToFinish.Models;

public readonly struct ParameterUpdate
{
    public ParameterUpdate(
        Element element,
        string parameterName,
        string value)
    {
        Element = element;
        ParameterName = parameterName;
        Value = value;
    }

    public Element Element { get; }

    public string ParameterName { get; }

    public string Value { get; }
}