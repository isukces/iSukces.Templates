// This file was generated by iSukces.Templates ver 1.24.821.2
// Get latest version from https://raw.githubusercontent.com/isukces/iSukces.Templates/main/Code/iSukces.Templates/PrimitiveObsession.ttinclude

using iSukces.Base;
using Newtonsoft.Json;

namespace iSukces.Templates.Demo;

[JsonConverter(typeof(MyIdentifierJsonConverter))]
public readonly partial struct MyIdentifier: IComparable<MyIdentifier>, IComparable<string>, IEquatable<string>, IPrimitiveWrapper<string>
{
    public MyIdentifier(string? value) => _value = value?.Trim();

    public override bool Equals(object? obj) => obj is MyIdentifier x && Equals(x);

    public bool Equals(MyIdentifier other) => Comparer.Equals(Value, other.Value);

    public override int GetHashCode() => Comparer.GetHashCode(Value);

    public bool Equals(string? other) => Comparer.Equals(Value, other ?? string.Empty);

    public int CompareTo(MyIdentifier other) => Comparer.Compare(Value, other.Value);

    public int CompareTo(string? other) => Comparer.Compare(Value, other ?? string.Empty);

    public override string ToString() => Value.Trim();

    public static implicit operator MyIdentifier(string? value) => new MyIdentifier(value);

    public static implicit operator string(MyIdentifier value) => value.Value;

    public static bool operator >(MyIdentifier left, MyIdentifier right) => left.CompareTo(right) > 0;

    public static bool operator <(MyIdentifier left, MyIdentifier right) => left.CompareTo(right) < 0;

    public static bool operator >=(MyIdentifier left, MyIdentifier right) => left.CompareTo(right) >= 0;

    public static bool operator <=(MyIdentifier left, MyIdentifier right) => left.CompareTo(right) <= 0;

    private readonly string? _value;

    public string Value => _value ?? string.Empty;

    public static MyIdentifier Empty { get; } = new MyIdentifier(null);

    public bool IsEmpty => string.IsNullOrEmpty(_value);

    public static StringComparer Comparer => StringComparer.OrdinalIgnoreCase;

}

public sealed class MyIdentifierJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType) => objectType == typeof(MyIdentifier);

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        return reader.Value switch
        {
            string stringValue => new MyIdentifier(stringValue),
            null => MyIdentifier.Empty,
            _ => throw new NotImplementedException()
        };
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is null)
            throw new NullReferenceException("value is null");
        writer.WriteValue(((MyIdentifier)value).Value);
    }

}

