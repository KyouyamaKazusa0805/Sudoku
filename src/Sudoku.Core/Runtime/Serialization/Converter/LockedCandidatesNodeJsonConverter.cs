namespace Sudoku.Runtime.Serialization.Converter;

/// <summary>
/// Defines a JSON converter that is used for the serialization and deserialization
/// on type <see cref="LockedCandidatesNode"/>.
/// </summary>
/// <remarks>
/// JSON Pattern:
/// <code>
/// {
///   "cells": [
///     10,
///     20,
///     30
///   ],
///   "digit": 2
/// }
/// </code>
/// </remarks>
/// <seealso cref="LockedCandidatesNode"/>
public sealed class LockedCandidatesNodeJsonConverter : JsonConverter<LockedCandidatesNode>
{
	/// <inheritdoc/>
	public override LockedCandidatesNode? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
		{
			throw new JsonException();
		}

		if (!reader.Read() || reader.TokenType != JsonTokenType.String)
		{
			throw new JsonException();
		}

		var cells = RxCyNotation.ParseCells(reader.GetString()!);
		if (!reader.Read() || reader.TokenType != JsonTokenType.Number)
		{
			throw new JsonException();
		}

		byte digit = reader.GetByte();
		if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
		{
			throw new JsonException();
		}

		return new((byte)(digit - 1), cells);
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, LockedCandidatesNode value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteString(nameof(LockedCandidatesNode.Cells), RxCyNotation.ToCellsString(value.Cells));
		writer.WriteNumber(nameof(LockedCandidatesNode.Digit), value.Digit + 1);
		writer.WriteEndObject();
	}
}
