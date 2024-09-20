namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Rotating Deadly Pattern</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Rotating Deadly Pattern</item>
/// </list>
/// </summary>
[StepSearcher(
	"StepSearcherName_RotatingDeadlyPatternStepSearcher",
	Technique.RotatingDeadlyPattern,
	SupportedSudokuTypes = SudokuType.Standard,
	SupportAnalyzingMultipleSolutionsPuzzle = false)]
public sealed partial class RotatingDeadlyPatternStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates the patterns.
	/// </summary>
	private static readonly CellMap[] Patterns;

	/// <summary>
	/// Indicates the iterator values for split chutes.
	/// </summary>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="g/requires-static-constructor-invocation" />
	/// </remarks>
	private static readonly int[][] ChuteIteratorValues = [
		[0, 3, 6], [0, 3, 7], [0, 3, 8], [0, 4, 6], [0, 4, 7], [0, 4, 8], [0, 5, 6], [0, 5, 7], [0, 5, 8],
		[1, 3, 6], [1, 3, 7], [1, 3, 8], [1, 4, 6], [1, 4, 7], [1, 4, 8], [1, 5, 6], [1, 5, 7], [1, 5, 8],
		[2, 3, 6], [2, 3, 7], [2, 3, 8], [2, 4, 6], [2, 4, 7], [2, 4, 8], [2, 5, 6], [2, 5, 7], [2, 5, 8]
	];


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static RotatingDeadlyPatternStepSearcher()
	{
		var result = new CellMap[162];
		var length = ChuteIteratorValues.Length;
		var n = 0;
		for (var i = 0; i < 3; i++)
		{
			for (var j = 0; j < length; j++)
			{
				var a = ChuteIteratorValues[j][0] + i * 27;
				var b = ChuteIteratorValues[j][1] + i * 27;
				var c = ChuteIteratorValues[j][2] + i * 27;
				result[n++] = [a, b, c, a + 9, b + 9, c + 9, a + 18, b + 18, c + 18];
			}
		}

		for (var i = 0; i < 3; i++)
		{
			for (var j = 0; j < length; j++)
			{
				var a = ChuteIteratorValues[j][0] * 9;
				var b = ChuteIteratorValues[j][1] * 9;
				var c = ChuteIteratorValues[j][2] * 9;
				result[n++] = [
					a + 3 * i, b + 3 * i, c + 3 * i,
					a + 1 + 3 * i, b + 1 + 3 * i, c + 1 + 3 * i,
					a + 2 + 3 * i, b + 2 + 3 * i, c + 2 + 3 * i
				];
			}
		}

		Patterns = result;
	}


	/// <inheritdoc/>
	protected internal override Step? Collect(ref StepAnalysisContext context)
	{
		// Test example (not exaustive):
		// 17..9..83389......6..38..7..931287..8.1673...72695431891...2.37.3....29.26..39.51

		ref readonly var grid = ref context.Grid;

		// Iterate on each pattern. Here the pattern cells is same as patterns used in Unique Matrix pattern.
		// We should remove one cell from the pattern, to make the pattern become a possible Rotating Deadly Pattern.
		foreach (ref readonly var cells in Patterns.AsReadOnlySpan())
		{
			// Iterate on each cell as missing cell.
			foreach (var missingCell in cells)
			{
				// Check whether all last 8 cells are empty cells.
				var pattern = cells - missingCell & EmptyCells;
				if (pattern.Count != 8)
				{
					continue;
				}

				// Now we should reserve a cell as the target cell, and collect all digits appeared from the other 7 cells,
				// to get a union.
				foreach (var targetCell in pattern)
				{
					var theOtherCells = pattern - targetCell;
					var digitsMask = grid[in theOtherCells];

					// Determine whether there are only 4 kinds of digits appeared in the pattern.
					if (Mask.PopCount(digitsMask) != 4)
					{
						continue;
					}

					// By removing all other digits except such four digits, the target cell will become a possible deadly pattern.
					// Now we should extract the pattern to check whether it is a deadly pattern.
					var copied = Grid.Empty;
					foreach (var cell in pattern)
					{
						copied.SetMask(
							cell,
							cell != targetCell ? grid[cell] : (Mask)(Grid.EmptyMask | digitsMask & grid.GetCandidates(cell))
						);
					}

					// Check whether the pattern is a real deadly pattern.
					if (!DeadlyPatternInferrer.TryInfer(in copied, out var result) || !result.IsDeadlyPattern)
					{
						continue;
					}

					// If so, we can conclude that the target cell must hold at least one digit in extra digits set.
					// Collect view nodes and step information.
					var candidateOffsets = new List<CandidateViewNode>();
					foreach (var cell in theOtherCells)
					{
						foreach (var digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + digit));
						}
					}

					var removableDigitsMask = (Mask)(grid.GetCandidates(targetCell) & digitsMask);
					var step = new RotatingDeadlyPatternStep(
						[.. from digit in removableDigitsMask select new Conclusion(Elimination, targetCell, digit)],
						[[.. candidateOffsets]],
						context.Options,
						digitsMask,
						in pattern
					);
					if (context.OnlyFindOne)
					{
						return step;
					}

					context.Accumulator.Add(step);
				}
			}
		}
		return null;
	}
}
