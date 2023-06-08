namespace Sudoku.Analytics.Resources;

/// <summary>
/// Represents an entry that can access resource strings.
/// </summary>
public static class MergedResources
{
	/// <summary>
	/// Indicates English language identifier.
	/// </summary>
	public const string EnglishLanguage = "en";

	/// <summary>
	/// Indicates Chinese language identifier.
	/// </summary>
	public const string ChineseLanguage = "zh";


	/// <summary>
	/// Gets the value via the specified string key.
	/// </summary>
	/// <param name="key">The resource key.</param>
	/// <returns>The value. If none found, <see langword="null"/>.</returns>
	public static string? GetString(string key)
	{
		var @default = Sudoku.Analytics.Resources.Resources.ResourceManager;
		return @default.GetString(key) ?? @default.GetString(key, CultureInfo.GetCultureInfo(1033));
	}
}
