namespace Sudoku.CommandLine.Strings;

/// <inheritdoc cref="Analytics.Strings.StringsAccessor"/>
public static class StringsAccessor
{
	/// <summary>
	/// Gets the value via the specified string key.
	/// </summary>
	/// <param name="key">The resource key.</param>
	/// <returns>The value. If none found, <see langword="null"/>.</returns>
	public static string? GetString(string key)
		=> Resources.ResourceManager.GetString(key) ?? Resources.ResourceManager.GetString(key, CultureInfo.GetCultureInfo(1033));
}
