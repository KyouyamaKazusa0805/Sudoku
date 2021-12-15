namespace System.Text.Json;

/// <summary>
/// Prpvides extension methods on <see cref="JsonSerializerOptions"/>.
/// </summary>
/// <seealso cref="JsonSerializerOptions"/>
public static class JsonSerializerOptionsExtensions
{
	/// <summary>
	/// Try to get the first JSON converter whose implemented type is specified as the generic argument.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the data as the result value that JSON converter returns.
	/// </typeparam>
	/// <typeparam name="TConverter">The type of the JSON converter.</typeparam>
	/// <param name="this">The current instance.</param>
	/// <returns>
	/// The found JSON converter instance. If none found or the wrong type converting, the method will
	/// return a new instance that created by the specified type <typeparamref name="T"/>
	/// (i.e. a/an <typeparamref name="TConverter"/> instance).
	/// </returns>
	public static JsonConverter<T> GetConverter<T, TConverter>(this JsonSerializerOptions @this)
	where T : IJsonSerializable<T, TConverter>
	where TConverter : JsonConverter<T>, new()
	{
		try
		{
			return (JsonConverter<T>)@this.GetConverter(typeof(JsonConverter<T>));
		}
		catch (Exception ex) when (ex is NotSupportedException or InvalidOperationException)
		{
			return new TConverter();
		}
	}

	/// <summary>
	/// Append the specified JSON converter into the collection, and returns the reference
	/// of this current instance.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the data as the result value that JSON converter returns.
	/// </typeparam>
	/// <typeparam name="TConverter">The type of the JSON converter.</typeparam>
	/// <param name="this">The current instance.</param>
	/// <param name="converter">The JSON converter to append.</param>
	/// <returns>The reference of the current instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static JsonSerializerOptions AppendConverter<T, TConverter>(
		this JsonSerializerOptions @this,
		TConverter converter
	)
	where T : IJsonSerializable<T, TConverter>
	where TConverter : JsonConverter<T>, new()
	{
		@this.Converters.Add(converter);
		return @this;
	}
}
