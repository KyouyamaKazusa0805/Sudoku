namespace Sudoku.Resources;

/// <summary>
/// Represents a type that compares culture language names.
/// </summary>
public static class LanguageNameComparisonExtensions
{
	/// <summary>
	/// Compares two <see cref="string"/> values, treated as culture name,
	/// to get a <see cref="bool"/> result indicating whether they are same culture name,
	/// or <paramref name="this"/> includes <paramref name="otherName"/>.
	/// </summary>
	/// <param name="this">The culture.</param>
	/// <param name="otherName">The other name to be compared.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsCultureNameEqual(this string @this, string otherName)
		=> @this.StartsWith(otherName, StringComparison.OrdinalIgnoreCase);
}
