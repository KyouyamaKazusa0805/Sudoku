namespace System.Text.Json;

/// <summary>
/// Provides extension methods on <see cref="Utf8JsonReader"/>.
/// </summary>
/// <seealso cref="Utf8JsonReader"/>
public static class Utf8JsonReaderExtensions
{
	/// <summary>
	/// Try to read an object.
	/// </summary>
	/// <typeparam name="T">The object to read.</typeparam>
	/// <param name="this">The reader instance.</param>
	/// <param name="converter">The converter to convert the value.</param>
	/// <param name="type">The type to convert.</param>
	/// <param name="options">The options on deserialization.</param>
	/// <returns>The instance.</returns>
	public static T? ReadObject<T>(
		this ref Utf8JsonReader @this,
		JsonConverter<T>? converter,
		Type type,
		JsonSerializerOptions options
	)
	{
		if (converter is not null)
		{
			@this.Read();
			return converter.Read(ref @this, type, options);
		}
		else
		{
			return JsonSerializer.Deserialize<T>(ref @this, options);
		}
	}
}
