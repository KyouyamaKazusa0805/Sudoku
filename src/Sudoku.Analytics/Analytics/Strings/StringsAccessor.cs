namespace Sudoku.Analytics.Strings;

/// <summary>
/// Represents an entry that can access resource strings.
/// </summary>
public static class StringsAccessor
{
	/// <summary>
	/// Gets the value via the specified string key, with the specified culture information.
	/// </summary>
	/// <param name="key">The resource key.</param>
	/// <param name="culture">The culture information instance.</param>
	/// <returns>The value. If none found, <see langword="null"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? GetString(string key, CultureInfo culture)
		=> (culture is null ? Resources.ResourceManager.GetString(key) : Resources.ResourceManager.GetString(key, culture))
		?? Resources.ResourceManager.GetString(key, IResourceReader.DefaultCulture);
}
