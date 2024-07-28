using iSukces.Templates.Builder.Common;

namespace iSukces.Templates.Builder.PrimitiveObsessionClasses;

public abstract class PrimitiveObsessionBase(string name, string type) : CodeWriterBase
{
    public static void WriteAll<T>(T[] infos, TextTransformation p)
        where T : PrimitiveObsessionBase
    {
        var back = infos.Select(a => a.Implement).ToArray();
        foreach (var info in infos)
        {
            info.Implement |= Config.IncludeFeatures;
            info.Implement &= ~Config.IgnoreFeatures;
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


    private void AddJsonConverter()
    {
        if ((Implement & Features.NewtonsoftJsonSerializer) == 0) return;
        Open($"public sealed class {Name}JsonConverter : JsonConverter");
        {
            WriteLine("public override bool CanConvert(Type objectType) => objectType == typeof(" + Name + ");");
            WriteLine();
            Open(
                "public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)");


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

    private IEnumerable<string> GetInterfaces()
    {
        if ((Implement & Features.PrimitiveWrapper) != 0)
            yield return $"IPrimitiveWrapper<{Type}>";
        if ((Implement & Features.EquatablePrimitive) != 0)
            yield return $"IEquatable<{Type}>";
        if ((Implement & Features.ComparablePrimitive) != 0)
            yield return $"{nameof(IComparable)}<{Type}>";
        if ((Implement & Features.Comparable) != 0)
            yield return $"IComparable<{Name}>";
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

    private void WriteAttributes()
    {
        if ((Implement & Features.NewtonsoftJsonSerializer) != 0)
            WriteLine($"[JsonConverter(typeof({Name}JsonConverter))]");
    }

    private void WriteCode(TextTransformation output)
    {
        Output = output;
        WriteAttributes();
        Open($"public readonly record struct {Name}({Type} Value): {Interfaces}");
        WriteCodeInternal();

        WriteLine($"public static {Name}? FromNullable({Type}? value)")
            .IncIndent()
            .WriteLine($"=> value is null ? null : new {Name}(value.Value);")
            .DecIndent()
            .WriteLine();


        Close(true);
        AddJsonConverter();
    }

    protected abstract void WriteCodeInternal();

    protected void WriteIComparableAndEquatable(Func<string, string, string>? convertToComparable = null)
    {
        convertToComparable ??= (a, b) => $"{a}.CompareTo({b})";
        if ((Implement & Features.ComparablePrimitive) != 0)
            WriteLine($"public bool Equals({Type} other) => Value.Equals(other);")
                .WriteLine();
        if ((Implement & Features.Comparable) != 0)
        {
            var c1 = convertToComparable("Value", "other.Value");
            WriteLine($"public int CompareTo({Name} other) => {c1};").WriteLine();
        }

        if ((Implement & Features.EquatablePrimitive) != 0)
            WriteLine($"public int CompareTo({Type} other) => Value.CompareTo(other);")
                .WriteLine();
    }

    public string Name { get; } = name;

    public string Interfaces
        => string.Join(", ", GetInterfaces().OrderBy(a => a).Distinct());

    public string   Type      { get; } = type;
    public Features Implement { get; set; }
}