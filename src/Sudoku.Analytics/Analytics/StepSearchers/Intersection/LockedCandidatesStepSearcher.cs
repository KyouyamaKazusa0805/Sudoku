namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Locked Candidates</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Pointing</item>
/// <item>Claiming</item>
/// </list>
/// </summary>
[StepSearcher(Technique.Pointing, Technique.Claiming)]
[StepSearcherRuntimeName("StepSearcherName_LockedCandidatesStepSearcher")]
public sealed partial class LockedCandidatesStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="/g/developer-notes" />
	/// <para>The main idea of this searching operation:</para>
	/// <para>
	/// <code><![CDATA[
	/// .-------.-------.-------.
	/// | C C C | A A A | A A A |
	/// | B B B | . . . | . . . |
	/// | B B B | . . . | . . . |
	/// '-------'-------'-------'
	/// ]]></code>
	/// For example, if the cells C form a locked candidates, there'll be two cases:
	/// <list type="number">
	/// <item><b>Pointing (Type 1)</b>: Cells A contains the digit, but cells B doesn't.</item>
	/// <item><b>Claiming (Type 2)</b>: Cells B contains the digit, but cells A doesn't.</item>
	/// </list>
	/// </para>
	/// <para>
	/// <para>Therefore, the algorithm is:</para>
	/// Use bitwise-or <c>operator |</c> to gather all candidate masks from cells A, cells B and cells C,
	/// and suppose the notation <c>a</c> is the mask result for cells A, <c>b</c> is the mask result for cells B,
	/// and <c>c</c> is the mask result for cells C. If the equation <c><![CDATA[(c & (a ^ b)) != 0]]></c>
	/// is correct, the locked candidates exists, and the result of the expression
	/// <c><![CDATA[c & (a ^ b)]]></c> is a mask that holds the digits of the locked candidates.
	/// </para>
	/// <para>
	/// Why this expression? <c>a ^ b</c> means the digit can only appear in either cells A or cells B.
	/// If both or neither, the digit won't contain the locked candidates pattern.
	/// Because of the optimization of the performance, we use the predefined table to iterate on
	/// all possible location where may form a locked candidates.
	/// </para>
	/// </remarks>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		foreach (var ((baseSet, coverSet), (a, b, c, _)) in IntersectionMaps)
		{
			// If the cells C doesn't contain any empty cells,
			// the location won't contain any locked candidates.
			if (!(EmptyCells & c))
			{
				continue;
			}

			// Gather the masks in cells A, B and C.
			var (maskA, maskB, maskC) = (grid[in a], grid[in b], grid[in c]);

			// Use the formula, and check whether the equation is correct.
			// If so, the mask 'm' will hold the digits that form locked candidates structures.
			var m = (Mask)(maskC & (maskA ^ maskB));
			if (m == 0)
			{
				continue;
			}

			// Now iterate on the mask to get all digits.
			foreach (var digit in m)
			{
				scoped ref readonly var candidatesMap = ref CandidatesMap[digit];

				// Check whether the digit contains any eliminations.
				var (housesMask, elimMap) = a & candidatesMap
					? ((Mask)(coverSet << 8 | baseSet), a & candidatesMap)
					: ((Mask)(baseSet << 8 | coverSet), b & candidatesMap);
				if (!elimMap)
				{
					continue;
				}

				// Okay, now put the current step into the collection.
				var (realBaseSet, realCoverSet, intersection) = (housesMask >> 8 & 127, housesMask & 127, c & candidatesMap);
				var step = new LockedCandidatesStep(
					[.. from cell in elimMap select new Conclusion(Elimination, cell, digit)],
					[
						[
							.. from cell in intersection select new CandidateViewNode(WellKnownColorIdentifier.Normal, cell * 9 + digit),
							new HouseViewNode(WellKnownColorIdentifier.Normal, realBaseSet),
							new HouseViewNode(WellKnownColorIdentifier.Auxiliary1, realCoverSet),
							.. GetCrosshatchBaseCells(in grid, digit, realBaseSet, in intersection)
						]
					],
					context.PredefinedOptions,
					digit,
					realBaseSet,
					realCoverSet
				);

				if (context.OnlyFindOne)
				{
					return step;
				}

				context.Accumulator.Add(step);
			}
		}

		return null;
	}

	/// <summary>
	/// Try to create a list of <see cref="CellViewNode"/>s indicating the crosshatching base cells.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="house">The house.</param>
	/// <param name="cells">The cells.</param>
	/// <returns>A list of <see cref="CellViewNode"/> instances.</returns>
	private CellViewNode[] GetCrosshatchBaseCells(scoped ref readonly Grid grid, Digit digit, House house, scoped ref readonly CellMap cells)
	{
		var info = Crosshatching.GetCrosshatchingInfo(in grid, digit, house, in cells);
		if (info is not var (combination, emptyCellsShouldBeCovered, emptyCellsNotNeedToBeCovered))
		{
			return [];
		}

		var result = new List<CellViewNode>();
		foreach (var c in combination)
		{
			result.Add(new(WellKnownColorIdentifier.Normal, c) { RenderingMode = DirectModeOnly });
		}
		foreach (var c in emptyCellsShouldBeCovered)
		{
			var p = emptyCellsNotNeedToBeCovered.Contains(c) ? WellKnownColorIdentifier.Auxiliary2 : WellKnownColorIdentifier.Auxiliary1;
			result.Add(new(p, c) { RenderingMode = DirectModeOnly });
		}

		return [.. result];
	}
}
