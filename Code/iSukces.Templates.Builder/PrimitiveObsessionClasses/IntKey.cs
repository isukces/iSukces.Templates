using iSukces.Templates.Builder.Common;

namespace iSukces.Templates.Builder.PrimitiveObsessionClasses;

public class IntKey(string name) : PrimitiveObsessionBase(name, "int")
{
    protected override IEnumerable<CaseExpressionItem> GetJsonConverterReadItems()
    {
        var parse = (Implement & Features.Parse) != 0
            ? $"{Name}.Parse(stringValue)"
            : $"new {Name}({WrappedType}.Parse(stringValue.Trim()), CultureInfo.InvariantCulture)";
        yield return new CaseExpressionItem("string stringValue", parse);
        yield return new CaseExpressionItem("int intValue", $"new {Name}(intValue)");
        yield return new CaseExpressionItem("long longValue", $"new {Name}(({WrappedType})longValue)");
        yield return new CaseExpressionItem($"null when objectType == typeof({Name}?)", "null");
        yield return new CaseExpressionItem("null", "throw new NotImplementedException()");
        yield return new CaseExpressionItem("_", "throw new NotImplementedException()");
    }
    
    protected override string GetRelativeOperatorCode(string op, string left, string right)
    {
        return $"{left}.Value {op} {right}.Value";
    }

    protected override string GetEqualsExpression(string a, string b) => $"{a} == {b}";
    
    protected override IEnumerable<string> GetUsingNamespaces()
    {
        foreach (var ns in base.GetUsingNamespaces())
            yield return ns;
        if ((Implement & (Features.NewtonsoftJsonSerializer | Features.Parse)) != 0)
            yield return "System.Globalization";
    }

    protected override void WriteCodeInternal()
    {
        WriteIComparableAndEquatable();
        WriteParse();
    }

    private void WriteParse()
    {
        if ((Implement & Features.Parse) == 0) return;
        Open($"public static {Name} Parse(string? text)");
        WriteLine("text = text?.Trim();");
        CheckArgumentNullOrEmpty("text");
        WriteLine($"return new {Name}({WrappedType}.Parse(text, CultureInfo.InvariantCulture));");
        Close(true);
    }
}