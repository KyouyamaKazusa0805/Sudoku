namespace Sudoku.Concepts;

/// <summary>
/// Defines an inference.
/// </summary>
[JsonConverter(typeof(Converter))]
public enum Inference
{
	/// <summary>
	/// Indicates the inference is strong inference.
	/// </summary>
	Strong,

	/// <summary>
	/// Indicates the inference is weak inference.
	/// </summary>
	Weak,

	/// <summary>
	/// Indicates the inference is strong inference that is generalized.
	/// </summary>
	StrongGeneralized,

	/// <summary>
	/// Indicates the inference is weak inference that is generalized.
	/// </summary>
	WeakGeneralized,

	/// <summary>
	/// Indicates the inference is conjugate pair.
	/// </summary>
	ConjugatePair,

	/// <summary>
	/// Indicates the inference is the default case that doesn't belong to above.
	/// </summary>
	Default
}

/// <summary>
/// Indicates the JSON converter of the current type.
/// </summary>
file sealed class Converter : JsonConverter<Inference>
{
	/// <inheritdoc/>
	public override Inference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.GetString() is { } value ? Enum.Parse<Inference>(value) : throw new JsonException();

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Inference value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString());
}
