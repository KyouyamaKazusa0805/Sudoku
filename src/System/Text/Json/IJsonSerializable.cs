namespace System.Text.Json;

/// <summary>
/// Represents a type that supports for JSON serialization.
/// </summary>
/// <typeparam name="TSelf">The type of itself.</typeparam>
public interface IJsonSerializable<TSelf> where TSelf : IJsonSerializable<TSelf>
{
	/// <summary>
	/// Returns a <see cref="JsonSerializerOptions"/> instance that can be used in following methods.
	/// </summary>
	public static abstract JsonSerializerOptions DefaultOptions { get; }


	/// <summary>
	/// Converts the current instance to a JSON string value.
	/// </summary>
	/// <returns>The JSON string value.</returns>
	public abstract string ToJsonString();


	/// <summary>
	/// Converts the JSON string value into a valid <typeparamref name="TSelf"/> instance.
	/// </summary>
	/// <param name="jsonString">The JSON string to be converted from.</param>
	/// <returns>The valid <typeparamref name="TSelf"/> instance converted.</returns>
	public static abstract TSelf? FromJsonString(string jsonString);
}
