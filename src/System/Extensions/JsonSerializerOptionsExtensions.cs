namespace System.Text.Json;

/// <summary>
/// Provdies extension methods on <see cref="JsonSerializerOptions"/>.
/// </summary>
/// <seealso cref="JsonSerializerOptions"/>
public static class JsonSerializerOptionsExtensions
{
	/// <summary>
	/// Returns the converter that supports the givent type, or the <typeparamref name="TConverter"/>
	/// will be used when the property <see cref="JsonSerializerOptions.Converters"/>
	/// doesn't contain any valid converters.
	/// </summary>
	/// <typeparam name="T">The type to get converter.</typeparam>
	/// <typeparam name="TConverter">
	/// The type that is the converter type to convert the instance of type <typeparamref name="T"/>.
	/// </typeparam>
	/// <param name="this">The options to check the existence of the converter.</param>
	/// <returns>
	/// The converter that supports the givent type, or the <typeparamref name="TConverter"/>
	/// will be used when the property <see cref="JsonSerializerOptions.Converters"/>
	/// doesn't contain any valid converters.
	/// </returns>
	/// <seealso cref="JsonSerializerOptions.Converters"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static JsonConverter<T> GetConverter<T, TConverter>(this JsonSerializerOptions @this)
		where TConverter : JsonConverter<T>, new() =>
		(JsonConverter<T>?)@this.GetConverter(typeof(T)) ?? new TConverter();
}
