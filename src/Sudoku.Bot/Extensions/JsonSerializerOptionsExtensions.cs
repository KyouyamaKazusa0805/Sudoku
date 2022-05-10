namespace System.Text.Json;

/// <summary>
/// Provides extension methods on <see cref="JsonSerializerOptions"/>.
/// </summary>
/// <seealso cref="JsonSerializerOptions"/>
internal static class JsonSerializerOptionsExtensions
{
	/// <summary>
	/// Adds a new converter of type <typeparamref name="TConverter"/>
	/// into the current <see cref="JsonSerializerOptions"/> instance.
	/// </summary>
	/// <typeparam name="TConverter">The type of the JSON converter.</typeparam>
	/// <param name="this">The instance.</param>
	/// <returns>The reference same as <paramref name="this"/>.</returns>
	public static JsonSerializerOptions AddConverter<TConverter>(this JsonSerializerOptions @this)
		where TConverter : notnull, JsonConverter, new()
	{
		@this.Converters.Add(new TConverter());

		return @this;
	}
}
