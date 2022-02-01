using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Steps;

namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Brute Force</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Brute Force</item>
/// </list>
/// </summary>
[StepSearcher(IsOptionsFixed = true, IsDirect = true)]
public sealed unsafe class BruteForceStepSearcher : IBruteForceStepSearcher
{
	/// <summary>
	/// The order of cell offsets to get values.
	/// </summary>
	private static readonly int[] TryAndErrorOrder =
	{
		40, 41, 32, 31, 30, 39, 48, 49, 50,
		51, 42, 33, 24, 23, 22, 21, 20, 29,
		38, 47, 56, 57, 58, 59, 60, 61, 52,
		43, 34, 25, 16, 15, 14, 13, 12, 11,
		10, 19, 28, 37, 46, 55, 64, 65, 66,
		67, 68, 69, 70, 71, 62, 53, 44, 35,
		26, 17,  8,  7,  6,  5,  4,  3,  2,
		 1,  0,  9, 18, 27, 36, 45, 54, 63,
		72, 73, 74, 75, 76, 77, 78, 79, 80
	};


	/// <inheritdoc/>
	public Grid Solution { get; set; }

	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(35, DisplayingLevel.E)
	{
		EnabledAreas = EnabledAreas.Default,
		DisabledReason = DisabledReason.LastResort
	};


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		if (Solution.IsUndefined)
		{
			goto ReturnNull;
		}

		foreach (int offset in TryAndErrorOrder)
		{
			if (grid.GetStatus(offset) == CellStatus.Empty)
			{
				int cand = offset * 9 + Solution[offset];
				var step = new BruteForceStep(
					ImmutableArray.Create(new Conclusion(ConclusionType.Assignment, cand)),
					ImmutableArray.Create(new PresentationData
					{
						Candidates = new[] { (cand, (ColorIdentifier)0) }
					})
				);
				if (onlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);
			}
		}

	ReturnNull:
		return null;
	}
}
