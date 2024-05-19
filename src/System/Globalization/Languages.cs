namespace System.Globalization;

/// <summary>
/// Indicates the supported languages in resource.
/// </summary>
public static class Languages
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
	/// Indicates the English culture.
	/// </summary>
	public static readonly CultureInfo EnglishCulture = new(1033);

	/// <summary>
	/// Indicates the Chinese culture.
	/// </summary>
	public static readonly CultureInfo ChineseCulture = new(2052);
}
