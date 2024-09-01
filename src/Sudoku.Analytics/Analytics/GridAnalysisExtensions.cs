namespace Sudoku.Analytics;

/// <summary>
/// Provides with extension methods on <see cref="Grid"/>.
/// </summary>
/// <seealso cref="Grid"/>
public static class GridAnalysisExtensions
{
	/// <summary>
	/// Applies for all conclusions into the current <see cref="Grid"/> instance.
	/// </summary>
	/// <param name="this">A <see cref="Grid"/> instance that receives the conclusions to be applied.</param>
	/// <param name="step">A conclusion-provider <see cref="Step"/> instance.</param>
	public static void Apply(this ref Grid @this, Step step)
	{
		foreach (var conclusion in step.Conclusions)
		{
			@this.Apply(conclusion);
		}
	}

	/// <summary>
	/// Checks whether the puzzle can be solved using only full house.
	/// </summary>
	/// <param name="this">The grid to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool CanPrimaryFullHouse(this ref readonly Grid @this)
		=> Analyzer.Default
			.WithStepSearchers(new SingleStepSearcher { EnableFullHouse = true })
			.WithUserDefinedOptions(new() { PrimarySingle = SingleTechniqueFlag.FullHouse })
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
	public static bool CanPrimaryHiddenSingle(this ref readonly Grid @this, bool allowHiddenSingleInLine)
		=> Analyzer.Default
			.WithStepSearchers(new SingleStepSearcher { EnableFullHouse = true, EnableLastDigit = true })
			.WithUserDefinedOptions(
				new()
				{
					DistinctDirectMode = true,
					IsDirectMode = true,
					PrimarySingle = SingleTechniqueFlag.HiddenSingle,
					PrimaryHiddenSingleAllowsLines = allowHiddenSingleInLine
				}
			)
			.Analyze(in @this)
			.IsSolved;

	/// <summary>
	/// Checks whether the puzzle can be solved using only naked single.
	/// </summary>
	/// <param name="this">The grid to be checked.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool CanPrimaryNakedSingle(this ref readonly Grid @this)
		=> Analyzer.Default
			.WithStepSearchers(new SingleStepSearcher { EnableFullHouse = true })
			.WithUserDefinedOptions(
				new()
				{
					DistinctDirectMode = true,
					IsDirectMode = true,
					PrimarySingle = SingleTechniqueFlag.NakedSingle
				}
			)
			.Analyze(in @this)
			.IsSolved;
}
