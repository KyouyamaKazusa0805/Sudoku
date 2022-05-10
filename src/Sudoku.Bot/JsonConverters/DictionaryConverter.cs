namespace Sudoku.Bot.JsonConverters;

internal sealed class DictionaryConverter : JsonConverter<Dictionary<string, string>>
{
	/// <inheritdoc/>
	public override Dictionary<string, string> Read(
		ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var result = new Dictionary<string, string>();
		var jsonElement = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
		foreach (var element in jsonElement.EnumerateObject())
		{
			result.Add(element.Name, element.Value.ToString());
		}

		return result;
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Dictionary<string, string> value, JsonSerializerOptions options)
	{
		foreach (var (name, correspondingValue) in value)
		{
			writer.WriteString(name, correspondingValue);
		}
	}
}
