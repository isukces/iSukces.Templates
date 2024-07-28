using iSukces.Templates.Builder.Common;

namespace iSukces.Templates.Builder.PrimitiveObsessionClasses;

public class IntKey(string name) : PrimitiveObsessionBase(name, "int")
{
    protected override IEnumerable<CaseExpressionItem> GetJsonConverterReadItems()
    {
        yield return new CaseExpressionItem("int intValue", $"new {Name}(intValue)");
        yield return new CaseExpressionItem("long longValue", $"new {Name}(({Type})longValue)");
        yield return new CaseExpressionItem($"null when objectType == typeof({Name}?)", "null");
        yield return new CaseExpressionItem("null", "throw new NotImplementedException()");
        yield return new CaseExpressionItem("_", "throw new NotImplementedException()");
    }
 
    protected override void WriteCodeInternal()
    {
        WriteIComparableAndEquatable();
        WriteParse();
    }
    
    protected override IEnumerable<string> GetUsingNamespaces()
    {
        foreach (var ns in base.GetUsingNamespaces())
            yield return ns;
        if ((Implement & (Features.NewtonsoftJsonSerializer | Features.Parse) ) != 0)
            yield return "System.Globalization";
    }


    private void WriteParse()
    {
        if ((Implement & Features.Parse) == 0) return;
        Open($"public static {Name} Parse(string? text)");
        WriteLine("text = text?.Trim();");
        CheckArgumentNullOrEmpty("text");
        WriteLine($"return new {Name}({Type}.Parse(text, CultureInfo.InvariantCulture));");
        Close(true);
    }
}