namespace Sudoku.Concepts.Solving.TechniqueStructures.ChainNodes;

partial class LockedCandidatesNode
{
	/// <summary>
	/// Defines a JSON converter that can serialize or deserialize between a JSON string text
	/// and a <see cref="LockedCandidatesNode"/> instance.
	/// </summary>
	public sealed class JsonConverter : JsonConverter<LockedCandidatesNode>
	{
		/// <inheritdoc/>
		public override LockedCandidatesNode? Read(
			ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
			writer.WriteString(nameof(Cells), RxCyNotation.ToCellsString(value.Cells));
			writer.WriteNumber(nameof(Digit), value.Digit + 1);
			writer.WriteEndObject();
		}
	}
}
