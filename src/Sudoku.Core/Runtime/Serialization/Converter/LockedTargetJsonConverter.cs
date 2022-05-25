namespace Sudoku.Runtime.Serialization.Converter;

/// <summary>
/// Defines a JSON converter that is used for the serialization and deserialization on type <see cref="LockedTarget"/>.
/// </summary>
/// <remarks>
/// JSON Pattern:
/// <code>
/// {
///   "digit": 1,
///   "cells": [
///     10,
///     20,
///     30
///   ]
/// }
/// </code>
/// </remarks>
/// <seealso cref="LockedTarget"/>
public sealed class LockedTargetJsonConverter : JsonConverter<LockedTarget>
{
	/// <inheritdoc/>
	public override LockedTarget Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
		{
			throw new JsonException();
		}

		if (!reader.Read()
			|| reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != nameof(LockedTarget.Digit))
		{
			throw new JsonException();
		}

		if (!reader.Read() || reader.TokenType != JsonTokenType.Number)
		{
			throw new JsonException();
		}

		if (!reader.Read()
			|| reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != nameof(LockedTarget.Cells))
		{
			throw new JsonException();
		}

		int digit = reader.GetInt32();
		if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
		{
			throw new JsonException();
		}

		var cells = Cells.Empty;
		while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
		{
			int cell = reader.GetInt32();
			cells.Add(cell);
		}

		if (reader.TokenType != JsonTokenType.EndObject)
		{
			throw new JsonException();
		}

		return new(digit, cells);
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, LockedTarget value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteNumber(nameof(LockedTarget.Digit), value.Digit);
		writer.WritePropertyName(nameof(LockedTarget.Cells));
		writer.WriteStartArray();
		foreach (int cell in value.Cells)
		{
			writer.WriteNumberValue(cell);
		}
		writer.WriteEndArray();
		writer.WriteEndObject();
	}
}
