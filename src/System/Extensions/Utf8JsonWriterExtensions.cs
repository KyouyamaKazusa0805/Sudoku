using System.Numerics;

namespace System.Text.Json;

/// <summary>
/// Provides extension methods on <see cref="Utf8JsonWriter"/>.
/// </summary>
/// <seealso cref="Utf8JsonWriter"/>
public static class Utf8JsonWriterExtensions
{
	/// <summary>
	/// Writes the bit collection of type <see cref="byte"/> into the JSON stream.
	/// </summary>
	/// <param name="this">The writer.</param>
	/// <param name="propertyName">The property name.</param>
	/// <param name="bits">The bits to write.</param>
	public static void WriteBitCollection(this Utf8JsonWriter @this, string propertyName, byte bits)
	{
		@this.WritePropertyName(propertyName);
		@this.WriteStartArray();
		foreach (int bitPos in bits)
		{
			@this.WriteNumberValue(bitPos);
		}
		@this.WriteEndArray();
	}

	/// <summary>
	/// Writes the bit collection of type <see cref="sbyte"/> into the JSON stream.
	/// </summary>
	/// <param name="this">The writer.</param>
	/// <param name="propertyName">The property name.</param>
	/// <param name="bits">The bits to write.</param>
	public static void WriteBitCollection(this Utf8JsonWriter @this, string propertyName, sbyte bits)
	{
		@this.WritePropertyName(propertyName);
		@this.WriteStartArray();
		foreach (int bitPos in bits)
		{
			@this.WriteNumberValue(bitPos);
		}
		@this.WriteEndArray();
	}

	/// <summary>
	/// Writes the bit collection of type <see cref="ushort"/> into the JSON stream.
	/// </summary>
	/// <param name="this">The writer.</param>
	/// <param name="propertyName">The property name.</param>
	/// <param name="bits">The bits to write.</param>
	public static void WriteBitCollection(this Utf8JsonWriter @this, string propertyName, ushort bits)
	{
		@this.WritePropertyName(propertyName);
		@this.WriteStartArray();
		foreach (int bitPos in bits)
		{
			@this.WriteNumberValue(bitPos);
		}
		@this.WriteEndArray();
	}

	/// <summary>
	/// Writes the bit collection of type <see cref="short"/> into the JSON stream.
	/// </summary>
	/// <param name="this">The writer.</param>
	/// <param name="propertyName">The property name.</param>
	/// <param name="bits">The bits to write.</param>
	public static void WriteBitCollection(this Utf8JsonWriter @this, string propertyName, short bits)
	{
		@this.WritePropertyName(propertyName);
		@this.WriteStartArray();
		foreach (int bitPos in bits)
		{
			@this.WriteNumberValue(bitPos);
		}
		@this.WriteEndArray();
	}

	/// <summary>
	/// Writes the bit collection of type <see cref="uint"/> into the JSON stream.
	/// </summary>
	/// <param name="this">The writer.</param>
	/// <param name="propertyName">The property name.</param>
	/// <param name="bits">The bits to write.</param>
	public static void WriteBitCollection(this Utf8JsonWriter @this, string propertyName, uint bits)
	{
		@this.WritePropertyName(propertyName);
		@this.WriteStartArray();
		foreach (int bitPos in bits)
		{
			@this.WriteNumberValue(bitPos);
		}
		@this.WriteEndArray();
	}

	/// <summary>
	/// Writes the bit collection of type <see cref="int"/> into the JSON stream.
	/// </summary>
	/// <param name="this">The writer.</param>
	/// <param name="propertyName">The property name.</param>
	/// <param name="bits">The bits to write.</param>
	public static void WriteBitCollection(this Utf8JsonWriter @this, string propertyName, int bits)
	{
		@this.WritePropertyName(propertyName);
		@this.WriteStartArray();
		foreach (int bitPos in bits)
		{
			@this.WriteNumberValue(bitPos);
		}
		@this.WriteEndArray();
	}

	/// <summary>
	/// Writes the bit collection of type <see cref="ulong"/> into the JSON stream.
	/// </summary>
	/// <param name="this">The writer.</param>
	/// <param name="propertyName">The property name.</param>
	/// <param name="bits">The bits to write.</param>
	public static void WriteBitCollection(this Utf8JsonWriter @this, string propertyName, ulong bits)
	{
		@this.WritePropertyName(propertyName);
		@this.WriteStartArray();
		foreach (int bitPos in bits)
		{
			@this.WriteNumberValue(bitPos);
		}
		@this.WriteEndArray();
	}

	/// <summary>
	/// Writes the bit collection of type <see cref="long"/> into the JSON stream.
	/// </summary>
	/// <param name="this">The writer.</param>
	/// <param name="propertyName">The property name.</param>
	/// <param name="bits">The bits to write.</param>
	public static void WriteBitCollection(this Utf8JsonWriter @this, string propertyName, long bits)
	{
		@this.WritePropertyName(propertyName);
		@this.WriteStartArray();
		foreach (int bitPos in bits)
		{
			@this.WriteNumberValue(bitPos);
		}
		@this.WriteEndArray();
	}

	/// <summary>
	/// Try to write an object.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="this">The instance.</param>
	/// <param name="value">The value to serialize.</param>
	/// <param name="converter">The converter.</param>
	/// <param name="options">The options on serialization.</param>
	public static void WriteObject<T>(
		this Utf8JsonWriter @this,
		T value,
		JsonConverter<T>? converter,
		JsonSerializerOptions options
	)
	{
		if (converter is not null)
		{
			converter.Write(@this, value, options);
		}
		else
		{
			JsonSerializer.Serialize(@this, value, options);
		}
	}
}
