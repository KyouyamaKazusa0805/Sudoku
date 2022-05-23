namespace Sudoku.Concepts.Solving.TechniqueStructures.ChainNodes;

partial class SoleCandidateNode
{
	/// <summary>
	/// Defines a JSON converter that can serialize or deserialize between a JSON string text
	/// and a <see cref="SoleCandidateNode"/> instance.
	/// </summary>
	public sealed class JsonConverter : JsonConverter<SoleCandidateNode>
	{
		/// <inheritdoc/>
		public override SoleCandidateNode? Read(
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

			if (!byte.TryParse(reader.GetString()!, out byte cell))
			{
				throw new JsonException();
			}

			if (!reader.Read() || reader.TokenType != JsonTokenType.Number)
			{
				throw new JsonException();
			}

			byte digit = reader.GetByte();
			if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
			{
				throw new JsonException();
			}

			return new(cell, (byte)(digit - 1));
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, SoleCandidateNode value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteString(nameof(Cell), RxCyNotation.ToCellString(value.Cell));
			writer.WriteNumber(nameof(Digit), value.Digit + 1);
			writer.WriteEndObject();
		}
	}
}
