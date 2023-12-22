using System.Globalization;
using System.Runtime.CompilerServices;

namespace Sudoku.Analytics.Strings;

/// <summary>
/// Represents an entry that can access resource strings.
/// </summary>
public static class StringsAccessor
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
	/// Gets the value via the specified string key, with the specified culture information.
	/// </summary>
	/// <param name="key">The resource key.</param>
	/// <param name="culture">The culture information instance.</param>
	/// <returns>The value. If none found, <see langword="null"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? GetString(string key, CultureInfo culture)
		=> (culture is null ? Resources.ResourceManager.GetString(key) : Resources.ResourceManager.GetString(key, culture))
		?? Resources.ResourceManager.GetString(key, new(1033));
}
