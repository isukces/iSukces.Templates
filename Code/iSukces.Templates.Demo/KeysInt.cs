﻿// This file was generated by iSukces.Templates ver 1.24.1229.5
// Get latest version from https://raw.githubusercontent.com/isukces/iSukces.Templates/main/Code/iSukces.Templates/PrimitiveObsession.ttinclude

using iSukces.Base;
using Newtonsoft.Json;
using System.Globalization;

namespace iSukces.Templates.Demo;

[JsonConverter(typeof(PrimaryKeyJsonConverter))]
public readonly record struct PrimaryKey(int Value): IComparable, IComparable<int>, IComparable<PrimaryKey>, IEquatable<int>, IEquatable<PrimaryKey>, IPrimitiveWrapper<int>
{
    public bool Equals(int other) => Value == other;

    public int CompareTo(PrimaryKey other) => Value.CompareTo(other.Value);

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        return obj is PrimaryKey other
            ? CompareTo(other)
            : throw new ArgumentException($"Object must be of type {nameof(PrimaryKey)}");
    }

    public int CompareTo(int other) => Value.CompareTo(other);

    public static PrimaryKey Parse(string? text)
    {
        text = text?.Trim();
        if (string.IsNullOrEmpty(text))
            throw new ArgumentException("", nameof(text));
        return new PrimaryKey(int.Parse(text, CultureInfo.InvariantCulture));
    }

    public static PrimaryKey? FromNullable(int? value)
        => value is null ? null : new PrimaryKey(value.Value);

    public static implicit operator PrimaryKey(int value) => new PrimaryKey(value);

    public static implicit operator int(PrimaryKey value) => value.Value;

    public static bool operator >(PrimaryKey left, PrimaryKey right) => left.Value > right.Value;

    public static bool operator <(PrimaryKey left, PrimaryKey right) => left.Value < right.Value;

    public static bool operator >=(PrimaryKey left, PrimaryKey right) => left.Value >= right.Value;

    public static bool operator <=(PrimaryKey left, PrimaryKey right) => left.Value <= right.Value;

}

public sealed class PrimaryKeyJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType) => objectType == typeof(PrimaryKey);

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        return reader.Value switch
        {
            string stringValue => PrimaryKey.Parse(stringValue),
            int intValue => new PrimaryKey(intValue),
            long longValue => new PrimaryKey((int)longValue),
            null when objectType == typeof(PrimaryKey?) => null,
            null => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        };
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is null)
            throw new NullReferenceException("value is null");
        writer.WriteValue(((PrimaryKey)value).Value);
    }

}

public sealed class PrimaryKeySystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<PrimaryKey>
{
    public override PrimaryKey Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
    {
        return new PrimaryKey(reader.GetInt32());
    }

    public override void Write(System.Text.Json.Utf8JsonWriter writer, PrimaryKey value, System.Text.Json.JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }

}

