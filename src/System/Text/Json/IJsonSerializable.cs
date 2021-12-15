namespace System.Text.Json;

/// <summary>
/// Defines a type that can serialize and deserialize.
/// </summary>
/// <typeparam name="T">The type of the instance.</typeparam>
/// <typeparam name="TConverter">The type of the converter bound with.</typeparam>
public interface IJsonSerializable<[Self] T, in TConverter>
where T : IJsonSerializable<T, TConverter>
where TConverter : JsonConverter<T>, new()
{
	/// <summary>
	/// Indicates the JSON serializer option instance that used
	/// during serialization or deserialization operation.
	/// </summary>
	protected static readonly JsonSerializerOptions SerializerOptions =
		new JsonSerializerOptions { WriteIndented = true }
			.AppendConverter<T, TConverter>(new TConverter());


	/// <summary>
	/// Serializes the current instance, and converts the instance into a JSON string.
	/// </summary>
	/// <param name="instance">The instance to serialize.</param>
	/// <returns>The JSON string result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(T instance) => JsonSerializer.Serialize(instance, SerializerOptions);

	/// <summary>
	/// Deserializes the specified possible JSON string, and parses the string, then return the result.
	/// </summary>
	/// <param name="json">The JSON string.</param>
	/// <returns>The instance result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T? Deserialize(string json) => JsonSerializer.Deserialize<T>(json, SerializerOptions);
}
