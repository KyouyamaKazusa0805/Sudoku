namespace Sudoku.Analytics.StepSearchers;

public partial class UniqueRectangleStepSearcher
{
	/// <summary>
	/// Check burred subset.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// <para>
	/// The pattern:
	/// <code><![CDATA[
	///  ↓ corner1, corner2
	/// (abx) | (abcd) acd acd
	///  ab   |  ab
	/// ]]></code>
	/// will remove the digit <c>a</c> in the corner cell <c>abx</c>.
	/// </para>
	/// <para>
	/// The letter <c>x</c> may present multiple digits and not be used. However, if all digits <c>x</c> are false,
	/// the UR pattern will be degenerated into type 1, and the normal subset with digits <c>c</c> and <c>d</c> will be formed.
	/// </para>
	/// </remarks>
	private partial void CheckBurredSubset(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref AnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index)
	{
		// Deconstruct the other cells, determining whether they holds a same two digits.
		if (otherCellsMap is not [var c1, var c2]
			|| grid.GetCandidates(c1) is var digitsMask1 && grid.GetCandidates(c2) is var digitsMask2
			&& (digitsMask1 != comparer || digitsMask2 != comparer))
		{
			return;
		}

		// Check whether corner cells hold both digits.
		if (grid.GetCandidates(corner1) is var digitsMaskCorner1 && grid.GetCandidates(corner2) is var digitsMaskCorner2
			&& (Mask)(digitsMaskCorner1 & digitsMaskCorner2) != comparer)
		{
			return;
		}

		// Determine whether a corner holds both and only two digits.
		if (digitsMaskCorner1 == comparer || digitsMaskCorner2 == comparer)
		{
			return;
		}

		// Iterate on each combination of corner cells, determining the final cell as conclusion producer.
		foreach (var house in (corner1.AsCellMap() + corner2).SharedHouses)
		{
			var extraCells = (HousesMap[house] & EmptyCells) - corner1 - corner2;

			// Here 'size' can be started at 1.
			// Example:
			// .....+6..9.369+4..5..5.27..6..48..3.....5..7......89+4+5.6+5....+2.47.67+4.+982+52.+4+7.56..:112 812 114 115 815 217 317 318 131 252 155 359 162
			for (var size = 1; size <= extraCells.Count - 1; size++)
			{
				foreach (var (thisCorner, elimCorner) in ((corner1, corner2), (corner2, corner1)))
				{
					var otherDigits = (Mask)(grid.GetCandidates(thisCorner) & ~comparer);
					if (otherDigits == 0)
					{
						// There is not necessary to determine the pattern because 3 of 4 cells are only two digits.
						return;
					}

					foreach (ref readonly var subsetCells in extraCells & size)
					{
						// Determine whether the 'size' cells contain 'size + 1' digits.
						var subsetDigitsMask = grid[in subsetCells];
						if (PopCount((uint)subsetDigitsMask) != size + 1)
						{
							continue;
						}

						// The burred subset must contain 1 digit that is UR digit.
						var onlyDigit = (Mask)(subsetDigitsMask & comparer);
						if (!IsPow2((uint)onlyDigit))
						{
							continue;
						}

						var elimDigit = Log2((uint)onlyDigit);

						// Check whether the extra cells holds all possible digits appeared in 'thisCorner',
						// with UR digits having been removed.
						if ((subsetDigitsMask & otherDigits) != otherDigits)
						{
							continue;
						}

						// Check whether the other side 'elimCorner' contains elimination digit.
						var elimDigitsMask = (Mask)(grid.GetCandidates(elimCorner) & subsetDigitsMask);
						if (elimDigitsMask == 0)
						{
							// No elimination exists.
							continue;
						}

						var candidateOffsets = new List<CandidateViewNode>();
						foreach (var cell in otherCellsMap)
						{
							foreach (var digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + digit));
							}
						}
						foreach (var digit in grid.GetCandidates(thisCorner))
						{
							candidateOffsets.Add(
								new(
									(comparer >> digit & 1) != 0 ? ColorIdentifier.Normal : ColorIdentifier.Auxiliary1,
									thisCorner * 9 + digit
								)
							);
						}
						foreach (var digit in (Mask)(grid.GetCandidates(elimCorner) & comparer))
						{
							candidateOffsets.Add(new(ColorIdentifier.Normal, elimCorner * 9 + digit));
						}
						foreach (var cell in subsetCells)
						{
							foreach (var digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, cell * 9 + digit));
							}
						}

						accumulator.Add(
							new UniqueRectangleBurredSubsetStep(
								[new(Elimination, elimCorner, elimDigit)],
								[[.. candidateOffsets, new HouseViewNode(ColorIdentifier.Normal, house)]],
								context.Options,
								d1,
								d2,
								urCells.AsCellMap(),
								index,
								in subsetCells,
								thisCorner,
								subsetDigitsMask
							)
						);
					}
				}
			}
		}
	}

	/// <summary>
	/// Check UR-XY-Wing, UR-XYZ-Wing, UR-WXYZ-Wing and AR-XY-Wing, AR-XYZ-Wing and AR-WXYZ-Wing.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <param name="areCornerCellsAligned">Indicates whether the corner cells cannot see each other.</param>
	/// <remarks>
	/// <para>The structures:</para>
	/// <para>
	/// Subtype 1:
	/// <code><![CDATA[
	///   ↓ corner1
	/// (ab )  abxy  yz  xz
	/// (ab )  abxy  *
	///   ↑ corner2
	/// ]]></code>
	/// Note that the pair of cells <c>abxy</c> should be in the same house.
	/// </para>
	/// <para>
	/// Subtype 2:
	/// <code><![CDATA[
	///   ↓ corner1
	/// (ab )  abx   xz
	///  aby  (ab )  *   yz
	///         ↑ corner2
	/// ]]></code>
	/// </para>
	/// </remarks>
	private partial void CheckRegularWing(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref AnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index, bool areCornerCellsAligned)
	{
		// Firstly, we should check whether the 2 corner cells should contain both a and b, and only contain a and b.
		// This expression only uses candidates to check digits appearing, so it doesn't determine whether the pattern is a UR or not.
		// ARs can also be passed of course.
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		var otherCell1Mask = grid.GetCandidates(otherCellsMap[0]);
		var otherCell2Mask = grid.GetCandidates(otherCellsMap[1]);
		if ((otherCell1Mask & otherCell2Mask & comparer) == 0 || ((otherCell1Mask | otherCell2Mask) & comparer) != comparer)
		{
			return;
		}

		// Now we check for other 2 cells, collecting digits not being UR/AR digits.
		var otherDigitsMask = (Mask)((otherCell1Mask | otherCell2Mask) & ~comparer);

		// Merge incomplete and complete wing logic into one loop.
		// Here we don't know what digit will be selected as a pivot, so we should iterate all digits.
		// The last case -1 is for complete wing.
		var cells = urCells.AsCellMap();
		var pivotDigits = (Digit[])[.. otherDigitsMask.GetAllSets(), -1];
		foreach (var pivotDigit in pivotDigits)
		{
			Cell[][] cellsGroups;
			if (pivotDigit == -1)
			{
				// No pivot digit should be checked. Due to no way to check intersection, we should delay the checking.
				(cellsGroups, var tempIndex) = (new Cell[PopCount((uint)otherDigitsMask)][], 0);
				foreach (var lastDigit in otherDigitsMask)
				{
					cellsGroups[tempIndex++] = [.. cells % CandidatesMap[lastDigit] & BivalueCells];
				}
			}
			else
			{
				// Pivot digit is specified. We should check for cells that contain that digit.
				var lastDigitsMask = (Mask)(otherDigitsMask & ~(1 << pivotDigit));
				if (PopCount((uint)lastDigitsMask) is 0 or 1)
				{
					continue;
				}

				(cellsGroups, var tempIndex, var atLeastOneGroupIsEmpty) = (new Cell[PopCount((uint)lastDigitsMask)][], 0, false);
				foreach (var lastDigit in lastDigitsMask)
				{
					ref var currentCellGroup = ref cellsGroups[tempIndex++];
					currentCellGroup = [.. cells % CandidatesMap[lastDigit] & CandidatesMap[pivotDigit] & BivalueCells];
					if (currentCellGroup.Length == 0)
					{
						atLeastOneGroupIsEmpty = true;
						break;
					}
				}
				if (atLeastOneGroupIsEmpty)
				{
					// If a cells group does not contain such cells, the current pivot digit will be considered invalid.
					// Now just skip for this case.
					continue;
				}
			}

			// Extract one element for each cells group.
			var finalPivotDigit = pivotDigit;
			foreach (CellMap combination in cellsGroups.GetExtractedCombinations())
			{
				if (cellsGroups.Length != combination.Count)
				{
					// The selected items cannot duplicate with others.
					continue;
				}

				// Here we should check for pivot digit for case 'pivotDigit == -1'.
				if (pivotDigit == -1)
				{
					var mergedMask = grid[in combination, false, '&'];
					if (!IsPow2(mergedMask))
					{
						// No pivot digit can be found, meaning no eliminations can be found.
						continue;
					}

					finalPivotDigit = TrailingZeroCount(mergedMask);
				}

				var maskToCompare = pivotDigit == -1 ? grid[in combination] & ~(1 << finalPivotDigit) : grid[in combination];
				if (((otherCell1Mask | otherCell2Mask) & ~comparer) != maskToCompare)
				{
					// Digits are not matched.
					continue;
				}

				var elimMapBase = pivotDigit == -1 ? combination : combination | cells & CandidatesMap[finalPivotDigit];
				var elimMap = elimMapBase.PeerIntersection & CandidatesMap[finalPivotDigit];
				if (!elimMap)
				{
					// No eliminations.
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var cell in cells & EmptyCells)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								(comparer >> digit & 1) != 0
									? ColorIdentifier.Normal
									: digit == finalPivotDigit ? ColorIdentifier.Auxiliary2 : ColorIdentifier.Auxiliary1,
								cell * 9 + digit
							)
						);
					}
				}
				foreach (var cell in combination)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								digit != finalPivotDigit ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Auxiliary2,
								cell * 9 + digit
							)
						);
					}
				}

				var cellOffsets = new List<CellViewNode>();
				foreach (var cell in urCells)
				{
					if (!EmptyCells.Contains(cell))
					{
						cellOffsets.Add(new(ColorIdentifier.Normal, cell));
					}
				}

				if (!IsIncompleteValid(arMode, AllowIncompleteUniqueRectangles, candidateOffsets, out _))
				{
					return;
				}

				accumulator.Add(
					new UniqueRectangleRegularWingStep(
						[.. from cell in elimMap select new Conclusion(Elimination, cell, finalPivotDigit)],
						[[.. candidateOffsets, .. cellOffsets]],
						context.Options,
						(arMode, pivotDigit, combination.Count) switch
						{
							(false, -1, 2) => Technique.UniqueRectangleXyWing,
							(false, -1, 3) => Technique.UniqueRectangleXyzWing,
							(false, -1, 4) => Technique.UniqueRectangleWxyzWing,
							(false, _, 2) => Technique.UniqueRectangleXyzWing,
							(false, _, 3) => Technique.UniqueRectangleWxyzWing,
							(_, -1, 2) => Technique.AvoidableRectangleXyWing,
							(_, -1, 3) => Technique.AvoidableRectangleXyzWing,
							(_, -1, 4) => Technique.AvoidableRectangleWxyzWing,
							(_, _, 2) => Technique.AvoidableRectangleXyzWing,
							(_, _, 3) => Technique.AvoidableRectangleWxyzWing
						},
						d1,
						d2,
						in cells,
						arMode,
						in combination,
						in otherCellsMap,
						otherDigitsMask,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR-W-Wing and AR-W-Wing.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// <para>The structures:</para>
	/// <para>
	/// Subtype 1:
	/// <code><![CDATA[
	///         ↓ corner1
	/// xz   abx  (ab )
	///     (ab )  aby   yz
	///       ↑ corner2
	/// ]]></code>
	/// </para>
	/// <para>
	/// Subtype 2:
	/// <code><![CDATA[
	///       ↓ corner1
	/// xz  (ab )  abx
	///      abx  (ab )  xz
	///             ↑ corner2
	/// ]]></code>
	/// </para>
	/// <para>Please note that corner cells may be aligned as a same row or column.</para>
	/// <para>
	/// <i>Also, this method is useless because it may be replaced with another techniques such as UR-XY-Wing and UR External Type 2.</i>
	/// </para>
	/// </remarks>
	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
	private partial void CheckWWing(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref AnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index)
	{
		// Firstly, we should check whether the 2 corner cells should contain both a and b, and only contain a and b.
		// This expression only uses candidates to check digits appearing, so it doesn't determine whether the pattern is a UR or not.
		// ARs can also be passed of course.
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		var otherCell1Mask = grid.GetCandidates(otherCellsMap[0]);
		var otherCell2Mask = grid.GetCandidates(otherCellsMap[1]);
		if ((otherCell1Mask & otherCell2Mask & comparer) == 0 || ((otherCell1Mask | otherCell2Mask) & comparer) != comparer)
		{
			return;
		}

		var otherDigits1 = (Mask)(otherCell1Mask & ~comparer);
		if (!IsPow2(otherDigits1))
		{
			return;
		}
		var otherDigits2 = (Mask)(otherCell2Mask & ~comparer);
		if (!IsPow2(otherDigits2))
		{
			return;
		}

		var otherDigit1 = TrailingZeroCount(otherDigits1);
		var otherDigit2 = TrailingZeroCount(otherDigits2);

		// Now we check for other 2 cells, collecting digits not being UR/AR digits.
		var cells = urCells.AsCellMap();
		foreach (var endCell1 in PeersMap[otherCellsMap[0]] & BivalueCells & CandidatesMap[otherDigit1])
		{
			foreach (var endCell2 in (PeersMap[otherCellsMap[1]] & BivalueCells & CandidatesMap[otherDigit2]) - endCell1)
			{
				// Check whether two cells are same, or in a same house. If so, the pattern will be degenerated to a normal type 3.
				if ((endCell1.AsCellMap() + endCell2).InOneHouse(out _))
				{
					continue;
				}

				var mergedMask = (Mask)(grid.GetCandidates(endCell1) & grid.GetCandidates(endCell2));
				if (!IsPow2(mergedMask))
				{
					continue;
				}

				var wDigit = TrailingZeroCount(mergedMask);
				if (otherDigit1 == wDigit || otherDigit2 == wDigit)
				{
					continue;
				}

				var elimMap = (endCell1.AsCellMap() + endCell2).PeerIntersection & CandidatesMap[wDigit];
				if (!elimMap)
				{
					// No eliminations.
					continue;
				}

				// A W-Wing found.
				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var cell in urCells)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								digit == d1 || digit == d2 ? ColorIdentifier.Normal : ColorIdentifier.Auxiliary1,
								cell * 9 + digit
							)
						);
					}
				}
				foreach (var digit in grid.GetCandidates(endCell1))
				{
					candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, endCell1 * 9 + digit));
				}
				foreach (var digit in grid.GetCandidates(endCell2))
				{
					candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, endCell2 * 9 + digit));
				}

				if (!IsIncompleteValid(arMode, AllowIncompleteUniqueRectangles, candidateOffsets, out _))
				{
					return;
				}

				var isAvoidable = arMode && (EmptyCells & cells).Count != 4;
				accumulator.Add(
					new UniqueRectangleWWingStep(
						[.. from cell in elimMap select new Conclusion(Elimination, cell, wDigit)],
						[[.. candidateOffsets]],
						context.Options,
						isAvoidable ? Technique.AvoidableRectangleWWing : Technique.UniqueRectangleWWing,
						d1,
						d2,
						in cells,
						isAvoidable,
						wDigit,
						in otherCellsMap,
						[endCell1, endCell2],
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR + SdC.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///           |   xyz
	///  ab+ ab+  | abxyz abxyz
	///           |   xyz
	/// ----------+------------
	/// (ab)(ab)  |
	///  ↑ corner1, corner2
	/// ]]></code>
	/// </remarks>
	private partial void CheckSueDeCoq(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref AnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index)
	{
		var notSatisfiedType3 = false;
		var mergedMaskInOtherCells = (Mask)0;
		foreach (var cell in otherCellsMap)
		{
			var currentMask = grid.GetCandidates(cell);
			mergedMaskInOtherCells |= currentMask;
			if ((currentMask & comparer) == 0 || currentMask == comparer || arMode && grid.GetState(cell) != CellState.Empty)
			{
				notSatisfiedType3 = true;
				break;
			}
		}

		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer || notSatisfiedType3
			|| (mergedMaskInOtherCells & comparer) != comparer)
		{
			return;
		}

		// Check whether the corners spanned two blocks. If so, UR + SdC can't be found.
		var blockMaskInOtherCells = otherCellsMap.BlockMask;
		if (!IsPow2(blockMaskInOtherCells))
		{
			return;
		}

		var otherDigitsMask = (Mask)(mergedMaskInOtherCells & ~comparer);
		var line = (byte)otherCellsMap.SharedLine;
		var block = (byte)TrailingZeroCount(otherCellsMap.SharedHouses & ~(1 << line));
		var d = Miniline.Map[new(line, block)].OtherBlocks;
		var list = new List<CellMap>(4);
		foreach (var cannibalMode in (false, true))
		{
			foreach (var otherBlock in d)
			{
				var emptyCellsInInterMap = HousesMap[otherBlock] & HousesMap[line] & EmptyCells;
				if (emptyCellsInInterMap.Count < 2)
				{
					// The intersection needs at least two empty cells.
					continue;
				}

				var a = Miniline.Map[new(line, otherBlock)].LineMap;
				var b = HousesMap[otherBlock] & ~HousesMap[line];
				var c = a & b;

				list.Clear();
				switch (emptyCellsInInterMap)
				{
					case { Count: 2 }:
					{
						list.AddRef(in emptyCellsInInterMap);
						break;
					}
					case [var i, var j, var k]:
					{
						list.AddRef([i, j]);
						list.AddRef([j, k]);
						list.AddRef([i, k]);
						list.AddRef(in emptyCellsInInterMap);
						break;
					}
				}

				// Iterate on each intersection combination.
				foreach (var currentInterMap in list)
				{
					var selectedInterMask = grid[in currentInterMap];
					if (PopCount((uint)selectedInterMask) <= currentInterMap.Count + 1)
					{
						// The intersection combination is an ALS or a normal subset,
						// which is invalid in SdCs.
						continue;
					}

					var blockMap = (b | c & ~currentInterMap) & EmptyCells;
					var lineMap = a & EmptyCells & ~otherCellsMap;

					// Iterate on the number of the cells that should be selected in block.
					for (var i = 1; i <= blockMap.Count - 1; i++)
					{
						// Iterate on each combination in block.
						foreach (ref readonly var selectedCellsInBlock in blockMap & i)
						{
							var flag = false;
							foreach (var digit in otherDigitsMask)
							{
								foreach (var cell in selectedCellsInBlock)
								{
									if (CandidatesMap[digit].Contains(cell))
									{
										flag = true;
										break;
									}
								}
							}
							if (flag)
							{
								continue;
							}

							var (currentBlockMap, elimMapBlock, elimMapLine) = (selectedCellsInBlock, CellMap.Empty, CellMap.Empty);

							// Get the links of the block.
							var blockMask = grid[in selectedCellsInBlock];

							// Get the elimination map in the block.
							foreach (var digit in blockMask)
							{
								elimMapBlock |= CandidatesMap[digit];
							}
							elimMapBlock &= blockMap & ~currentBlockMap;

							foreach (var digit in otherDigitsMask)
							{
								elimMapLine |= CandidatesMap[digit];
							}
							elimMapLine &= lineMap & ~currentInterMap;

							checkGeneralizedSdc(
								accumulator, in grid, ref context, arMode, cannibalMode, d1, d2, urCells,
								line, otherBlock, otherDigitsMask, blockMask, selectedInterMask,
								otherDigitsMask, in elimMapLine, in elimMapBlock, in otherCellsMap, in currentBlockMap,
								in currentInterMap, i, 0, index
							);
						}
					}
				}
			}
		}


		static void checkGeneralizedSdc(
			List<UniqueRectangleStep> accumulator,
			ref readonly Grid grid,
			ref AnalysisContext context,
			bool arMode,
			bool cannibalMode,
			Digit digit1,
			Digit digit2,
			Cell[] urCells,
			House line,
			House block,
			Mask lineMask,
			Mask blockMask,
			Mask selectedInterMask,
			Mask otherDigitsMask,
			ref readonly CellMap elimMapLine,
			ref readonly CellMap elimMapBlock,
			ref readonly CellMap currentLineMap,
			ref readonly CellMap currentBlockMap,
			ref readonly CellMap currentInterMap,
			int i,
			int j,
			int index
		)
		{
			var maskOnlyInInter = (Mask)(selectedInterMask & ~(blockMask | lineMask));
			var maskIsolated = (Mask)(cannibalMode ? lineMask & blockMask & selectedInterMask : maskOnlyInInter);
			if (!cannibalMode && ((blockMask & lineMask) != 0 || maskIsolated != 0 && !IsPow2(maskIsolated))
				|| cannibalMode && !IsPow2(maskIsolated))
			{
				return;
			}

			var elimMapIsolated = CellMap.Empty;
			var digitIsolated = TrailingZeroCount(maskIsolated);
			if (digitIsolated != TrailingZeroCountFallback)
			{
				elimMapIsolated = (cannibalMode ? currentBlockMap | currentLineMap : currentInterMap)
					% CandidatesMap[digitIsolated]
					& EmptyCells;
			}

			if (currentInterMap.Count + i + j + 1 == PopCount((uint)blockMask) + PopCount((uint)lineMask) + PopCount((uint)maskOnlyInInter)
				&& !!(elimMapBlock | elimMapLine | elimMapIsolated))
			{
				// Check eliminations.
				var conclusions = new List<Conclusion>(10);
				foreach (var cell in elimMapBlock)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						if ((blockMask >> digit & 1) != 0)
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
				}
				foreach (var cell in elimMapLine)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						if ((lineMask >> digit & 1) != 0)
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
				}
				foreach (var cell in elimMapIsolated)
				{
					conclusions.Add(new(Elimination, cell, digitIsolated));
				}
				if (conclusions.Count == 0)
				{
					return;
				}

				// Record highlight candidates and cells.
				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var cell in urCells)
				{
					if (arMode && grid.GetState(cell) != CellState.Empty)
					{
						// Skip for non-empty cells.
						continue;
					}

					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								(otherDigitsMask >> digit & 1) != 0 ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal,
								cell * 9 + digit
							)
						);
					}
				}
				foreach (var cell in currentBlockMap)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								!cannibalMode && digit == digitIsolated ? ColorIdentifier.Auxiliary3 : ColorIdentifier.Auxiliary2,
								cell * 9 + digit
							)
						);
					}
				}
				foreach (var cell in currentInterMap)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								digitIsolated == digit
									? ColorIdentifier.Auxiliary3
									: (otherDigitsMask >> digit & 1) != 0 ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Auxiliary2,
								cell * 9 + digit
							)
						);
					}
				}

				accumulator.Add(
					new UniqueRectangleSueDeCoqStep(
						[.. conclusions],
						[
							[
								.. arMode ? GetHighlightCells(urCells) : [],
								.. candidateOffsets,
								new HouseViewNode(ColorIdentifier.Normal, block),
								new HouseViewNode(ColorIdentifier.Auxiliary2, line)
							]
						],
						context.Options,
						digit1,
						digit2,
						[.. urCells],
						arMode,
						block,
						line,
						blockMask,
						lineMask,
						selectedInterMask,
						cannibalMode,
						maskIsolated,
						in currentBlockMap,
						in currentLineMap,
						in currentInterMap,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR + ALS and AR + ALS.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the method searches for avoidable rectangles.</param>
	/// <param name="comparer">The comparer.</param>
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="alses">
	/// <para>The ALS patterns found.</para>
	/// <para>
	/// <include file="../../global-doc-comments.xml" path="g/csharp11/feature[@name='scoped-keyword']/para"/>
	/// </para>
	/// <para>
	/// This parameter expects a <see langword="scoped"/> keyword
	/// because <paramref name="alses"/> may assign to <paramref name="context"/>
	/// because <paramref name="context"/> expects a <see cref="ReadOnlySpan{T}"/> instance,
	/// which is a very "implicit" compiler error that we cannot aware of this at once.
	/// </para>
	/// <para>
	/// By appending modifier <see langword="scoped"/>, compiler will know that this parameter should only be used
	/// inside this method or other places allowing such <see langword="scoped"/> usages (recursive usages),
	/// which won't extend its lifecycle.
	/// </para>
	/// <para>
	/// Such usage is same as method <see cref="CheckExternalAlmostLockedSetsXz"/>.
	/// </para>
	/// <para>
	/// Please check this page to learn more information on
	/// <see href="https://blog.walterlv.com/post/cs8350-ref-arguments-combination-is-disallowed">CS8350</see>.
	/// </para>
	/// </param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// <para>
	/// The pattern:
	/// <code><![CDATA[
	///       ↓ cell1, cells2
	///  ab | abc    *
	///  ab | abcd   *
	///     |       cdef
	/// ----+------------
	///     |       def
	///     |       def
	/// ]]></code>
	/// where a cell <c>cdef</c> and 2 cells <c>def</c> will be formed an ALS pattern. Therefore, a chain is formed:
	/// <code><![CDATA[abcd(d) == {abc, abcd}(c) -- cdef(c) == {def, def}(d) => * != d]]></code>
	/// </para>
	/// <para>
	/// In addition, this pattern will cover all possible cases of UR + 2D and UR + 3X.
	/// </para>
	/// </remarks>
	private partial void CheckAlmostLockedSetsXz(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref AnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, scoped ReadOnlySpan<AlmostLockedSet> alses, int index)
	{
		var cells = urCells.AsCellMap();
		if (!CheckPreconditionsOnIncomplete(in grid, urCells, d1, d2))
		{
			return;
		}

		if (!arMode && (EmptyCells & cells) != cells)
		{
			return;
		}

		// Check whether there're only 2 extra digit.
		var extraDigitsMask = (Mask)(grid[in cells] & ~comparer);
		if (PopCount((uint)extraDigitsMask) != 2)
		{
			return;
		}

		// We know that cells holding two digits will form a strong link.
		var extraDigit1 = TrailingZeroCount(extraDigitsMask);
		var extraDigit2 = extraDigitsMask.GetNextSet(extraDigit1);
		var extraCells1 = CandidatesMap[extraDigit1] & cells;
		var extraCells2 = CandidatesMap[extraDigit2] & cells;
		foreach (ref readonly var quadruple in (
			(ExtraCells1: extraCells1, ExtraCells2: extraCells2, ExtraDigit1: extraDigit1, ExtraDigit2: extraDigit2),
			(ExtraCells1: extraCells2, ExtraCells2: extraCells1, ExtraDigit1: extraDigit2, ExtraDigit2: extraDigit1)
		))
		{
			ref readonly var cells1 = ref quadruple.ExtraCells1;
			ref readonly var cells2 = ref quadruple.ExtraCells2;
			var digit1 = quadruple.ExtraDigit1;
			var digit2 = quadruple.ExtraDigit2;

			// Check elimination on digit2 and find weak links on digit1.
			foreach (var als in alses)
			{
				// Check whether the ALS digits mask holds all possible digits as extra digits in UR.
				var alsDigitsMask = als.DigitsMask;
				if ((alsDigitsMask & extraDigitsMask) != extraDigitsMask)
				{
					continue;
				}

				// Check whether the ALS pattern intersects with the current UR pattern.
				// If so, we should skip it because it'll disturb out work.
				#region Extra description
				// In fact, technique allows ALS pattern intersects with UR. There're two cases:
				//
				//   1) ALS node (strong link node) overlaps with cells that UR uses.
				//      In this case, the doubly-linked ALS-XZ will be replaced
				//      with another technique called 'Self Constraint' in this program. This will be handled in chaining searcher.
				//   2) Though 1) doesn't cover, ALS pattern still overlaps with UR pattern cells.
				//      In this case, the doubly-linked ALS-XZ still forms,
				//      but extra eliminations produced by UR pattern will become unavailable.
				//
				// Considered its complexity of implementation, I will skip overlapped cases and may not handle such cases.
				#endregion
				var alsCells = als.Cells;
				if (alsCells & cells)
				{
					continue;
				}

				var alsCells1 = alsCells & CandidatesMap[digit1];
				var alsCells2 = alsCells & CandidatesMap[digit2];

				// Check whether digit1 forms a weak link from both ALS and UR.
				if ((cells1.PeerIntersection & alsCells1) != alsCells1)
				{
					continue;
				}

				// Check for elimination cells.
				var digit2UnionMap = cells2 | alsCells2;
				var elimMap = digit2UnionMap.PeerIntersection & CandidatesMap[digit2];
				if (!elimMap)
				{
					continue;
				}

				// Okay. Now elimination is found. Collect view nodes and record the step.
				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var cell in urCells)
				{
					if (grid.GetState(cell) != CellState.Empty)
					{
						continue;
					}

					foreach (var digit in comparer)
					{
						if ((grid.GetCandidates(cell) >> digit & 1) != 0)
						{
							candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + digit));
						}
					}
				}
				foreach (var cell in cells2)
				{
					candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, cell * 9 + digit2));
				}
				foreach (var cell in cells1)
				{
					candidateOffsets.Add(new(ColorIdentifier.Auxiliary2, cell * 9 + digit1));
				}
				foreach (var cell in alsCells)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								digit == digit1
									? ColorIdentifier.Auxiliary2
									: digit == digit2 ? ColorIdentifier.Auxiliary1 : ColorIdentifier.AlmostLockedSet1,
								cell * 9 + digit
							)
						);
					}
				}
				if (!IsIncompleteValid(arMode, AllowIncompleteUniqueRectangles, candidateOffsets, out var isIncomplete))
				{
					continue;
				}

				var eliminations = from cell in elimMap select new Conclusion(Elimination, cell, digit2);
				var cellOffsets = (List<CellViewNode>)[
					.. from cell in urCells select new CellViewNode(ColorIdentifier.Normal, cell),
					.. from cell in alsCells select new CellViewNode(ColorIdentifier.AlmostLockedSet1, cell)
				];
				var multivalueCellsCount = 0;
				foreach (var cell in urCells)
				{
					if ((Mask)(grid.GetCandidates(cell) & ~comparer) != 0)
					{
						multivalueCellsCount++;
					}
				}
				if (!digit2UnionMap.InOneHouse(out _))
				{
					accumulator.Add(
						new UniqueRectangleAlmostLockedSetsXzStep(
							[.. eliminations],
							[[.. cellOffsets, .. candidateOffsets]],
							context.Options,
							d1,
							d2,
							in cells,
							isIncomplete,
							arMode,
							false,
							als,
							multivalueCellsCount,
							index
						)
					);
					continue;
				}

				// Skip the case on ARs.
				if (arMode)
				{
					continue;
				}

				// A Doubly-linked ALS-XZ pattern is formed. We should check for extra eliminations.
				// Extra eliminations can be found in two places:
				//
				//   1) ALS pattern -> subset
				//   2) UR pattern
				//
				// For 2), it will be tough to be checked.
				var doublyLinkedEliminations = (List<Conclusion>)[
					.. eliminations,
					..
					from elimCell in (cells1 | alsCells1).PeerIntersection & CandidatesMap[digit1]
					select new Conclusion(Elimination, elimCell, digit1),
					.. UniqueRectangleModule.GetConclusions(in cells, comparer, in grid)
				];
				foreach (var cell in HousesMap[als.House] & ~alsCells)
				{
					foreach (var digit in (Mask)(grid.GetCandidates(cell) & alsDigitsMask & ~extraDigitsMask))
					{
						doublyLinkedEliminations.Add(new(Elimination, cell, digit));
					}
				}
				if (doublyLinkedEliminations.Count == 0)
				{
					continue;
				}

				// Change view nodes.
				foreach (var cell in cells1)
				{
					candidateOffsets.Remove(new(ColorIdentifier.Auxiliary2, cell * 9 + digit1));
					candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, cell * 9 + digit1));
				}
				foreach (var cell in alsCells)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						if (digit != digit1)
						{
							continue;
						}

						candidateOffsets.Remove(new(ColorIdentifier.Auxiliary2, cell * 9 + digit));
						candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, cell * 9 + digit));
					}
				}

				accumulator.Add(
					new UniqueRectangleAlmostLockedSetsXzStep(
						[.. doublyLinkedEliminations],
						[[.. cellOffsets, .. candidateOffsets]],
						context.Options,
						d1,
						d2,
						in cells,
						isIncomplete,
						arMode,
						true,
						als,
						multivalueCellsCount,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR + baba grouping.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="comparer">The comparer.</param>
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///      ↓urCellInSameBlock
	/// ab  abc      abc  ←anotherCell
	///
	///     abcx-----abcy ←resultCell
	///           c
	///      ↑targetCell
	/// ]]></code>
	/// Where the digit <c>a</c> and <c>b</c> in the bottom-left cell <c>abcx</c> can be removed.
	/// </remarks>
	private partial void CheckBabaGroupingUnique(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref AnalysisContext context, Cell[] urCells, Mask comparer, Digit d1, Digit d2, int index)
	{
		var cells = urCells.AsCellMap();

		// Check all cells are empty.
		var containsValueCells = false;
		foreach (var cell in cells)
		{
			if (grid.GetState(cell) != CellState.Empty)
			{
				containsValueCells = true;
				break;
			}
		}
		if (containsValueCells)
		{
			return;
		}

		// Iterate on each cell.
		foreach (var targetCell in cells)
		{
			var block = targetCell.ToHouse(HouseType.Block);
			var bivalueCellsToCheck = PeersMap[targetCell] & HousesMap[block] & BivalueCells & ~cells;
			if (!bivalueCellsToCheck)
			{
				continue;
			}

			// Check all bi-value cells.
			foreach (var bivalueCellToCheck in bivalueCellsToCheck)
			{
				if ((bivalueCellToCheck.AsCellMap() + targetCell).SharedLine != TrailingZeroCountFallback)
				{
					// 'targetCell' and 'bivalueCellToCheck' can't lie on a same line.
					continue;
				}

				if (grid.GetCandidates(bivalueCellToCheck) != comparer)
				{
					// 'bivalueCell' must contain both 'd1' and 'd2'.
					continue;
				}

				var urCellInSameBlock = ((HousesMap[block] & cells) - targetCell)[0];
				var coveredLine = (bivalueCellToCheck.AsCellMap() + urCellInSameBlock).SharedLine;
				if (coveredLine == TrailingZeroCountFallback)
				{
					// The bi-value cell 'bivalueCellToCheck' should be lie on a same house
					// as 'urCellInSameBlock'.
					continue;
				}

				var anotherCell = (cells - urCellInSameBlock & HousesMap[coveredLine])[0];
				foreach (var extraDigit in (Mask)(grid.GetCandidates(targetCell) & ~comparer))
				{
					var abcMask = (Mask)(comparer | (Mask)(1 << extraDigit));
					if (grid.GetCandidates(anotherCell) != abcMask)
					{
						continue;
					}

					// Check the conjugate pair of the extra digit.
					var resultCell = (cells - urCellInSameBlock - anotherCell - targetCell)[0];
					var map = targetCell.AsCellMap() + resultCell;
					var line = map.SharedLine;
					if (!IsConjugatePair(extraDigit, in map, line))
					{
						continue;
					}

					var _xOr_yMask = grid.GetCandidates(bivalueCellToCheck);
					if (grid.GetCandidates(urCellInSameBlock) != abcMask)
					{
						goto SubType2;
					}

					// Here, is the basic sub-type having passed the checking.
					// Gather conclusions.
					var conclusions = new List<Conclusion>();
					foreach (var digit in grid.GetCandidates(targetCell))
					{
						if (digit == d1 || digit == d2)
						{
							conclusions.Add(new(Elimination, targetCell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						goto SubType2;
					}

					// Collect for view nodes.
					var candidateOffsets = (List<CandidateViewNode>)[new(ColorIdentifier.Auxiliary1, targetCell * 9 + extraDigit)];
					var candidateOffsets2 = (List<CandidateViewNode>)[
						new(ColorIdentifier.Auxiliary1, resultCell * 9 + extraDigit),
						new(ColorIdentifier.Auxiliary1, targetCell * 9 + extraDigit),
					];
					if (CandidatesMap[d1].Contains(resultCell))
					{
						candidateOffsets.Add(new(ColorIdentifier.Normal, resultCell * 9 + d1));
					}
					if (CandidatesMap[d2].Contains(resultCell))
					{
						candidateOffsets.Add(new(ColorIdentifier.Normal, resultCell * 9 + d2));
					}
					if (CandidatesMap[extraDigit].Contains(resultCell))
					{
						candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, resultCell * 9 + extraDigit));
					}

					foreach (var digit in (Mask)(grid.GetCandidates(urCellInSameBlock) & abcMask))
					{
						if (digit == extraDigit)
						{
							candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, urCellInSameBlock * 9 + digit));
							candidateOffsets2.Add(new(ColorIdentifier.Auxiliary1, urCellInSameBlock * 9 + digit));
						}
						else
						{
							candidateOffsets.Add(new(ColorIdentifier.Normal, urCellInSameBlock * 9 + digit));
						}
					}
					foreach (var digit in grid.GetCandidates(anotherCell))
					{
						if (digit == extraDigit)
						{
							candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, anotherCell * 9 + digit));
							candidateOffsets2.Add(new(ColorIdentifier.Auxiliary1, anotherCell * 9 + digit));
						}
						else
						{
							candidateOffsets.Add(new(ColorIdentifier.Normal, anotherCell * 9 + digit));
						}
					}
					foreach (var digit in _xOr_yMask)
					{
						candidateOffsets.Add(new(ColorIdentifier.Auxiliary2, bivalueCellToCheck * 9 + digit));
					}

					// Add into the list.
					var extraDigitId = (char)(extraDigit + '1');
					var extraDigitMask = (Mask)(1 << extraDigit);
					accumulator.Add(
						new UniqueRectangleWithBabaGroupingStep(
							[.. conclusions],
							[
								[
									new CellViewNode(ColorIdentifier.Normal, targetCell),
									.. candidateOffsets,
									new HouseViewNode(ColorIdentifier.Normal, block),
									new HouseViewNode(ColorIdentifier.Auxiliary1, line)
								],
								[
									.. candidateOffsets2,
									new BabaGroupViewNode(bivalueCellToCheck, 'y', _xOr_yMask),
									new BabaGroupViewNode(targetCell, 'x', _xOr_yMask),
									new BabaGroupViewNode(urCellInSameBlock, extraDigitId, extraDigitMask),
									new BabaGroupViewNode(anotherCell, 'x', _xOr_yMask),
									new BabaGroupViewNode(resultCell, extraDigitId, extraDigitMask)
								]
							],
							context.Options,
							d1,
							d2,
							[.. urCells],
							targetCell,
							extraDigit,
							index
						)
					);

				SubType2:
					// Sub-type 2.
					// The extra digit should form a conjugate pair in that line.
					var anotherMap = urCellInSameBlock.AsCellMap() + anotherCell;
					var anotherLine = anotherMap.SharedLine;
					if (!IsConjugatePair(extraDigit, in anotherMap, anotherLine))
					{
						continue;
					}

					// This type also requires both diagonal cells should contain digits x, y and the conjugate pair digit.
					var _xOr_yOrExtraDigitMask = (Mask)(_xOr_yMask | (Mask)(1 << extraDigit));
					var bothDiagonalCellsContain_xOr_yOrExtraDigit = true;
					foreach (var cell in targetCell.AsCellMap() + GetDiagonalCell(urCells, targetCell))
					{
						if ((grid.GetCandidates(cell) & _xOr_yOrExtraDigitMask) != _xOr_yOrExtraDigitMask)
						{
							bothDiagonalCellsContain_xOr_yOrExtraDigit = false;
							break;
						}
					}
					if (!bothDiagonalCellsContain_xOr_yOrExtraDigit)
					{
						continue;
					}

					// Collect conclusions.
					var conclusionsAnotherSubType = new List<Conclusion>();
					foreach (var digit in grid.GetCandidates(targetCell))
					{
						if (digit == d1 || digit == d2)
						{
							conclusionsAnotherSubType.Add(new(Elimination, targetCell, digit));
						}
					}
					if (conclusionsAnotherSubType.Count == 0)
					{
						continue;
					}

					// Collect for view nodes.
					candidateOffsets = [];
					candidateOffsets2 = [new CandidateViewNode(ColorIdentifier.Auxiliary1, resultCell * 9 + extraDigit)];
					if (CandidatesMap[d1].Contains(resultCell))
					{
						candidateOffsets.Add(new(ColorIdentifier.Normal, resultCell * 9 + d1));
					}
					if (CandidatesMap[d2].Contains(resultCell))
					{
						candidateOffsets.Add(new(ColorIdentifier.Normal, resultCell * 9 + d2));
					}
					if (CandidatesMap[extraDigit].Contains(resultCell))
					{
						candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, resultCell * 9 + extraDigit));
					}

					foreach (var digit in (Mask)(grid.GetCandidates(urCellInSameBlock) & abcMask))
					{
						if (digit == extraDigit)
						{
							candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, urCellInSameBlock * 9 + digit));
							candidateOffsets2.Add(new(ColorIdentifier.Auxiliary1, urCellInSameBlock * 9 + digit));
						}
						else
						{
							candidateOffsets.Add(new(ColorIdentifier.Normal, urCellInSameBlock * 9 + digit));
						}
					}
					foreach (var digit in grid.GetCandidates(anotherCell))
					{
						if (digit == extraDigit)
						{
							candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, anotherCell * 9 + digit));
							candidateOffsets2.Add(new(ColorIdentifier.Auxiliary1, anotherCell * 9 + digit));
						}
						else
						{
							candidateOffsets.Add(new(ColorIdentifier.Normal, anotherCell * 9 + digit));
						}
					}
					var _xOr_yMask2 = grid.GetCandidates(bivalueCellToCheck);
					foreach (var digit in _xOr_yMask2)
					{
						candidateOffsets.Add(new(ColorIdentifier.Auxiliary2, bivalueCellToCheck * 9 + digit));
					}

					// Add into the list.
					var extraDigitId2 = (char)(extraDigit + '1');
					var extraDigitMask2 = (Mask)(1 << extraDigit);
					accumulator.Add(
						new UniqueRectangleWithBabaGroupingStep(
							[.. conclusionsAnotherSubType],
							[
								[
									new CellViewNode(ColorIdentifier.Normal, targetCell),
									..
									from digit in grid.GetCandidates(targetCell)
									let id = extraDigit == digit ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal
									select new CandidateViewNode(id, targetCell * 9 + digit),
									.. candidateOffsets,
									new HouseViewNode(ColorIdentifier.Normal, block),
									new HouseViewNode(ColorIdentifier.Auxiliary1, line),
									new HouseViewNode(ColorIdentifier.Auxiliary1, anotherLine)
								],
								[
									.. candidateOffsets2,
									new CandidateViewNode(ColorIdentifier.Auxiliary1, targetCell * 9 + extraDigit),
									new BabaGroupViewNode(bivalueCellToCheck, 'y', _xOr_yMask2),
									new BabaGroupViewNode(targetCell, 'x', _xOr_yMask2),
									new BabaGroupViewNode(urCellInSameBlock, extraDigitId2, extraDigitMask2),
									new BabaGroupViewNode(anotherCell, 'x', _xOr_yMask2),
									new BabaGroupViewNode(resultCell, extraDigitId2, extraDigitMask2)
								]
							],
							context.Options,
							d1,
							d2,
							[.. urCells],
							targetCell,
							extraDigit,
							index
						)
					);
				}
			}
		}
	}

	/// <summary>
	/// Check AR + Hidden single.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">
	/// The map of other cells during the current UR searching.
	/// </param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// <para>The pattern:</para>
	/// <para>
	/// <code><![CDATA[
	/// ↓corner1
	/// a   | aby.  .
	/// abx | a  .  b
	///     | .  .  .
	///       ↑corner2(cell 'a')
	/// ]]></code>
	/// There's only one cell can be filled with the digit 'b' besides the cell 'aby'.
	/// </para>
	/// </remarks>
	private partial void CheckHiddenSingleAvoidable(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref AnalysisContext context, Cell[] urCells, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index)
	{
		if (grid.GetState(corner1) != CellState.Modifiable || grid.GetState(corner2) != CellState.Modifiable
			|| grid.GetDigit(corner1) != grid.GetDigit(corner2) || grid.GetDigit(corner1) != d1 && grid.GetDigit(corner1) != d2)
		{
			return;
		}

		// Get the base digit ('a') and the other digit ('b').
		// Here 'b' is the digit that we should check the possible hidden single.
		var baseDigit = grid.GetDigit(corner1);
		var otherDigit = baseDigit == d1 ? d2 : d1;
		var cellsThatTwoOtherCellsBothCanSee = otherCellsMap.PeerIntersection & CandidatesMap[otherDigit];

		// Iterate on two cases (because we holds two other cells,
		// and both those two cells may contain possible elimination).
		for (var i = 0; i < 2; i++)
		{
			var (baseCell, anotherCell) = i == 0 ? (otherCellsMap[0], otherCellsMap[1]) : (otherCellsMap[1], otherCellsMap[0]);

			// Iterate on each house type.
			foreach (var houseType in HouseTypes)
			{
				var houseIndex = baseCell.ToHouse(houseType);

				// If the house doesn't overlap with the specified house, just skip it.
				if (!(cellsThatTwoOtherCellsBothCanSee & HousesMap[houseIndex]))
				{
					continue;
				}

				var otherCells = HousesMap[houseIndex] & CandidatesMap[otherDigit] & PeersMap[anotherCell];
				var sameHouses = (otherCells + anotherCell).SharedHouses;
				foreach (var sameHouse in sameHouses)
				{
					// Check whether all possible positions of the digit 'b' in this house only
					// lies in the given cells above ('cellsThatTwoOtherCellsBothCanSee').
					if ((HousesMap[sameHouse] - anotherCell & CandidatesMap[otherDigit]) != otherCells)
					{
						continue;
					}

					// Possible hidden single found.
					// If the elimination doesn't exist, just skip it.
					if (!CandidatesMap[otherDigit].Contains(baseCell))
					{
						continue;
					}

					var cellOffsets = new List<CellViewNode>();
					foreach (var cell in urCells)
					{
						cellOffsets.Add(new(ColorIdentifier.Normal, cell));
					}

					var candidateOffsets = new List<CandidateViewNode> { new(ColorIdentifier.Normal, anotherCell * 9 + otherDigit) };
					foreach (var cell in otherCells)
					{
						candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, cell * 9 + otherDigit));
					}

					accumulator.Add(
						new AvoidableRectangleHiddenSingleStep(
							[new(Elimination, baseCell, otherDigit)],
							[[.. cellOffsets, .. candidateOffsets, new HouseViewNode(ColorIdentifier.Normal, sameHouse)]],
							context.Options,
							d1,
							d2,
							[.. urCells],
							baseCell,
							anotherCell,
							sameHouse,
							index
						)
					);
				}
			}
		}
	}
}
