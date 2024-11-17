using iSukces.Templates.Builder.Common;

namespace iSukces.Templates.Builder.PrimitiveObsessionClasses;

public abstract class PrimitiveObsessionBase(string name, string wrappedType) : CodeWriterBase
{
    public static void WriteAll<T>(T[] infos, TextTransformation p)
        where T : PrimitiveObsessionBase
    {
        var back = infos.Select(a => a.Implement).ToArray();
        foreach (var info in infos)
        {
            info.Implement |= Config.IncludeFeatures;
            info.Implement &= ~Config.IgnoreFeatures;

            if ((info.Implement & (Features.RelativeOperators | Features.ComparableObject)) != 0)
                info.Implement |= Features.Comparable;
        }

        var usings = infos
            .SelectMany(a => a.GetUsingNamespaces())
            .Distinct()
            .Where(a => !Config.ImplicitUsings.Contains(a))
            .OrderBy(a => a).ToArray();
        foreach (var ns in usings)
            p.WriteLine($"using {ns};");
        p.WriteLine("");

        p.WriteLine($"namespace {Config.Namespace};");
        p.WriteLine("");

        for (var index = 0; index < infos.Length; index++)
        {
            var info = infos[index];
            info.WriteCode(p);
            info.Implement = back[index];
        }
    }

    protected virtual void AddFieldsAndProperties()
    {
    }

    private void AddJsonConverter()
    {
        if ((Implement & Features.NewtonsoftJsonSerializer) == 0) return;
        Open($"public sealed class {Name}JsonConverter : JsonConverter");
        {
            WriteLine("public override bool CanConvert(Type objectType) => objectType == typeof(" + Name + ");");
            WriteLine();
            Open("public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)");

            WriteCaseExpressionStatement("return reader.Value switch", GetJsonConverterReadItems(), ";");
            Close(true);
            Open("public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)");
            {
                SingleLineIf("value is null",
                    "throw new NullReferenceException(\"value is null\");");
                WriteLine($"writer.WriteValue((({Name})value).Value);");
            }
            Close(true);
        }
        Close(true);
    }

    protected void BoolOperator(string op, string expr)
    {
        WriteLine($"public static bool operator {op}({Name} left, {Name} right) => {expr};").WriteLine();
    }

    protected virtual string GetEqualsExpression(string a, string b)
    {
        return $"{a}.Equals({b})";
    }

    private IEnumerable<string> GetInterfaces()
    {
        if ((Implement & Features.PrimitiveWrapper) != 0)
            yield return $"IPrimitiveWrapper<{WrappedType}>";
        if ((Implement & Features.EquatablePrimitive) != 0)
            yield return $"IEquatable<{WrappedType}>";
        yield return $"IEquatable<{Name}>";
        if ((Implement & Features.ComparablePrimitive) != 0)
            yield return $"{nameof(IComparable)}<{WrappedType}>";
        if ((Implement & Features.Comparable) != 0)
            yield return $"IComparable<{Name}>";
        if ((Implement & Features.ComparableObject) != 0)
            yield return $"IComparable";
    }


    protected abstract IEnumerable<CaseExpressionItem> GetJsonConverterReadItems();

    protected virtual IEnumerable<string> GetUsingNamespaces()
    {
        yield return "System";
        if ((Implement & Features.PrimitiveWrapper) != 0)
            yield return "iSukces.Base";
        if ((Implement & Features.NewtonsoftJsonSerializer) != 0)
            yield return "Newtonsoft.Json";
    }

    protected virtual string PrepareArgument(string variableName)
    {
        return variableName;
    }

    private void WriteAttributes()
    {
        if ((Implement & Features.NewtonsoftJsonSerializer) != 0)
            WriteLine($"[JsonConverter(typeof({Name}JsonConverter))]");
    }


    private void WriteCode(TextTransformation output)
    {
        Output = output;
        WriteAttributes();
        var record      = UseRecordStruct ? " record" : "";
        var constructor = UseRecordStruct ? $"({WrappedTypeRefNullable} Value)" : "";
        var part        = IsPartial ? " partial" : "";
        Open($"public readonly{record}{part} struct {Name}{constructor}: {Interfaces}");
        WriteCodeInternal();

        if (AllowWriteFromNullable)
            WriteLine($"public static {Name}? FromNullable({WrappedType}? value)")
                .IncIndent()
                .WriteLine($"=> value is null ? null : new {Name}(value.Value);")
                .DecIndent()
                .WriteLine();
        WriteToString();
        WriteTypeConversion();
        WriteRelativeOperators();
        WriteEqualityOperators();
        AddFieldsAndProperties();
        Close(true);
        AddJsonConverter();
    }

    protected abstract void WriteCodeInternal();

    private void WriteEqualityOperators()
    {
        if (UseRecordStruct || (Implement & Features.EqualityOperators) == 0)
            return;
        BoolOperator("==", "left.Equals(right)");
        BoolOperator("!=", "!left.Equals(right)");
    }

    protected void WriteIComparableAndEquatable(Func<string, string, string>? convertToComparable = null)
    {
        var arg = PrepareArgument("other");
        convertToComparable ??= (a, b) => $"{a}.CompareTo({b})";
        if ((Implement & Features.EquatablePrimitive) != 0)
            WriteLine($"public bool Equals({WrappedTypeRefNullable} other) => {GetEqualsExpression("Value", arg)};")
                .WriteLine();
        if ((Implement & Features.Comparable) != 0)
        {
            var c1 = convertToComparable("Value", "other.Value");
            WriteLine($"public int CompareTo({Name} other) => {c1};").WriteLine();
        }
        if ((Implement & Features.ComparableObject) != 0)
        {
            //var c1 = convertToComparable("Value", arg);
            Open("public int CompareTo(object? obj)")
                .WriteLine("if (obj is null) return 1;")
                .WriteLine("return obj is XPackageVersion other")
                .IncIndent()
                .WriteLine("? CompareTo(other)")
                .WriteLine($": throw new ArgumentException($\"Object must be of type {{nameof({Name})}}\");")
                .DecIndent()
                .Close(true);
        }

        if ((Implement & Features.ComparablePrimitive) != 0)
        {
            var c1 = convertToComparable("Value", arg);
            WriteLine($"public int CompareTo({WrappedTypeRefNullable} other) => {c1};")
                .WriteLine();
        }
    }

    private void WriteRelativeOperators()
    {
        if ((Implement & Features.RelativeOperators) == 0) return;

        foreach (var op in "> < >= <=".Split(' '))
            BoolOperator(op, GetRelativeOperatorCode(op, "left", "right"));
    }

    protected virtual string GetRelativeOperatorCode(string op, string left, string right)
    {
        return $"{left}.CompareTo({right}) {op} 0";
    }

    private void WriteToString()
    {
        if (string.IsNullOrEmpty(Config.ToStringExpression)) return;
        WriteLine($"public override string ToString() => {Config.ToStringExpression};")
            .WriteLine();
    }

    private void WriteTypeConversion()
    {
        Write(ConvertFromPrimitive, Name, WrappedTypeRefNullable, $"new {Name}(value)");
        Write(ConvertToPrimitive, WrappedType, Name, "value.Value");
        return;

        void Write(TypeConversion c, string result, string arg, string expression)
        {
            if (c == TypeConversion.None) return;

            WriteLine(
                    $"public static {ConvertToPrimitive.ToString().ToLower()} operator {result}({arg} value) => {expression};")
                .WriteLine();
        }
    }

    protected bool UseRecordStruct           { get; set; } = true;
    protected bool AllowWriteFromNullable    { get; set; } = true;
    protected bool HasReferenceNullableValue { get; set; }
    public    bool IsPartial                 { get; set; }

    public TypeConversion ConvertToPrimitive   { get; set; } = Config.ConvertToPrimitive;
    public TypeConversion ConvertFromPrimitive { get; set; } = Config.ConvertFromPrimitive;

    public string Name { get; } = name;

    public string Interfaces
        => string.Join(", ", GetInterfaces().OrderBy(a => a).Distinct());

    public string WrappedType { get; } = wrappedType;

    public string WrappedTypeRefNullable => HasReferenceNullableValue ? $"{WrappedType}?" : WrappedType;

    public Features Implement { get; set; }
}