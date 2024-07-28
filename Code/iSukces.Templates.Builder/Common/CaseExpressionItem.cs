namespace iSukces.Templates.Builder.Common;

public sealed class CaseExpressionItem(string condition, string value)
{
    public string Condition { get; } = condition;
    public string Value     { get; } = value;
}