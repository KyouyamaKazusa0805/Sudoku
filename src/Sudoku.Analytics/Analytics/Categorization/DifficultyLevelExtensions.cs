namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Provides with extension methods on <see cref="DifficultyLevel"/>.
/// </summary>
/// <seealso cref="DifficultyLevel"/>
public static class DifficultyLevelExtensions
{
	/// <summary>
	/// Gets the name of the current value, with specified culture.
	/// </summary>
	/// <param name="this">The value.</param>
	/// <param name="culture">The culture.</param>
	/// <returns>The string value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetName(this DifficultyLevel @this, CultureInfo? culture = null)
		=> ResourceDictionary.Get(@this.ToString(), culture);
}
