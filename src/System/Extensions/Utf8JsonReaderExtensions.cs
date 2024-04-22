namespace System.Text.Json;

/// <summary>
/// Provides extension methods on <see cref="Utf8JsonReader"/>.
/// </summary>
/// <seealso cref="Utf8JsonReader"/>
public static class Utf8JsonReaderExtensions
{
	/// <summary>
	/// To read as a nested object in the JSON string stream.
	/// </summary>
	/// <typeparam name="T">The type of the instance to be deserialized.</typeparam>
	/// <param name="this">The <see cref="Utf8JsonReader"/> instance.</param>
	/// <param name="options">The options.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T? GetNestedObject<T>(this ref Utf8JsonReader @this, JsonSerializerOptions? options = null)
		=> JsonSerializer.Deserialize<T>(ref @this, options);
}
