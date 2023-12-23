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
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	public static T? GetNestedObject<T>(this scoped ref Utf8JsonReader @this, JsonSerializerOptions? options = null)
		=> JsonSerializer.Deserialize<T>(ref @this, options);
}
