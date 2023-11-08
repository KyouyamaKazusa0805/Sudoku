using System.Numerics;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.Steps;
using Sudoku.Concepts;
using Sudoku.Linq;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.CachedFields;
using static Sudoku.Analytics.ConclusionType;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with an <b>XYZ-Ring</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>XYZ-Ring Type 1</item>
/// <item>XYZ-Ring Type 2</item>
/// </list>
/// </summary>
[StepSearcher(Technique.XyzRingType1, Technique.XyzRingType2)]
public sealed partial class XyzRingStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="/g/developer-notes" />
	/// The pattern is nearly equals an XYZ-Wing + a conjugate pair.
	/// <code><![CDATA[
	/// abc.  .  | ab .  .
	/// .  .  .  | .  .  .
	/// .  .  .  | .  .  .
	/// ---------+--------
	/// ac .  .  | .  .  .
	/// .  .  .  | .  .  .
	/// .  .  a  | a  .  . ← conjugate pair of 'a'
	/// ]]></code>
	/// Or
	/// <code><![CDATA[
	/// abc.  .  | ab .  .
	/// .  .  .  | .  .  .
	/// .  .  .  | .  .  .
	/// ---------+--------
	/// ac .  .  | .  .  .
	/// .  .  .  | .  .  .
	/// a  .  .  | a  .  . ← conjugate pair of 'a'
	/// ]]></code>
	/// </remarks>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;

		// The pattern starts with a tri-value cell, so check for it.
		var trivalueCells = CellMap.Empty;
		foreach (var cell in EmptyCells)
		{
			if (PopCount((uint)grid.GetCandidates(cell)) == 3)
			{
				trivalueCells.Add(cell);
			}
		}

		// Iterate on each pivot cell to get all possible results.
		foreach (var pivot in trivalueCells)
		{
			var digitsMaskPivot = grid.GetCandidates(pivot);

			// Fetch for two cells from two different houses.
			foreach (var housePair in HouseTypes.GetSubsets(2))
			{
				var house1 = pivot.ToHouseIndex(housePair[0]);
				var house2 = pivot.ToHouseIndex(housePair[1]);
				var bivalueCellsFromHouse1 = BivalueCells & HousesMap[house1];
				var bivalueCellsFromHouse2 = BivalueCells & HousesMap[house2];
				if (!bivalueCellsFromHouse1 || !bivalueCellsFromHouse2)
				{
					continue;
				}

				foreach (var leafCell1 in bivalueCellsFromHouse1)
				{
					var digitsMask1 = grid.GetCandidates(leafCell1);
					foreach (var leafCell2 in bivalueCellsFromHouse2)
					{
						if ((CellsMap[pivot] + leafCell1 + leafCell2).InOneHouse(out _))
						{
							continue;
						}

						var digitsMask2 = grid.GetCandidates(leafCell2);

						// Check whether 3 cells intersected by one common digit, and contains 3 different digits.
						var unionedDigitsMask = (Mask)((Mask)(digitsMaskPivot | digitsMask1) | digitsMask2);
						if (PopCount((uint)unionedDigitsMask) != 3
							|| unionedDigitsMask != digitsMaskPivot
							|| !IsPow2((Mask)(digitsMaskPivot & digitsMask1 & digitsMask2)))
						{
							continue;
						}

						var intersectedDigit = Log2((uint)(Mask)(digitsMaskPivot & digitsMask1 & digitsMask2));
						var theOtherTwoDigitsMask = (Mask)(unionedDigitsMask & ~(1 << intersectedDigit));
						var theOtherDigit1 = TrailingZeroCount(theOtherTwoDigitsMask);
						var theOtherDigit2 = theOtherTwoDigitsMask.GetNextSet(theOtherDigit1);
						var coveringHouseForDigit1 = (digitsMask1 >> theOtherDigit1 & 1) != 0 ? house1 : house2;
						var coveringHouseForDigit2 = house1 == coveringHouseForDigit1 ? house2 : house1;
						var leafCellContainingDigit1 = (digitsMask1 >> theOtherDigit1 & 1) != 0 ? leafCell1 : leafCell2;
						var leafCellContainingDigit2 = (digitsMask2 >> theOtherDigit2 & 1) != 0 ? leafCell2 : leafCell1;

						// Iterate houses that can form a conjugate pair of digit 'a'.
						foreach (var (houseLinkedWithConjugatePair, touchedLeaf, untouchedLeaf) in (
							(leafCell1.ToHouseIndex(HouseType.Block), leafCell1, leafCell2),
							(leafCell1.ToHouseIndex(HouseType.Row), leafCell1, leafCell2),
							(leafCell1.ToHouseIndex(HouseType.Column), leafCell1, leafCell2),
							(leafCell2.ToHouseIndex(HouseType.Block), leafCell2, leafCell1),
							(leafCell2.ToHouseIndex(HouseType.Row), leafCell2, leafCell1),
							(leafCell2.ToHouseIndex(HouseType.Column), leafCell2, leafCell1)
						))
						{
							foreach (var start in
								(CandidatesMap[intersectedDigit] & HousesMap[houseLinkedWithConjugatePair]) - touchedLeaf - pivot)
							{
								foreach (var conjugatePairHouseType in HouseTypes)
								{
									var conjugatePairHouse = start.ToHouseIndex(conjugatePairHouseType);
									if (conjugatePairHouse == coveringHouseForDigit1 || conjugatePairHouse == coveringHouseForDigit2)
									{
										continue;
									}

									var conjugatePairCells = CandidatesMap[intersectedDigit] & HousesMap[conjugatePairHouse];
									if (conjugatePairCells.Count != 2
										|| conjugatePairCells.Contains(leafCell1) || conjugatePairCells.Contains(leafCell2))
									{
										continue;
									}

									// A conjugate pair is formed. Now check for the other cell (end cell),
									// to determine whether the cell shares with a same house with the other cell defined in XYZ-Wing pattern.
									var end = (conjugatePairCells - start)[0];
									if (!(CellsMap[untouchedLeaf] + end).InOneHouse(out var untouchedHouse))
									{
										continue;
									}

									// Now a ring is formed. Now check for eliminations.
									var conclusions = new List<Conclusion>();
									foreach (var cell in (HousesMap[coveringHouseForDigit1] & CandidatesMap[theOtherDigit1])
										- pivot - leafCellContainingDigit1 - start - end)
									{
										conclusions.Add(new(Elimination, cell, theOtherDigit1));
									}
									foreach (var cell in (HousesMap[coveringHouseForDigit2] & CandidatesMap[theOtherDigit2])
										- pivot - leafCellContainingDigit2 - start - end)
									{
										conclusions.Add(new(Elimination, cell, theOtherDigit2));
									}

									(CellsMap[pivot] + touchedLeaf).InOneHouse(out var pivotAndTouchedCellHouse);
									foreach (var cell in (HousesMap[pivotAndTouchedCellHouse] & HousesMap[houseLinkedWithConjugatePair] & CandidatesMap[intersectedDigit])
										- pivot - leafCell1 - leafCell2 - start - end)
									{
										conclusions.Add(new(Elimination, cell, intersectedDigit));
									}

									bool isType2;
									if (isType2 = (CellsMap[pivot] + touchedLeaf + start).InOneHouse(out _))
									{
										// Extra eliminations will be appended.
										foreach (var cell in (HousesMap[houseLinkedWithConjugatePair] & CandidatesMap[intersectedDigit])
											- pivot - leafCell1 - leafCell2 - start - end)
										{
											conclusions.Add(new(Elimination, cell, intersectedDigit));
										}
										foreach (var cell in (HousesMap[untouchedHouse] & CandidatesMap[intersectedDigit])
											- pivot - leafCell1 - leafCell2 - start - end)
										{
											conclusions.Add(new(Elimination, cell, intersectedDigit));
										}
									}

									if (conclusions.Count == 0)
									{
										continue;
									}

									var step = new XyzRingStep(
										[.. conclusions],
										[
											[
												..
												from digit in digitsMaskPivot
												let colorIdentifier = digit == intersectedDigit
													? WellKnownColorIdentifier.Auxiliary1
													: WellKnownColorIdentifier.Normal
												select new CandidateViewNode(colorIdentifier, pivot * 9 + digit),
												..
												from digit in digitsMask1
												let colorIdentifier = digit == intersectedDigit
													? WellKnownColorIdentifier.Auxiliary1
													: WellKnownColorIdentifier.Normal
												select new CandidateViewNode(colorIdentifier, leafCell1 * 9 + digit),
												..
												from digit in digitsMask2
												let colorIdentifier = digit == intersectedDigit
													? WellKnownColorIdentifier.Auxiliary1
													: WellKnownColorIdentifier.Normal
												select new CandidateViewNode(colorIdentifier, leafCell2 * 9 + digit),
												new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, start * 9 + intersectedDigit),
												new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, end * 9 + intersectedDigit),
											]
										],
										context.PredefinedOptions,
										pivot,
										leafCell1,
										leafCell2,
										new(start, end, intersectedDigit),
										isType2
									);
									if (context.OnlyFindOne)
									{
										return step;
									}

									context.Accumulator.Add(step);
								}
							}
						}
					}
				}
			}
		}

		return null;
	}
}
