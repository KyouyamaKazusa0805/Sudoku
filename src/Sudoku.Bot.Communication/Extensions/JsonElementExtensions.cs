namespace System.Text.Json;

/// <summary>
/// Provides with extension methods on <see cref="JsonElement"/>.
/// </summary>
/// <seealso cref="JsonElement"/>
public static class JsonElementExtensions
{
	/// <summary>
	/// Try to find the JSON object via the name.
	/// </summary>
	/// <param name="this">The current instance.</param>
	/// <param name="name">The key that corresponds to the value you want to find.</param>
	/// <returns>The <see cref="JsonElement"/> instance found. If failed to find, <see langword="null"/>.</returns>
	public static JsonElement? Get(this JsonElement @this, string name)
		=> @this.ValueKind != JsonValueKind.Object ? null : @this.TryGetProperty(name, out var value) ? value : null;

	/// <summary>
	/// Try to index the JSON array instance via the index.
	/// </summary>
	/// <param name="element">The current instance.</param>
	/// <param name="index">The index you want to find.</param>
	/// <returns>
	/// The <see cref="JsonElement"/> instance at the specified index. If failed to index, <see langword="null"/>.
	/// </returns>
	public static JsonElement? Get(this JsonElement element, int index)
		=> element.ValueKind is var valueKind && valueKind != JsonValueKind.Array
			? valueKind == JsonValueKind.Object
				? element.Get(index.ToString())
				: null
			: element.EnumerateArray().ElementAtOrDefault(index) is { ValueKind: not JsonValueKind.Undefined } value
				? value
				: null;
}
