using iSukces.Templates.Builder.Common;

namespace iSukces.Templates.Builder.PrimitiveObsessionClasses;

public class DoubleKey(string name) : PrimitiveObsessionBase(name, "double")
{
    protected override void AddFieldsAndProperties()
    {
        base.AddFieldsAndProperties();
        WriteLine($"public static {Name} Empty {{ get; }} = new {Name}(double.NaN);");
        WriteLine();
        WriteLine("public bool IsEmpty => double.IsNaN(Value);");
        WriteLine();
    }

    protected override IEnumerable<CaseExpressionItem> GetJsonConverterReadItems()
    {
        var parse = (Implement & Features.Parse) != 0
            ? $"{Name}.Parse(stringValue)"
            : $"new {Name}({WrappedType}.Parse(stringValue.Trim()))";
        yield return new CaseExpressionItem("decimal decimalValue", $"new {Name}((double)decimalValue)");
        yield return new CaseExpressionItem("double doubleValue", $"new {Name}(doubleValue)");
        yield return new CaseExpressionItem("long longValue", $"new {Name}(longValue)");
        yield return new CaseExpressionItem("string stringValue", parse);
        yield return new CaseExpressionItem($"null when objectType == typeof({Name}?)", "null");
        yield return new CaseExpressionItem("null", "throw new NotImplementedException()");
        yield return new CaseExpressionItem("_", "throw new NotImplementedException()");
    }

    protected override IEnumerable<string> GetUsingNamespaces()
    {
        var a = base.GetUsingNamespaces().ToList();
        if ((Implement & Features.Parse) != 0)
            a.Add("System.Globalization");
        return a;
    }
    
    protected override void WriteCodeInternal()
    {
        WriteIComparableAndEquatable();
        WriteParse();
    }

    protected override void WriteSystemTextConverter(bool reader)
    {
        if (reader)
            WriteLine($"return {Name}.Parse(reader.GetString()!);");
        else
            WriteLine("writer.WriteStringValue(value.Value.ToString());");
    }

    private void WriteParse()
    {
        if ((Implement & Features.Parse) == 0) return;
        Open($"public static {Name} Parse(string? text)");
        WriteLine("text = text?.Trim();");
        CheckArgumentNullOrEmpty("text");
        WriteLine($"return new {Name}({WrappedType}.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture));");
        Close(true);
    }
}
