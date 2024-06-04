namespace Sudoku.Concepts;

/// <summary>
/// Provides with extension methods for <see cref="Grid"/> on analyzing.
/// </summary>
/// <seealso cref="Grid"/>
public static class GridAnalyticsExtensions
{
	/// <summary>
	/// Checks whether the puzzle can be solved using only full house.
	/// </summary>
	/// <param name="this">The grid to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool CanPrimaryFullHouse(this scoped in Grid @this)
		=> Analyzer.Default
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
	public static bool CanPrimaryHiddenSingle(this scoped in Grid @this, bool allowHiddenSingleInLine)
		=> Analyzer.Default
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
	public static bool CanPrimaryNakedSingle(this scoped in Grid @this)
		=> Analyzer.Default
			.WithStepSearchers(new SingleStepSearcher { EnableFullHouse = true })
			.WithUserDefinedOptions(new() { DistinctDirectMode = true, IsDirectMode = true })
			.WithConditionalOptions(new() { LimitedSingle = SingleTechniqueFlag.NakedSingle })
			.Analyze(in @this)
			.IsSolved;
}
