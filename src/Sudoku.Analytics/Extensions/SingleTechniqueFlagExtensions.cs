namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Provides with extension methods on <see cref="SingleTechniqueFlag"/>.
/// </summary>
/// <seealso cref="SingleTechniqueFlag"/>
public static class SingleTechniqueFlagExtensions
{
	/// <summary>
	/// Try to get the name of the current <see cref="SingleTechniqueFlag"/>.
	/// </summary>
	/// <param name="this">The <see cref="SingleTechniqueFlag"/> instance.</param>
	/// <param name="culture">The culture information.</param>
	/// <returns>The name of the current technique.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the target technique is out of range.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetName(this SingleTechniqueFlag @this, CultureInfo? culture = null)
		=> @this switch
		{
			SingleTechniqueFlag.FullHouse => Technique.FullHouse.GetName(culture),
			SingleTechniqueFlag.LastDigit => Technique.LastDigit.GetName(culture),
			SingleTechniqueFlag.HiddenSingle => ResourceDictionary.Get("SingleTechnique_HiddenSingle", culture),
			SingleTechniqueFlag.HiddenSingleBlock => Technique.CrosshatchingBlock.GetName(culture),
			SingleTechniqueFlag.HiddenSingleRow => Technique.CrosshatchingRow.GetName(culture),
			SingleTechniqueFlag.HiddenSingleColumn => Technique.CrosshatchingColumn.GetName(culture),
			SingleTechniqueFlag.NakedSingle => Technique.NakedSingle.GetName(culture),
			_ => throw new ArgumentOutOfRangeException(nameof(@this))
		};
}
