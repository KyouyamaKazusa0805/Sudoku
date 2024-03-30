namespace System.Text.Json;

/// <summary>
/// Represents a JSON converter that serializes and deserializes a <see cref="Range"/> object.
/// </summary>
/// <seealso cref="Range"/>
public sealed class RangeConverter : JsonConverter<Range>
{
	/// <inheritdoc/>
	public override Range Read(scoped ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.GetString() is not string s || !s.Contains(".."))
		{
			throw new JsonException();
		}

		var pair = s.Split("..", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (pair is not [var left, var right])
		{
			throw new JsonException();
		}

		return new(getIndex(left), getIndex(right));


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static Index getIndex(string s) => s.StartsWith('^') ? ^int.Parse(s[1..]) : int.Parse(s);
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Range value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString());
}
