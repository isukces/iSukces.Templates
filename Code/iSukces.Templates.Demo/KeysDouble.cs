﻿// This file was generated by iSukces.Templates ver 1.24.1229.5
// Get latest version from https://raw.githubusercontent.com/isukces/iSukces.Templates/main/Code/iSukces.Templates/PrimitiveObsession.ttinclude

using iSukces.Base;
using Newtonsoft.Json;
using System.Globalization;

namespace iSukces.Templates.Demo;

[JsonConverter(typeof(TemperatureJsonConverter))]
public readonly record struct Temperature(double Value): IComparable, IComparable<double>, IComparable<Temperature>, IEquatable<double>, IEquatable<Temperature>, IPrimitiveWrapper<double>
{
    public bool Equals(double other) => Value.Equals(other);

    public int CompareTo(Temperature other) => Value.CompareTo(other.Value);

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        return obj is Temperature other
            ? CompareTo(other)
            : throw new ArgumentException($"Object must be of type {nameof(Temperature)}");
    }

    public int CompareTo(double other) => Value.CompareTo(other);

    public static Temperature Parse(string? text)
    {
        text = text?.Trim();
        if (string.IsNullOrEmpty(text))
            throw new ArgumentException("", nameof(text));
        return new Temperature(double.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture));
    }

    public static Temperature? FromNullable(double? value)
        => value is null ? null : new Temperature(value.Value);

    public static implicit operator Temperature(double value) => new Temperature(value);

    public static implicit operator double(Temperature value) => value.Value;

    public static bool operator >(Temperature left, Temperature right) => left.CompareTo(right) > 0;

    public static bool operator <(Temperature left, Temperature right) => left.CompareTo(right) < 0;

    public static bool operator >=(Temperature left, Temperature right) => left.CompareTo(right) >= 0;

    public static bool operator <=(Temperature left, Temperature right) => left.CompareTo(right) <= 0;

    public static Temperature Empty { get; } = new Temperature(double.NaN);

    public bool IsEmpty => double.IsNaN(Value);

}

public sealed class TemperatureJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType) => objectType == typeof(Temperature);

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        return reader.Value switch
        {
            decimal decimalValue => new Temperature((double)decimalValue),
            double doubleValue => new Temperature(doubleValue),
            long longValue => new Temperature(longValue),
            string stringValue => Temperature.Parse(stringValue),
            null when objectType == typeof(Temperature?) => null,
            null => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        };
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is null)
            throw new NullReferenceException("value is null");
        writer.WriteValue(((Temperature)value).Value);
    }

}

