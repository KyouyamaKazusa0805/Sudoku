namespace Sudoku.Concepts;

/// <summary>
/// Provides with extension methods on <see cref="Grid"/>.
/// </summary>
/// <seealso cref="Grid"/>
public static class GridExtensions
{
	/// <summary>
	/// Checks whether the puzzle can be solved using only full house.
	/// </summary>
	/// <param name="this">The grid to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool CanPrimaryFullHouse(this scoped ref readonly Grid @this)
		=> Analyzers.Default
			.WithStepSearchers(new SingleStepSearcher { EnableFullHouse = true })
			.WithConditionalOptions(new() { LimitedSingle = SingleTechniqueFlag.FullHouse })
			.Analyze(in @this)
			.IsSolved;

	/// <summary>
	/// Checks whether the puzzle can be solved using only full house and hidden single.
	/// </summary>
	/// <param name="this">The grid to be checked.</param>
	/// <param name="allowHiddenSingleInLine">
	/// A <see cref="bool"/> value indicating whether hidden single includes line types.
	/// </param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool CanPrimaryHiddenSingle(this scoped ref readonly Grid @this, bool allowHiddenSingleInLine)
		=> Analyzers.Default
			.WithStepSearchers(new SingleStepSearcher { EnableFullHouse = true, EnableLastDigit = true })
			.WithUserDefinedOptions(new() { DistinctDirectMode = true, IsDirectMode = true })
			.WithConditionalOptions(new() { LimitedSingle = SingleTechniqueFlag.HiddenSingle, AllowsHiddenSingleInLines = allowHiddenSingleInLine })
			.Analyze(in @this)
			.IsSolved;

	/// <summary>
	/// Checks whether the puzzle can be solved using only naked single.
	/// </summary>
	/// <param name="this">The grid to be checked.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool CanPrimaryNakedSingle(this scoped ref readonly Grid @this)
		=> Analyzers.Default
			.WithStepSearchers(new SingleStepSearcher { EnableFullHouse = true })
			.WithUserDefinedOptions(new() { DistinctDirectMode = true, IsDirectMode = true })
			.WithConditionalOptions(new() { LimitedSingle = SingleTechniqueFlag.NakedSingle })
			.Analyze(in @this)
			.IsSolved;
}
