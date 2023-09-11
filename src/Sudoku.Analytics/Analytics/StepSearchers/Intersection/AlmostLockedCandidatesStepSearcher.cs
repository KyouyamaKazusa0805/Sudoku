using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.Steps;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;
using static Sudoku.Concepts.Intersection;

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with an <b>Almost Locked Candidates</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Almost Locked Pair</item>
/// <item>Almost Locked Triple</item>
/// <item>Almost Locked Quadruple (Maybe unnecessary)</item>
/// </list>
/// </summary>
[StepSearcher(Technique.AlmostLockedPair, Technique.AlmostLockedTriple, Technique.AlmostLockedQuadruple)]
public sealed partial class AlmostLockedCandidatesStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates whether the user checks the almost locked quadruple.
	/// </summary>
	[RuntimeIdentifier(RuntimeIdentifier.CheckAlmostLockedQuadruple)]
	public bool CheckAlmostLockedQuadruple { get; set; }


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		for (var size = 2; size <= (CheckAlmostLockedQuadruple ? 4 : 3); size++)
		{
			foreach (var ((baseSet, coverSet), (a, b, c, _)) in IntersectionMaps)
			{
				if (c && EmptyCells)
				{
					if (Collect(ref context, size, baseSet, coverSet, a, b, c) is { } step1)
					{
						return step1;
					}
					if (Collect(ref context, size, coverSet, baseSet, b, a, c) is { } step2)
					{
						return step2;
					}
				}
			}
		}

		return null;
	}

	/// <summary>
	/// <inheritdoc cref="Collect(ref AnalysisContext)" path="/summary"/>
	/// </summary>
	/// <param name="context"><inheritdoc cref="Collect(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <param name="size">The size.</param>
	/// <param name="baseSet">The base set.</param>
	/// <param name="coverSet">The cover set.</param>
	/// <param name="a">The left grid map.</param>
	/// <param name="b">The right grid map.</param>
	/// <param name="c">The intersection.</param>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="/g/developer-notes" />
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
	private static AlmostLockedCandidatesStep? Collect(
		scoped ref AnalysisContext context,
		int size,
		House baseSet,
		House coverSet,
		scoped in CellMap a,
		scoped in CellMap b,
		scoped in CellMap c
	)
	{
		scoped ref readonly var grid = ref context.Grid;

		// Iterate on each cell combination.
		foreach (var alsCells in (a & EmptyCells).GetSubsets(size - 1))
		{
			// Gather the mask. The cell combination must contain the specified number of digits.
			var mask = grid[alsCells];
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
			var ahsMask = (Mask)0;
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
				if (!alsCells.Contains(aCell))
				{
					foreach (var digit in (Mask)(mask & grid.GetCandidates(aCell)))
					{
						conclusions.Add(new(Elimination, aCell, digit));
					}
				}
			}
			foreach (var digit in (Mask)(Grid.MaxCandidatesMask & ~mask))
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
				foreach (var cell in alsCells & CandidatesMap[digit])
				{
					candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
				}
			}
			foreach (var cell in c)
			{
				foreach (var digit in (Mask)(mask & grid.GetCandidates(cell)))
				{
					candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + digit));
				}
			}
			foreach (var cell in ahsCells)
			{
				foreach (var digit in (Mask)(mask & grid.GetCandidates(cell)))
				{
					candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
				}
			}

			var babaGroupingNodes = new List<BabaGroupViewNode>(alsCells.Count + ahsCells.Count);
			var character = (Utf8Char)'a';
			foreach (var cell in alsCells)
			{
				babaGroupingNodes.Add(new(WellKnownColorIdentifier.Normal, cell, character++, grid.GetCandidates(cell)));
			}

			character = (Utf8Char)'a';
			foreach (var cell in ahsCells)
			{
				babaGroupingNodes.Add(new(WellKnownColorIdentifier.Normal, cell, character++, grid.GetCandidates(cell)));
			}

			foreach (var cell in c & EmptyCells)
			{
				babaGroupingNodes.Add(new(WellKnownColorIdentifier.Normal, cell, (Utf8Char)(char)('a' - 1 + size), grid.GetCandidates(cell)));
			}

			scoped var valueCells =
				from cell in (alsCells | ahsCells) - EmptyCells
				select new CellViewNode(WellKnownColorIdentifier.Normal, cell);
			var step = new AlmostLockedCandidatesStep(
				[.. conclusions],
				[
					[
						.. valueCells,
						.. candidateOffsets,
						new HouseViewNode(WellKnownColorIdentifier.Normal, baseSet),
						new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, coverSet)
					],
					[
						.. valueCells,
						.. babaGroupingNodes,
						new HouseViewNode(WellKnownColorIdentifier.Normal, baseSet),
						new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, coverSet)
					]
				],
				mask,
				alsCells,
				ahsCells,
				valueCells.Length != 0
			);

			if (context.OnlyFindOne)
			{
				return step;
			}

			context.Accumulator.Add(step);
		}

		return null;
	}
}
