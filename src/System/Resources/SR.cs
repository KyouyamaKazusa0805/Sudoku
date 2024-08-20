namespace System.Resources;

/// <summary>
/// Represents a easy way to fetch resource values using the specified key and culture information.
/// </summary>
public static partial class SR
{
	/// <summary>
	/// Indicates English language identifier.
	/// </summary>
	public const string EnglishLanguage = "en-US";

	/// <summary>
	/// Indicates Chinese language identifier.
	/// </summary>
	public const string ChineseLanguage = "zh-CN";

	/// <summary>
	/// Indicates the resource manager reflection binding flags.
	/// </summary>
	private const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;


	/// <summary>
	/// Indicates the default culture.
	/// </summary>
	public static readonly CultureInfo DefaultCulture = new(EnglishLanguage);
}
