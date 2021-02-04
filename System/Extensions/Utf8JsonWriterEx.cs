using System.Text.Json;
using System.Text.Json.Serialization;

namespace System.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Utf8JsonWriter"/>.
	/// </summary>
	/// <seealso cref="Utf8JsonWriter"/>
	public static class Utf8JsonWriterEx
	{
		/// <summary>
		/// Try to write an object.
		/// </summary>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
		/// <param name="value">The value to serialize.</param>
		/// <param name="converter">The converter.</param>
		/// <param name="options">The options on serialization.</param>
		public static void WriteObject<T>(
			this Utf8JsonWriter @this, T value, JsonConverter<T>? converter, JsonSerializerOptions options)
		{
			if (converter is not null)
			{
				converter.Write(@this, value, options);
			}
			else
			{
				JsonSerializer.Serialize(@this, value, options);
			}
		}
	}
}
