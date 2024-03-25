namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Provides with extension methods on <see cref="SingleTechnique"/>.
/// </summary>
/// <seealso cref="SingleTechnique"/>
public static class SingleTechniqueExtensions
{
	/// <summary>
	/// Try to get the name of the current <see cref="SingleTechnique"/>.
	/// </summary>
	/// <param name="this">The <see cref="SingleTechnique"/> instance.</param>
	/// <param name="culture">The culture information.</param>
	/// <returns>The name of the current technique.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the target technique is out of range.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetName(this SingleTechnique @this, CultureInfo? culture = null)
		=> @this switch
		{
			SingleTechnique.FullHouse => Technique.FullHouse.GetName(culture),
			SingleTechnique.LastDigit => Technique.LastDigit.GetName(culture),
			SingleTechnique.HiddenSingle => ResourceDictionary.Get("SingleTechnique_HiddenSingle", culture),
			SingleTechnique.HiddenSingleBlock => Technique.CrosshatchingBlock.GetName(culture),
			SingleTechnique.HiddenSingleRow => Technique.CrosshatchingRow.GetName(culture),
			SingleTechnique.HiddenSingleColumn => Technique.CrosshatchingColumn.GetName(culture),
			SingleTechnique.NakedSingle => Technique.NakedSingle.GetName(culture),
			_ => throw new ArgumentOutOfRangeException(nameof(@this))
		};
}
