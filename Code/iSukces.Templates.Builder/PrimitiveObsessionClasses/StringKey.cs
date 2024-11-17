using iSukces.Templates.Builder.Common;

namespace iSukces.Templates.Builder.PrimitiveObsessionClasses;

public class StringKey : PrimitiveObsessionBase
{
    public StringKey(string name, StringComparison comparison = StringComparison.Ordinal) : base(name, "string")
    {
        UseRecordStruct           = false;
        AllowWriteFromNullable    = false;
        HasReferenceNullableValue = true;
        WithComparison(comparison);
    }

    protected override void AddFieldsAndProperties()
    {
        base.AddFieldsAndProperties();

        WriteLine($"private readonly {WrappedTypeRefNullable} _value;");
        WriteLine();

        WriteLine($"public {WrappedType} Value => _value ?? string.Empty;");
        WriteLine();
        
        WriteLine($"public static {Name} Empty {{ get; }} = new {Name}(null);");
        WriteLine();


        WriteLine("public bool IsEmpty => string.IsNullOrEmpty(_value);");
        WriteLine();

        WriteLine($"public static StringComparer {ComparerPropertyName} => {Comparer};");
        WriteLine();
    }

    protected override string GetEqualsExpression(string a, string b)
    {
        return $"{ComparerPropertyName}.Equals({a}, {b})";
    }

    protected override IEnumerable<CaseExpressionItem> GetJsonConverterReadItems()
    {
        yield return new CaseExpressionItem("string stringValue", $"new {Name}(stringValue)");
        yield return new CaseExpressionItem("null", $"{Name}.Empty");
        yield return new CaseExpressionItem("_", "throw new NotImplementedException()");
    }

    protected override string PrepareArgument(string variableName)
    {
        return $"{variableName} ?? string.Empty";
    }

    protected override void WriteCodeInternal()
    {
        var text = string.Format(StringPreprocess, "value");
        WriteLine($"public {Name}({WrappedTypeRefNullable} value) => _value = {text};");
        WriteLine();

        WriteLine($"public override bool Equals(object? obj) => obj is {Name} x && Equals(x);")
            .WriteLine();

        WriteLine($"public bool Equals({Name} other) => {GetEqualsExpression("Value", "other.Value")};")
            .WriteLine();


        WriteLine($"public override int GetHashCode() => {ComparerPropertyName}.GetHashCode(Value);")
            .WriteLine();

        WriteIComparableAndEquatable((a, b) => $"{ComparerPropertyName}.Compare({a}, {b})");
    }

    public string Comparer { get; set; } = "StringComparer.Ordinal";

    public string StringPreprocess { get; set; } = "{0}?.Trim()";

    private const string ComparerPropertyName = "Comparer";

    public StringKey WithComparison(StringComparison comparison)
    {
        Comparer = "StringComparer." + comparison;
        return this;
    }
}