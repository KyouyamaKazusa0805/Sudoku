namespace SudokuStudio.Resources;

/// <summary>
/// Defines an easy entry to get <see cref="string"/> resources.
/// </summary>
internal static class ResourceDictionary
{
	/// <summary>
	/// Try to fetch the target resource via the specified key.
	/// </summary>
	/// <param name="key">The resource key.</param>
	/// <returns>The target resource.</returns>
	public static string GetString(string key) => (string)Application.Current.Resources[key];
}
