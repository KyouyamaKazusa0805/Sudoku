namespace Sudoku.Concepts.Collections.Serialization;

/// <summary>
/// Defines a serialization converter to convert the <see cref="Candidates"/>
/// to a JSON <see cref="string"/> or vice versa.
/// </summary>
[JsonConverter(typeof(Candidates))]
public sealed class CandidatesConverter : JsonConverter<Candidates>
{
	/// <inheritdoc/>
	public override Candidates Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (!CanConvert(typeToConvert))
		{
			return Candidates.Empty;
		}

		var result = Candidates.Empty;
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.String)
			{
				result.Add(Candidates.Parse(reader.GetString()!)[0]);
			}
		}

		return result;
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Candidates value, JsonSerializerOptions options)
	{
		writeCollection(writer, value);


		static void writeCollection(Utf8JsonWriter @this, in Candidates candidates)
		{
			@this.WriteStartArray();
			foreach (int candidate in candidates)
			{
				@this.WriteStringValue((Candidates.Empty + candidate).ToString());
			}
			@this.WriteEndArray();
		}
	}
}
