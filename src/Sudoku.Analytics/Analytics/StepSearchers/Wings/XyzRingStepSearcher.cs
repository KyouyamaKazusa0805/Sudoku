namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with an <b>XYZ-Ring</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Non-Nice XYZ-Ring</item>
/// <item>Nice XYZ-Ring</item>
/// <item>Grouped Non-Nice XYZ-Ring</item>
/// <item>Grouped Nice XYZ-Ring</item>
/// </list>
/// </summary>
[StepSearcher(
	Technique.XyzLoop, Technique.XyzNiceLoop, Technique.GroupedXyzLoop, Technique.GroupedXyzNiceLoop,
	Technique.SiameseXyzLoop, Technique.SiameseXyzNiceLoop, Technique.SiameseGroupedXyzLoop, Technique.SiameseGroupedXyzNiceLoop)]
[StepSearcherRuntimeName("StepSearcherName_XyzRingStepSearcher")]
public sealed partial class XyzRingStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates whether the step searcher allows searching for Siamese XYZ-Rings.
	/// </summary>
	[RuntimeIdentifier(RuntimeIdentifier.AllowSiameseXyzRing)]
	public bool AllowSiamese { get; set; }


	/// <inheritdoc/>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="/g/developer-notes" />
	/// The pattern is formed by an XYZ-Wing and a conjugate pair.
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
		var accumulator = new List<XyzRingStep>();
		CollectCore(accumulator, in grid, in context);

		if (accumulator.Count == 0)
		{
			return null;
		}

		// Check for Siamese XYZ-Rings.
		scoped var siameses = AllowSiamese ? Siamese.GetSiamese(accumulator, in grid) : [];
		if (context.OnlyFindOne)
		{
			return siameses is [var siamese, ..] ? siamese : accumulator is [var normal, ..] ? normal : null;
		}

		if (siameses.Length != 0)
		{
			foreach (var step in siameses)
			{
				context.Accumulator.Add(step);
			}
		}
		if (accumulator.Count != 0)
		{
			context.Accumulator.AddRange(accumulator);
		}

		return null;
	}

	/// <summary>
	/// The core method to collect steps.
	/// </summary>
	/// <param name="accumulator">The accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	private void CollectCore(List<XyzRingStep> accumulator, scoped ref readonly Grid grid, scoped ref readonly AnalysisContext context)
	{
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

						// Iterate on all houses, determining whether two leaf cells can make the house
						// out of filling with intersected digit.
						for (var conflictedHouse = 0; conflictedHouse < 27; conflictedHouse++)
						{
							var cellsShouldBeCovered = HousesMap[conflictedHouse] & CandidatesMap[intersectedDigit];
							if (!cellsShouldBeCovered || !!(cellsShouldBeCovered & (CellsMap[pivot] + leafCell1 + leafCell2)))
							{
								continue;
							}

							if (((CellsMap[leafCell1] + leafCell2).ExpandedPeers & cellsShouldBeCovered) != cellsShouldBeCovered)
							{
								continue;
							}

							// An XYZ-Ring is formed. Now check for eliminations.
							var patternCells = cellsShouldBeCovered + pivot + leafCell1 + leafCell2;
							var conclusions = new List<Conclusion>();

							// Elim zone 1: Sharing house for pivot and leaf cell 1 -> eliminate digit they both hold (not intersected digit).
							foreach (var cell in (HousesMap[coveringHouseForDigit1] & CandidatesMap[theOtherDigit1]) - patternCells - cellsShouldBeCovered)
							{
								conclusions.Add(new(Elimination, cell, theOtherDigit1));
							}

							// Elim zone 2: Sharing house for pivot and leaf cell 2 -> eliminate digit they both hold (not intersected digit).
							foreach (var cell in (HousesMap[coveringHouseForDigit2] & CandidatesMap[theOtherDigit2]) - patternCells - cellsShouldBeCovered)
							{
								conclusions.Add(new(Elimination, cell, theOtherDigit2));
							}

							var isType2 = false;
							foreach (var (leaf, theOtherLeaf) in ((leafCell1, leafCell2), (leafCell2, leafCell1)))
							{
								var linkCellsIntersected = cellsShouldBeCovered & PeersMap[leaf];
								foreach (var linkCellHouse in linkCellsIntersected.CoveredHouses)
								{
									foreach (var leafCellHouse in (CellsMap[pivot] + leaf).CoveredHouses)
									{
										if (linkCellHouse.ToHouseType() == HouseType.Block ^ leafCellHouse.ToHouseType() == HouseType.Block
											&& (HousesMap[linkCellHouse] & HousesMap[leafCellHouse]) is var i)
										{
											// Elim zone 3: Intersected cell for the leaf and one grouped node of cells in (grouped) strong link
											// that they shares in a same mini-line -> eliminate intersected digit.
											foreach (var cell in (i & CandidatesMap[intersectedDigit]) - patternCells - cellsShouldBeCovered)
											{
												conclusions.Add(new(Elimination, cell, intersectedDigit));
											}
										}

										if ((HousesMap[leafCellHouse] & linkCellsIntersected) == linkCellsIntersected)
										{
											// Type 2 is checked.
											isType2 = true;

											// Elim zone 4 and 5: shared houses for leaf and (grouped) strong link nodes.
											foreach (var cell in (HousesMap[leafCellHouse] & CandidatesMap[intersectedDigit])
												- pivot - cellsShouldBeCovered - leaf)
											{
												conclusions.Add(new(Elimination, cell, intersectedDigit));
											}

											var lastCellsToCheck = cellsShouldBeCovered - linkCellsIntersected + theOtherLeaf;
											foreach (var house in lastCellsToCheck.CoveredHouses)
											{
												foreach (var cell in HousesMap[house] & CandidatesMap[intersectedDigit] - lastCellsToCheck)
												{
													conclusions.Add(new(Elimination, cell, intersectedDigit));
												}
											}
										}
									}
								}
							}
							if (conclusions.Count == 0)
							{
								continue;
							}

							accumulator.Add(
								new(
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
											..
											from cell in cellsShouldBeCovered
											select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, cell * 9 + intersectedDigit),
										]
									],
									context.PredefinedOptions,
									intersectedDigit,
									pivot,
									leafCell1,
									leafCell2,
									1 << conflictedHouse,
									isType2,
									cellsShouldBeCovered.Count > 2
								)
							);
						}
					}
				}
			}
		}
	}
}
