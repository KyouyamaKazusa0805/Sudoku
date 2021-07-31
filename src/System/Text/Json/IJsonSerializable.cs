using System.Extensions;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace System.Text.Json
{
	/// <summary>
	/// Defines a type that can serialize and deserialize.
	/// </summary>
	/// <typeparam name="TSelf">The type of the instance.</typeparam>
	/// <typeparam name="TConverter">The type of the converter bound with.</typeparam>
	public interface IJsonSerializable<TSelf, in TConverter>
		where TSelf : IJsonSerializable<TSelf, TConverter>
		where TConverter : JsonConverter<TSelf>, new()
	{
		/// <summary>
		/// Indicates the JSON serializer option instance that used
		/// during serialization or deserialization operation.
		/// </summary>
		protected static readonly JsonSerializerOptions SerializerOptions;


		/// <summary>
		/// Indicates the <see langword="static"/> constructor of this type.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static IJsonSerializable() => SerializerOptions = new JsonSerializerOptions
		{
			WriteIndented = true
		}.AppendConverter<TSelf, TConverter>(new TConverter());


		/// <summary>
		/// Serializes the current instance, and converts the instance into a JSON string.
		/// </summary>
		/// <param name="instance">The instance to serialize.</param>
		/// <returns>The JSON string result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Serialize(TSelf instance) => JsonSerializer.Serialize(instance, SerializerOptions);

		/// <summary>
		/// Deserializes the specified possible JSON string, and parses the string, then return the result.
		/// </summary>
		/// <param name="json">The JSON string.</param>
		/// <returns>The instance result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TSelf? Deserialize(string json) => JsonSerializer.Deserialize<TSelf>(json, SerializerOptions);
	}
}
