namespace Sudoku.Concepts.Solving.TechniqueStructures;

partial class AlternatingInferenceChain
{
	/// <summary>
	/// Defines a type that is used as a JSON converter that can convert a <see cref="string"/> into
	/// an <see cref="AlternatingInferenceChain"/> instance, or vice versa.
	/// </summary>
	public sealed class JsonConverter : JsonConverter<AlternatingInferenceChain>
	{
		/// <inheritdoc/>
		public override AlternatingInferenceChain? Read(
			ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
			{
				throw new JsonException();
			}

			if (!reader.Read() || reader.TokenType is not (JsonTokenType.True or JsonTokenType.False))
			{
				throw new JsonException();
			}

			bool isStrong = reader.TokenType == JsonTokenType.True;
			if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
			{
				throw new JsonException();
			}

			var nodes = new List<Node>();
			while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
			{
				nodes.Add(JsonSerializer.Deserialize<Node>(ref reader, options)!);
			}

			if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
			{
				throw new JsonException();
			}

			return new(nodes.ToArray(), isStrong);
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, AlternatingInferenceChain value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteBoolean(nameof(IsStrong), value.IsStrong);
			writer.WritePropertyName(nameof(Chain));
			writer.WriteStartArray();
			foreach (var node in value._nodes)
			{
				JsonSerializer.Serialize(writer, node.GetType(), options);
			}
			writer.WriteEndArray();
			writer.WriteEndObject();
		}
	}
}
