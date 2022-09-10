namespace Sudoku.Solving.Implementations.Searchers;

[StepSearcher]
internal sealed unsafe partial class AlmostLockedCandidatesStepSearcher : IAlmostLockedCandidatesStepSearcher
{
	/// <inheritdoc/>
	[StepSearcherProperty]
	public bool CheckAlmostLockedQuadruple { get; set; }

	/// <inheritdoc/>
	bool IAlmostLockedCandidatesStepSearcher.CheckForValues { get; set; } = false;


	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		for (int size = 2, maxSize = CheckAlmostLockedQuadruple ? 4 : 3; size <= maxSize; size++)
		{
			foreach (var ((baseSet, coverSet), (a, b, c, _)) in IntersectionMaps)
			{
				if (c & EmptyCells)
				{
					if (GetAll(accumulator, grid, size, baseSet, coverSet, a, b, c, onlyFindOne) is { } step1)
					{
						return step1;
					}
					if (GetAll(accumulator, grid, size, coverSet, baseSet, b, a, c, onlyFindOne) is { } step2)
					{
						return step2;
					}
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Process the calculation.
	/// </summary>
	/// <param name="result">The result.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="size">The size.</param>
	/// <param name="baseSet">The base set.</param>
	/// <param name="coverSet">The cover set.</param>
	/// <param name="a">The left grid map.</param>
	/// <param name="b">The right grid map.</param>
	/// <param name="c">The intersection.</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searching for one step is okay.</param>
	/// <remarks>
	/// <para>
	/// The diagrams:
	/// <code><![CDATA[
	/// ALP:
	/// abx aby | ab
	/// abz     |
	///
	/// ALT:
	/// abcw abcx | abc abc
	/// abcy abcz |
	/// ]]></code>
	/// </para>
	/// <para>Algorithm:</para>
	/// <para>
	/// If the cell <c>ab</c> (in ALP) or <c>abc</c> (in ALT) is filled with the digit <c>p</c>,
	/// then the cells <c>abx</c> and <c>aby</c> (in ALP) and <c>abcw</c> and <c>abcx</c> (in ALT) can't
	/// fill the digit <c>p</c>. Therefore the digit <c>p</c> can only be filled into the left-side block.
	/// </para>
	/// <para>
	/// If the block only contains those cells that can contain the digit <c>p</c>, the ALP or ALT will be formed,
	/// and the elimination is <c>z</c> (in ALP) and <c>y</c> and <c>z</c> (in ALT).
	/// </para>
	/// </remarks>
	private static IStep? GetAll(
		ICollection<IStep> result, scoped in Grid grid, int size, int baseSet, int coverSet,
		scoped in CellMap a, scoped in CellMap b, scoped in CellMap c, bool onlyFindOne)
	{
		// Iterate on each cell combination.
		foreach (var cells in a & EmptyCells & size - 1)
		{
			// Gather the mask. The cell combination must contain the specified number of digits.
			var mask = grid.GetDigitsUnion(cells);
			if (PopCount((uint)mask) != size)
			{
				continue;
			}

			// Check whether overlapped.
			var isOverlapped = false;
			foreach (var digit in mask)
			{
				if (ValuesMap[digit] & HousesMap[coverSet])
				{
					isOverlapped = true;
					break;
				}
			}
			if (isOverlapped)
			{
				continue;
			}

			// Then check whether the another house (left-side block in those diagrams)
			// forms an AHS (i.e. those digits must appear in the specified cells).
			short ahsMask = 0;
			foreach (var digit in mask)
			{
				ahsMask |= (HousesMap[coverSet] & CandidatesMap[digit] & b) / coverSet;
			}
			if (PopCount((uint)ahsMask) != size - 1)
			{
				continue;
			}

			// Gather the AHS cells.
			var ahsCells = CellMap.Empty;
			foreach (var pos in ahsMask)
			{
				ahsCells.Add(HouseCells[coverSet][pos]);
			}

			// Gather all eliminations.
			var conclusions = new List<Conclusion>();
			foreach (var aCell in a)
			{
				if (cells.Contains(aCell))
				{
					continue;
				}

				foreach (var digit in (short)(mask & grid.GetCandidates(aCell)))
				{
					conclusions.Add(new(Elimination, aCell, digit));
				}
			}
			foreach (var digit in (short)(Grid.MaxCandidatesMask & ~mask))
			{
				foreach (var ahsCell in ahsCells & CandidatesMap[digit])
				{
					conclusions.Add(new(Elimination, ahsCell, digit));
				}
			}

			// Check whether any eliminations exists.
			if (conclusions.Count == 0)
			{
				continue;
			}

			// Gather highlight candidates.
			var candidateOffsets = new List<CandidateViewNode>();
			foreach (var digit in mask)
			{
				foreach (var cell in cells & CandidatesMap[digit])
				{
					candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
				}
			}
			foreach (var cell in c)
			{
				foreach (var digit in (short)(mask & grid.GetCandidates(cell)))
				{
					candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
				}
			}
			foreach (var cell in ahsCells)
			{
				foreach (var digit in (short)(mask & grid.GetCandidates(cell)))
				{
					candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
				}
			}

			var map = (cells | ahsCells) - EmptyCells;
			var valueCells = new List<CellViewNode>(map.Count);
			foreach (var cell in map)
			{
				valueCells.Add(new(DisplayColorKind.Normal, cell));
			}

			var hasValueCell = valueCells.Count != 0;
			var step = new AlmostLockedCandidatesStep(
				ImmutableArray.CreateRange(conclusions),
				ImmutableArray.Create(
					View.Empty
						| (hasValueCell ? valueCells : null)
						| candidateOffsets
						| new HouseViewNode[]
						{
							new(DisplayColorKind.Normal, baseSet),
							new(DisplayColorKind.Auxiliary2, coverSet)
						}
				),
				mask,
				cells,
				ahsCells,
				hasValueCell
			);

			if (onlyFindOne)
			{
				return step;
			}

			result.Add(step);
		}

		return null;
	}
}
