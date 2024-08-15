namespace Sudoku.Snyder;

/// <summary>
/// Provides with extension methods on <see cref="Grid"/>, calculating with Snyder checking.
/// </summary>
/// <seealso cref="Grid"/>
public static class GridSnyderExtensions
{
	/// <summary>
	/// Indicates the backing collector.
	/// </summary>
	private static readonly Collector Collector = new Collector()
		.WithStepSearchers(
			new SingleStepSearcher { EnableFullHouse = true, EnableLastDigit = true, HiddenSinglesInBlockFirst = true },
			new DirectIntersectionStepSearcher { AllowDirectClaiming = true, AllowDirectPointing = true },
			new DirectSubsetStepSearcher
			{
				AllowDirectHiddenSubset = true,
				AllowDirectLockedHiddenSubset = true,
				AllowDirectLockedSubset = true,
				AllowDirectNakedSubset = true,
				DirectHiddenSubsetMaxSize = 4,
				DirectNakedSubsetMaxSize = 4
			}
		)
		.WithUserDefinedOptions(new() { DistinctDirectMode = true, IsDirectMode = true });


	/// <summary>
	/// Determine whether the grid lacks some candidates that are included in a grid,
	/// through basic elimination rule (Naked Single checking) and specified Snyder techniques.
	/// </summary>
	/// <param name="this">The grid to be checked.</param>
	/// <param name="techniques">A list of techniques to be checked.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="techniques"/> is greater than the maximum value of enumeration field defined.
	/// </exception>
	public static bool IsMissingCandidates(this ref readonly Grid @this, SnyderTechniques techniques)
	{
		switch (techniques)
		{
			case SnyderTechniques.None:
			{
				return @this.IsMissingCandidates;
			}
			case < SnyderTechniques.None or > SnyderTechniques.LockedHiddenTriple:
			{
				throw new ArgumentOutOfRangeException(nameof(techniques));
			}
			default:
			{
				var gridResetCandidates = @this.ResetCandidatesGrid;
				foreach (var step in Collector.Collect(new(in gridResetCandidates)))
				{
					if (techniques.HasFlag(Enum.Parse<SnyderTechniques>(step.Code.ToString())))
					{
						gridResetCandidates.Apply(step);
					}
				}
				return @this != gridResetCandidates;
			}
		}
	}
}
