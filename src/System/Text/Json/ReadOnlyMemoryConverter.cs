namespace System.Text.Json;

/// <summary>
/// Represents a JSON converter that can convert <see cref="ReadOnlyMemory{T}"/> instances into JSON strings.
/// </summary>
/// <typeparam name="T">The type of each element.</typeparam>
public sealed class ReadOnlyMemoryConverter<T> : JsonConverter<ReadOnlyMemory<T>>
{
	/// <inheritdoc/>
	public override bool HandleNull => true;


	/// <inheritdoc/>
	public override ReadOnlyMemory<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> JsonSerializer.Deserialize<T[]>(ref reader, options);

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, ReadOnlyMemory<T> value, JsonSerializerOptions options)
		=> writer.WriteArray(value.ToArray(), options);
}
