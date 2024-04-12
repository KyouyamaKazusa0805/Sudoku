namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Brute Force</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Brute Force</item>
/// </list>
/// </summary>
[StepSearcher(
	"StepSearcherName_BruteForceStepSearcher",
	Technique.BruteForce,
	IsPure = true,
	IsFixed = true,
	SupportMultiple = false)]
public sealed partial class BruteForceStepSearcher : StepSearcher
{
	/// <summary>
	/// The order of cell offsets to get values.
	/// </summary>
	/// <remarks>
	/// For example, the first value is 40, which means the first cell to be tried to be filled
	/// is the 40th cell in the grid (i.e. the cell <c>r5c5</c>).
	/// </remarks>
	private static readonly Cell[] BruteForceTryAndErrorOrder = [
		40, 41, 32, 31, 30, 39, 48, 49, 50,
		51, 42, 33, 24, 23, 22, 21, 20, 29,
		38, 47, 56, 57, 58, 59, 60, 61, 52,
		43, 34, 25, 16, 15, 14, 13, 12, 11,
		10, 19, 28, 37, 46, 55, 64, 65, 66,
		67, 68, 69, 70, 71, 62, 53, 44, 35,
		26, 17, 8, 7, 6, 5, 4, 3, 2,
		1, 0, 9, 18, 27, 36, 45, 54, 63,
		72, 73, 74, 75, 76, 77, 78, 79, 80
	];


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		if (Solution.IsUndefined)
		{
			goto ReturnNull;
		}

		scoped ref readonly var grid = ref context.Grid;
		foreach (var offset in BruteForceTryAndErrorOrder)
		{
			if (grid.GetState(offset) == CellState.Empty)
			{
				var cand = offset * 9 + Solution.GetDigit(offset);
				var step = new BruteForceStep(
					[new(Assignment, cand)],
					[[new CandidateViewNode(ColorIdentifier.Normal, cand)]],
					context.PredefinedOptions
				);
				if (context.OnlyFindOne)
				{
					return step;
				}

				context.Accumulator.Add(step);
			}
		}

	ReturnNull:
		return null;
	}
}
