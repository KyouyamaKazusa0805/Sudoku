namespace System.Text.Json;

/// <summary>
/// Provides with extension methods with <see cref="JsonElement"/>.
/// </summary>
/// <seealso cref="JsonElement"/>
internal static class JsonElementExtensions
{
	/// <summary>
	/// Try to get all property values via the specified names.
	/// </summary>
	/// <param name="this">The <see cref="JsonElement"/> instance.</param>
	/// <param name="propertyNames">The property names you want to check and get corresponding values.</param>
	/// <returns>The pair of <see cref="string"/> values indicating the found values.</returns>
	public static (string PropertyName, string? CorrespondingValue)[]? TryGetPropertyValues(
		this in JsonElement @this, params string[] propertyNames)
	{
		if (propertyNames.Length is var propertyNamesLength && propertyNamesLength == 0)
		{
			return null;
		}

		var result = new List<(string, string?)>(propertyNamesLength);
		foreach (string propertyName in propertyNames)
		{
			if (@this.TryGetProperty(propertyName, out var foundJsonElement))
			{
				result.Add((propertyName, foundJsonElement.GetString()));
			}
		}

		return result.Count == 0 ? null : result.ToArray();
	}

	/// <summary>
	/// Converts a <see cref="JsonElement"/> into a <see cref="Dictionary{TKey, TValue}"/> instance.
	/// </summary>
	/// <param name="this">The <see cref="JsonElement"/> instance.</param>
	/// <returns>
	/// The <see cref="Dictionary{TKey, TValue}"/> instance. The keys and values in this dictionary
	/// are all <see cref="string"/> instances.
	/// </returns>
	public static Dictionary<string, string> ToDictionaryOfStringElements(this JsonElement @this)
		=> new(
			from subelement in @this.EnumerateObject()
			select new KeyValuePair<string, string>(subelement.Name, subelement.Value.ToString())
		);
}
