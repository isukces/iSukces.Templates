namespace iSukces.Templates.Builder.MvvmClasses;

public sealed class Property(string type, string name, string initValueExpression)
{
    public string Type                { get; } = type;
    public string Name                { get; } = name;
    public string InitValueExpression { get; } = initValueExpression;
}
