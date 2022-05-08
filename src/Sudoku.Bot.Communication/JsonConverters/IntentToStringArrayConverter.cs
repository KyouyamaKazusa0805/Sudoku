namespace Sudoku.Bot.Communication.JsonConverters;

/// <summary>
/// Defines a JSON converter that allows the conversion between <see cref="Intent"/> and <see cref="string"/>[].
/// </summary>
public sealed class IntentToStringArrayConverter : JsonConverter<Intent>
{
	/// <inheritdoc/>
	public override Intent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> (
			JsonSerializer.Deserialize<List<string>>(ref reader, options) is { } collection
				? from element in collection select Enum.Parse<Intent>(element)
				: null
		)?.Aggregate(static (interim, value) => interim | value) ?? Intents.PublicDomain;

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Intent value, JsonSerializerOptions options)
		=> JsonSerializer.Serialize(writer, from f in value.ToString().Split(',') select f.Trim(), options);
}
