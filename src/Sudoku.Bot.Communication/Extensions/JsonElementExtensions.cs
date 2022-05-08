namespace System.Text.Json;

/// <summary>
/// Provides with extension methods on <see cref="JsonElement"/>.
/// </summary>
/// <seealso cref="JsonElement"/>
public static class JsonElementExtensions
{
	/// <summary>
	/// 查找JSON对象
	/// </summary>
	/// <param name="element"></param>
	/// <param name="name">json的key</param>
	/// <returns></returns>
	public static JsonElement? Get(this JsonElement element, string name)
		=> element.ValueKind != JsonValueKind.Object
			? null
			: element.TryGetProperty(name, out JsonElement value) ? value : null;

	/// <summary>
	/// 索引JSON数组
	/// </summary>
	/// <param name="element"></param>
	/// <param name="index">json的index</param>
	/// <returns></returns>
	public static JsonElement? Get(this JsonElement element, int index)
	{
		if (element.ValueKind != JsonValueKind.Array)
		{
			return element.ValueKind == JsonValueKind.Object ? element.Get(index.ToString()) : null;
		}

		var value = element.EnumerateArray().ElementAtOrDefault(index);
		return value.ValueKind != JsonValueKind.Undefined ? value : null;
	}
}
