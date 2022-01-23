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
	/// <param name="converter">
	/// The converter to convert the value. If the value is <see langword="null"/>,
	/// the default deserialization method
	/// <see cref="JsonSerializer.Deserialize{TValue}(ref Utf8JsonReader, JsonSerializerOptions?)"/>
	/// will be invoked.
	/// </param>
	/// <param name="options">
	/// The options on deserialization. The argument cannot be <see langword="null"/>
	/// when the argument <paramref name="converter"/> is not <see langword="null"/>;
	/// otherwise, a <see cref="JsonException"/> will be thrown.
	/// </param>
	/// <returns>The instance.</returns>
	/// <remarks><i>
	/// Please note that the method will implicitly
	/// invoke the method <see cref="Utf8JsonReader.Read"/> once
	/// when the argument <paramref name="converter"/> is not <see langword="null"/>.
	/// </i></remarks>
	/// <exception cref="JsonException">
	/// Throws when:
	/// <list type="bullet">
	/// <item>The current JSON value is mal-formed.</item>
	/// <item>
	/// The argument <paramref name="options"/> is <see langword="null"/>
	/// when the arguemnt <paramref name="converter"/> is not <see langword="null"/>.
	/// </item>
	/// </list>
	/// </exception>
	/// <seealso cref="Utf8JsonReader.Read"/>
	/// <seealso cref="JsonSerializer.Deserialize{TValue}(ref Utf8JsonReader, JsonSerializerOptions?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T? ReadObject<T>(
		this ref Utf8JsonReader @this,
		JsonConverter<T>? converter,
		[NotNullIfNotNull("converter")] JsonSerializerOptions? options
	) =>
		converter is not null
			? !@this.Read()
				? throw new JsonException("The current JSON value is mal-formed.")
				: converter.Read(
					ref @this,
					typeof(T),
					options ?? throw new JsonException($"Argument '{nameof(options)}' cannot be null when the argument '{nameof(converter)}' is not null.")
				)
			: JsonSerializer.Deserialize<T>(ref @this, options);
}
