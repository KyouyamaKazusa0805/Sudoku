namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Locked Candidates</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Pointing</item>
/// <item>Claiming</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed partial class LockedCandidatesStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="/g/deverloper-notes" />
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
	/// If both or neither, the digit won't contain the locked candidates structure.
	/// Because of the optimization of the performance, we use the predefined table to iterate on
	/// all possible location where may form a locked candidate.
	/// </para>
	/// </remarks>
	protected internal override Step? GetAll(scoped ref AnalysisContext context)
	{
		var housesMask = (short)0;
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
			var maskA = grid.GetDigitsUnion(a);
			var maskB = grid.GetDigitsUnion(b);
			var maskC = grid.GetDigitsUnion(c);

			// Use the formula, and check whether the equation is correct.
			// If so, the mask 'm' will hold the digits that form locked candidates structures.
			var m = (short)(maskC & (maskA ^ maskB));
			if (m == 0)
			{
				continue;
			}

			// Now iterate on the mask to get all digits.
			foreach (var digit in m)
			{
				// Check whether the digit contains any eliminations.
				(housesMask, var elimMap) = a & CandidatesMap[digit]
					? ((short)(coverSet << 8 | baseSet), a & CandidatesMap[digit])
					: ((short)(baseSet << 8 | coverSet), b & CandidatesMap[digit]);
				if (!elimMap)
				{
					continue;
				}

				// Gather the information, such as the type of the locked candidates, the located house, etc..
				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var cell in c & CandidatesMap[digit])
				{
					candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
				}

				// Okay, now accumulate into the collection.
				var bs = housesMask >> 8 & 127;
				var cs = housesMask & 127;
				var step = new LockedCandidatesStep(
					from cell in elimMap select new Conclusion(Elimination, cell, digit),
					new[]
					{
						View.Empty
							| candidateOffsets
							| new HouseViewNode[] { new(DisplayColorKind.Normal, bs), new(DisplayColorKind.Auxiliary1, cs) }
					},
					digit,
					bs,
					cs
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
}
