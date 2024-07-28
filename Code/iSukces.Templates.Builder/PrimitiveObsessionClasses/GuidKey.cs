using iSukces.Templates.Builder.Common;

namespace iSukces.Templates.Builder.PrimitiveObsessionClasses;

public class GuidKey(string name) : PrimitiveObsessionBase(name, "Guid")
{
    protected override IEnumerable<CaseExpressionItem> GetJsonConverterReadItems()
    {
        var parse = (Implement & Features.Parse) != 0
            ? $"{Name}.Parse(stringValue)"
            : $"new {Name}({Type}.Parse(stringValue.Trim()))";
        yield return new CaseExpressionItem("string stringValue", parse);
        yield return new CaseExpressionItem($"null when objectType == typeof({Name}?)", "null");
        yield return new CaseExpressionItem("null", "throw new NotImplementedException()");
        yield return new CaseExpressionItem("_", "throw new NotImplementedException()");
    }


    protected override void WriteCodeInternal()
    {
        WriteIComparableAndEquatable();
        WriteParse();
        WriteLine($"public static {Name} NewUid() => new {Name}(Guid.NewGuid());");
        WriteLine();
        WriteLine($"public static {Name} Empty {{ get; }} = new {Name}(Guid.Empty);");
        WriteLine();
    }

    private void WriteParse()
    {
        if ((Implement & Features.Parse) == 0) return;
        Open($"public static {Name} Parse(string? text)");
        WriteLine("text = text?.Trim();");
        CheckArgumentNullOrEmpty("text");
        WriteLine($"return new {Name}({Type}.Parse(text));");
        Close(true);
    }
}