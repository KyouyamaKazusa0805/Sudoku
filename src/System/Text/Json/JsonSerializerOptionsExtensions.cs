using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace System.Text.Json
{
	/// <summary>
	/// Prpvides extension methods on <see cref="JsonSerializerOptions"/>.
	/// </summary>
	/// <seealso cref="JsonSerializerOptions"/>
	public static class JsonSerializerOptionsExtensions
	{
		/// <summary>
		/// Try to get the first JSON converter whose implemented type is specified as the generic argument.
		/// </summary>
		/// <typeparam name="TSelf">
		/// The type of the data as the result value that JSON converter returns.
		/// </typeparam>
		/// <typeparam name="TConverter">The type of the JSON converter.</typeparam>
		/// <param name="this">The current instance.</param>
		/// <returns>
		/// The found JSON converter instance. If none found or the wrong type converting, the method will
		/// return a new instance that created by the specified type <typeparamref name="TSelf"/>
		/// (i.e. a/an <typeparamref name="TConverter"/> instance).
		/// </returns>
		public static JsonConverter<TSelf> GetConverter<TSelf, TConverter>(this JsonSerializerOptions @this)
			where TSelf : IJsonSerializable<TSelf, TConverter>
			where TConverter : JsonConverter<TSelf>, new()
		{
			try
			{
				return (JsonConverter<TSelf>)@this.GetConverter(typeof(JsonConverter<TSelf>));
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
		/// <typeparam name="TSelf">
		/// The type of the data as the result value that JSON converter returns.
		/// </typeparam>
		/// <typeparam name="TConverter">The type of the JSON converter.</typeparam>
		/// <param name="this">The current instance.</param>
		/// <param name="converter">The JSON converter to append.</param>
		/// <returns>The reference of the current instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static JsonSerializerOptions AppendConverter<TSelf, TConverter>(
			this JsonSerializerOptions @this, TConverter converter)
			where TSelf : IJsonSerializable<TSelf, TConverter>
			where TConverter : JsonConverter<TSelf>, new()
		{
			@this.Converters.Add(converter);
			return @this;
		}
	}
}
