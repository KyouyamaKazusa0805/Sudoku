namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Provides with extension methods on <see cref="SingleTechniquePrefer"/>.
/// </summary>
/// <seealso cref="SingleTechniquePrefer"/>
public static class SingleTechniquePreferExtensions
{
	/// <inheritdoc cref="ICultureFormattable.ToString(CultureInfo?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToTechniqueString(this SingleTechniquePrefer @this, CultureInfo? culture = null)
		=> ResourceDictionary.Get($"{nameof(SingleTechniquePrefer)}_{@this}", culture);
}
