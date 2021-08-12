using System.Text.Json.Serialization;

namespace System.Text.Json
{
	/// <summary>
	/// Provides extension methods on <see cref="Utf8JsonWriter"/>.
	/// </summary>
	/// <seealso cref="Utf8JsonWriter"/>
	public static class Utf8JsonWriterExtensions
	{
		/// <summary>
		/// Try to write an object.
		/// </summary>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <param name="this">The instance.</param>
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

		/// <summary>
		/// Try to write a series of objects.
		/// </summary>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <param name="this">The instance.</param>
		/// <param name="values">Values to serialize.</param>
		/// <param name="converter">The converter.</param>
		/// <param name="options">The options on serialization.</param>
		public static void WriteObjects<T>(
			this Utf8JsonWriter @this, IEnumerable<T>? values, JsonConverter<T>? converter,
			JsonSerializerOptions options)
		{
			if (values is null)
			{
				return;
			}

			foreach (var value in values)
			{
				@this.WriteObject(value, converter, options);
			}
		}
	}
}
