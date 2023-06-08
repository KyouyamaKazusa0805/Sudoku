namespace Sudoku.CommandLine.Strings;

/// <inheritdoc cref="Analytics.Resources.MergedResources"/>
public static class MergedResources
{
	/// <summary>
	/// Indicates the default language culture used in the resource file.
	/// </summary>
	private static readonly CultureInfo Neutral = CultureInfo.GetCultureInfo(1033);


	/// <summary>
	/// Gets the value via the specified string key.
	/// </summary>
	/// <param name="key">The resource key.</param>
	/// <returns>The value. If none found, <see langword="null"/>.</returns>
	public static string? GetString(string key)
	{
		var @default = Resources.ResourceManager;
		return @default.GetString(key) ?? @default.GetString(key, Neutral);
	}
}
