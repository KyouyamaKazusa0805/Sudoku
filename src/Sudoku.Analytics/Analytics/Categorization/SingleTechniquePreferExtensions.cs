namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Provides with extension methods on <see cref="SingleTechnique"/>.
/// </summary>
/// <seealso cref="SingleTechnique"/>
public static class SingleTechniquePreferExtensions
{
	/// <inheritdoc cref="ICultureFormattable.ToString(CultureInfo?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToTechniqueString(this SingleTechnique @this, CultureInfo? culture = null)
		=> ResourceDictionary.Get($"SingleTechnique_{@this}", culture);
}
