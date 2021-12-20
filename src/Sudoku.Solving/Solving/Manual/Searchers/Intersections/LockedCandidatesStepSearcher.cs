namespace Sudoku.Solving.Manual.Searchers.Intersections;

/// <summary>
/// Provides with a <b>Locked Candidates</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Pointing</item>
/// <item>Claiming</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe class LockedCandidatesStepSearcher : ILockedCandidatesStepSearcher
{
	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(2, DisplayingLevel.A);


	/// <inheritdoc/>
	/// <remarks>
	/// <para>
	/// The main idea of this searching operation:
	/// </para>
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
	public unsafe Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		int* r = stackalloc int[2];
		foreach (var ((baseSet, coverSet), (a, b, c, _)) in IntersectionMaps)
		{
			// If the cells C doesn't contain any empty cells,
			// the location won't contain any locked candidates.
			if ((EmptyMap & c).IsEmpty)
			{
				continue;
			}

			// Gather the masks in cells A, B and C.
			short maskA = 0, maskB = 0, maskC = 0;
			foreach (int cell in a) maskA |= grid.GetCandidates(cell);
			foreach (int cell in b) maskB |= grid.GetCandidates(cell);
			foreach (int cell in c) maskC |= grid.GetCandidates(cell);

			// Use the formula, and check whether the equation is correct.
			// If so, the mask 'm' will hold the digits that form locked candidates structures.
			short m = (short)(maskC & (maskA ^ maskB));
			if (m == 0)
			{
				continue;
			}

			// Now iterate on the mask to get all digits.
			foreach (int digit in m)
			{
				// Check whether the digit contains any eliminations.
				Cells elimMap;
				if (!(a & CandMaps[digit]).IsEmpty)
				{
					r[0] = coverSet;
					r[1] = baseSet;
					elimMap = a & CandMaps[digit];
				}
				else
				{
					r[0] = baseSet;
					r[1] = coverSet;
					elimMap = b & CandMaps[digit];
				}
				if (elimMap.IsEmpty)
				{
					continue;
				}

				// Gather the information, such as the type of the locked candidates, the located region, etc..
				var candidateOffsets = new List<(int, ColorIdentifier)>();
				foreach (int cell in c & CandMaps[digit])
				{
					candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)0));
				}

				// Okay, now accumulate into the collection.
				var step = new LockedCandidatesStep(
					elimMap.ToImmutableConclusions(digit),
					ImmutableArray.Create(new PresentationData
					{
						Candidates = candidateOffsets,
						Regions = new[] { (r[0], (ColorIdentifier)0), (r[1], (ColorIdentifier)1) }
					}),
					digit,
					r[0],
					r[1]
				);

				if (onlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);
			}
		}

		return null;
	}
}
