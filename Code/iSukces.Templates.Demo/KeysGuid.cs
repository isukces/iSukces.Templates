// This file was generated by iSukces.Templates ver 1.24.821.2
// Get latest version from https://raw.githubusercontent.com/isukces/iSukces.Templates/main/Code/iSukces.Templates/PrimitiveObsession.ttinclude
using iSukces.Base;
using Newtonsoft.Json;

namespace iSukces.Templates.Demo;

[JsonConverter(typeof(MyUidJsonConverter))]
public readonly record struct MyUid(Guid Value): IComparable<Guid>, IComparable<MyUid>, IEquatable<Guid>, IPrimitiveWrapper<Guid>
{
    public bool Equals(Guid other) => Value.Equals(other);

    public int CompareTo(MyUid other) => Value.CompareTo(other.Value);

    public int CompareTo(Guid other) => Value.CompareTo(other);

    public static MyUid Parse(string? text)
    {
        text = text?.Trim();
        if (string.IsNullOrEmpty(text))
            throw new ArgumentException("", nameof(text));
        return new MyUid(Guid.Parse(text));
    }

    public static MyUid NewUid() => new MyUid(Guid.NewGuid());

    public static MyUid? FromNullable(Guid? value)
        => value is null ? null : new MyUid(value.Value);

    public static implicit operator MyUid(Guid value) => new MyUid(value);

    public static implicit operator Guid(MyUid value) => value.Value;

    public static bool operator >(MyUid left, MyUid right) => left.CompareTo(right) > 0;

    public static bool operator <(MyUid left, MyUid right) => left.CompareTo(right) < 0;

    public static bool operator >=(MyUid left, MyUid right) => left.CompareTo(right) >= 0;

    public static bool operator <=(MyUid left, MyUid right) => left.CompareTo(right) <= 0;

    public static MyUid Empty { get; } = new MyUid(Guid.Empty);

    public bool IsEmpty => Value.Equals(Guid.Empty);

}

public sealed class MyUidJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType) => objectType == typeof(MyUid);

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        return reader.Value switch
        {
            string stringValue => MyUid.Parse(stringValue),
            null when objectType == typeof(MyUid?) => null,
            null => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        };
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is null)
            throw new NullReferenceException("value is null");
        writer.WriteValue(((MyUid)value).Value);
    }

}




