namespace Sudoku.Solving.Manual.Searchers.DeadlyPatterns.Rectangles;

/// <summary>
/// Provides with a <b>Unique Rectangle</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Basic types:
/// <list type="bullet">
/// <item>Unique/Avoidable Rectangle Type 1, 2, 3, 5</item>
/// <item>Unique Rectangle Type 4, 6</item>
/// <item>Hidden Unique/Avoidable Rectangle</item>
/// </list>
/// </item>
/// <item>
/// Unique Rectangle with structures:
/// <list type="bullet">
/// <item>Unique Rectangle with Conjugate Pair</item>
/// <item>Avoidable Rectangle with Hidden Single</item>
/// <item>Unique Rectangle with Unknown Covering</item>
/// <item>Unique Rectangle with Sue de Coq</item>
/// <item>Unique Rectangle with Guardian</item>
/// </list>
/// </item>
/// <item>
/// Miscellaneous:
/// <list type="bullet">
/// <item>Unique Rectangle 2D, 3X</item>
/// </list>
/// </item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe class UniqueRectangleStepSearcher : IUniqueRectangleStepSearcher
{
	/// <inheritdoc/>
	public bool AllowIncompleteUniqueRectangles { get; set; }

	/// <inheritdoc/>
	public bool SearchForExtendedUniqueRectangles { get; set; }

	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(8, DisplayingLevel.B);

	/// <inheritdoc/>
	public delegate*<in Grid, bool> Predicate
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => null;
	}


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		bool* possibleAvoidableCases = stackalloc[] { false, true };

		var list = new List<UniqueRectangleStep>();

		// Iterate on mode (whether use AR or UR mode to search).
		for (int i = 0; i < 2; i++)
		{
			GetAll(list, grid, possibleAvoidableCases[i]);
		}
		if (list.Count == 0)
		{
			goto ReturnNull;
		}

		// Sort and remove duplicate instances if worth.
		var resultList =
			from step in IDistinctableStep<UniqueRectangleStep>.Distinct(list)
			orderby step.TechniqueCode, step.AbsoluteOffset
			select step;
		if (onlyFindOne)
		{
			goto ReturnFirstElement;
		}

		accumulator.AddRange(resultList);

	ReturnNull:
		return null;

	ReturnFirstElement:
		return resultList.First();
	}

	/// <summary>
	/// Get all possible hints from the grid.
	/// </summary>
	/// <param name="gathered">The list stored the result.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="arMode">Indicates whether the current mode is searching for ARs.</param>
	private void GetAll(ICollection<UniqueRectangleStep> gathered, in Grid grid, bool arMode)
	{
		// Iterate on each possible UR structure.
		for (int index = 0, outerLength = Patterns.Length; index < outerLength; index++)
		{
			int[] urCells = Patterns[index];

			// Check preconditions.
			if (!CheckPreconditions(grid, urCells, arMode))
			{
				continue;
			}

			// Get all candidates that all four cells appeared.
			short mask = 0;
			foreach (int urCell in urCells)
			{
				mask |= grid.GetCandidates(urCell);
			}

			// Iterate on each possible digit combination.
			var allDigitsInThem = mask.GetAllSets();
			for (int i = 0, length = allDigitsInThem.Length, innerLength = length - 1; i < innerLength; i++)
			{
				int d1 = allDigitsInThem[i];
				for (int j = i + 1; j < length; j++)
				{
					int d2 = allDigitsInThem[j];

					// All possible UR patterns should contain at least one cell that contains both 'd1' and 'd2'.
					short comparer = (short)(1 << d1 | 1 << d2);
					bool isNotPossibleUr = true;
					foreach (int cell in urCells)
					{
						if (PopCount((uint)(grid.GetCandidates(cell) & comparer)) == 2)
						{
							isNotPossibleUr = false;
							break;
						}
					}
					if (!arMode && isNotPossibleUr)
					{
						continue;
					}

					if (SearchForExtendedUniqueRectangles)
					{
						CheckUnknownCoveringUnique(gathered, grid, urCells, comparer, d1, d2, index);
						CheckGuardianUniqueStandard(gathered, grid, urCells, comparer, d1, d2, index);
#if IMPLEMENTED
						CheckGuardianAvoidableStandard(gathered, grid, urCells, comparer, d1, d2, index);
						for (int size = 2; size < 4; size++)
						{
							CheckGuardianUniqueSubset(gathered, grid, urCells, comparer, d1, d2, index);
							CheckGuardianAvoidableSubset(gathered, grid, urCells, comparer, d1, d2, index);
						}
#endif
					}

					// Iterate on each corner of four cells.
					for (int c1 = 0; c1 < 4; c1++)
					{
						int corner1 = urCells[c1];
						var otherCellsMap = new Cells(urCells) { ~corner1 };

						CheckType1(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, otherCellsMap, index);
						CheckType5(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, otherCellsMap, index);
						CheckHidden(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, otherCellsMap, index);

						if (!arMode && SearchForExtendedUniqueRectangles)
						{
							Check3X(gathered, grid, urCells, false, comparer, d1, d2, corner1, otherCellsMap, index);
							Check3X2SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, otherCellsMap, index);
							Check3N2SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, otherCellsMap, index);
							Check3U2SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, otherCellsMap, index);
							Check3E2SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, otherCellsMap, index);
						}

						// If we aim to a single cell, all four cells should be checked.
						// Therefore, the 'break' clause should be written here, rather than 'for' loop declaration.
						// If that declaration is 'for (int c1 = 0; c1 < 3; c1++)',
						// we'll miss the cases for checking the last cell.
						if (c1 == 3)
						{
							break;
						}

						for (int c2 = c1 + 1; c2 < 4; c2++)
						{
							int corner2 = urCells[c2];
							var tempOtherCellsMap = otherCellsMap - corner2;

							// Both diagonal and non-diagonal.
							CheckType2(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);

							if (SearchForExtendedUniqueRectangles)
							{
								for (int size = 2; size <= 4; size++)
								{
									CheckWing(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, size, index);
								}
							}

							switch ((CornerCell1: c1, CornerCell2: c2))
							{
								// Diagonal type.
								case (CornerCell1: 0, CornerCell2: 3):
								case (CornerCell1: 1, CornerCell2: 2):
								{
									if (!arMode)
									{
										CheckType6(gathered, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);

										if (SearchForExtendedUniqueRectangles)
										{
											Check2D(gathered, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
											Check2D1SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
										}
									}
									else
									{
										// Don't merge into the else-if block. Here the methods may be extended.
										if (SearchForExtendedUniqueRectangles)
										{
											CheckHiddenSingleAvoidable(gathered, grid, urCells, d1, d2, corner1, corner2, tempOtherCellsMap, index);
										}
									}

									break;
								}

								// Non-diagonal type.
								default:
								{
									CheckType3Naked(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
#if IMPLEMENTED
									CheckType3Hidden(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
#endif

									if (!arMode)
									{
										CheckType4(gathered, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);

										if (SearchForExtendedUniqueRectangles)
										{
											Check2B1SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
											Check4X3SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
											Check4C3SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
										}
									}

									if (SearchForExtendedUniqueRectangles)
									{
										CheckSdc(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
									}

									break;
								}
							}
						}
					}
				}
			}
		}
	}


	#region Utilized methods
	/// <summary>
	/// Check preconditions.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is searching for ARs.</param>
	/// <returns>Indicates whether the UR is passed to check.</returns>
	private static bool CheckPreconditions(in Grid grid, int[] urCells, bool arMode)
	{
		byte emptyCountWhenArMode = 0, modifiableCount = 0;
		foreach (int urCell in urCells)
		{
			switch (grid.GetStatus(urCell))
			{
				case CellStatus.Given:
				case CellStatus.Modifiable when !arMode:
				{
					return false;
				}
				case CellStatus.Empty when arMode:
				{
					emptyCountWhenArMode++;
					break;
				}
				case CellStatus.Modifiable:
				{
					modifiableCount++;
					break;
				}
			}
		}

		return modifiableCount != 4 && emptyCountWhenArMode != 4;
	}

	/// <summary>
	/// To determine whether the specified region forms a conjugate pair
	/// of the specified digit, and the cells where they contain the digit
	/// is same as the given map contains.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <param name="map">The map.</param>
	/// <param name="region">The region.</param>
	/// <returns>A <see cref="bool"/> value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsConjugatePair(int digit, in Cells map, int region) =>
		(RegionMaps[region] & CandMaps[digit]) == map;

	/// <summary>
	/// Check whether the highlight UR candidates is incomplete.
	/// </summary>
	/// <param name="list">The list to check.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool IsIncompleteUr(IEnumerable<(int, ColorIdentifier Identifier)> list) =>
		!AllowIncompleteUniqueRectangles && list.Count(static d => d.Identifier is { UseId: true, Id: 0 }) != 8;

	/// <summary>
	/// Get a cell that can't see each other.
	/// </summary>
	/// <param name="urCells">The UR cells.</param>
	/// <param name="cell">The current cell.</param>
	/// <returns>The diagonal cell.</returns>
	/// <exception cref="ArgumentException">
	/// Throws when the specified argument <paramref name="cell"/> is invalid.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetDiagonalCell(int[] urCells, int cell) =>
		cell == urCells[0]
			? urCells[3]
			: cell == urCells[1]
				? urCells[2]
				: cell == urCells[2]
					? urCells[1]
					: urCells[0];

	/// <summary>
	/// Get whether two cells are in a same region.
	/// </summary>
	/// <param name="cell1">The cell 1 to check.</param>
	/// <param name="cell2">The cell 2 to check.</param>
	/// <param name="region">
	/// The result regions that both two cells lie in. If the cell can't be found, this argument will be 0.
	/// </param>
	/// <returns>
	/// The <see cref="bool"/> value indicating whether the another cell is same region as the current one.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsSameRegionCell(int cell1, int cell2, [DiscardWhen(false)] out int region)
	{
		(bool r, region) = new Cells { cell1, cell2 }.CoveredRegions is var v and not 0 ? (true, v) : (false, 0);
		return r;
	}

	/// <summary>
	/// Get all highlight cells.
	/// </summary>
	/// <param name="urCells">The all UR cells used.</param>
	/// <returns>The list of highlight cells.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static IList<(int, ColorIdentifier)> GetHighlightCells(int[] urCells) => new[]
	{
		(urCells[0], (ColorIdentifier)0),
		(urCells[1], (ColorIdentifier)0),
		(urCells[2], (ColorIdentifier)0),
		(urCells[3], (ColorIdentifier)0)
	};
	#endregion


	#region Basic types implmentation
	/// <summary>
	/// Check type 1.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	/// |   ↓ cornerCell
	/// | (abc) ab
	/// |  ab   ab
	/// ]]></code>
	/// </remarks>
	private void CheckType1(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		bool arMode,
		short comparer,
		int d1,
		int d2,
		int cornerCell,
		in Cells otherCellsMap,
		int index
	)
	{
		// Get the summary mask.
		short mask = 0;
		foreach (int cell in otherCellsMap)
		{
			mask |= grid.GetCandidates(cell);
		}
		if (mask != comparer)
		{
			return;
		}

		// Type 1 found. Now check elimination.
		bool d1Exists = grid.Exists(cornerCell, d1) is true;
		bool d2Exists = grid.Exists(cornerCell, d2) is true;
		if (!d1Exists && d2Exists)
		{
			return;
		}

		var conclusions = new List<Conclusion>(2);
		if (d1Exists) conclusions.Add(new(ConclusionType.Elimination, cornerCell, d1));
		if (d2Exists) conclusions.Add(new(ConclusionType.Elimination, cornerCell, d2));

		var candidateOffsets = new List<(int, ColorIdentifier)>(6);
		foreach (int cell in otherCellsMap)
		{
			foreach (int digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)0));
			}
		}

		if (!AllowIncompleteUniqueRectangles && (candidateOffsets.Count, conclusions.Count) != (6, 2))
		{
			return;
		}

		accumulator.Add(
			new UniqueRectangleType1Step(
				ImmutableArray.CreateRange(conclusions),
				ImmutableArray.Create(new PresentationData
				{
					Cells = arMode ? GetHighlightCells(urCells) : null,
					Candidates = arMode ? null : candidateOffsets
				}),
				d1,
				d2,
				urCells,
				arMode,
				index
			)
		);
	}

	/// <summary>
	/// Check type 2.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
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
	/// The structure:
	/// <code><![CDATA[
	/// |   ↓ corner1, corner2
	/// | (abc) (abc)
	/// |  ab    ab
	/// ]]></code>
	/// </remarks>
	private void CheckType2(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		bool arMode,
		short comparer,
		int d1,
		int d2,
		int corner1,
		int corner2,
		in Cells otherCellsMap,
		int index
	)
	{
		// Get the summary mask.
		short mask = 0;
		foreach (int cell in otherCellsMap)
		{
			mask |= grid.GetCandidates(cell);
		}
		if (mask != comparer)
		{
			return;
		}

		// If 'PopCount(mask)' is 1, the type 2 instance will be degenerated to type 1.
		int extraMask = (grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) ^ comparer;
		if ((extraMask & extraMask - 1) == 0)
		{
			return;
		}

		// Type 2 or 5 found. Now check elimination.
		int extraDigit = TrailingZeroCount(extraMask);
		var elimMap = new Cells { corner1, corner2 }.PeerIntersection & CandMaps[extraDigit];
		if (elimMap.IsEmpty)
		{
			return;
		}

		var candidateOffsets = new List<(int, ColorIdentifier)>();
		foreach (int cell in urCells)
		{
			if (grid.GetStatus(cell) == CellStatus.Empty)
			{
				foreach (int digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)(digit == extraDigit ? 1 : 0)));
				}
			}
		}

		if (IsIncompleteUr(candidateOffsets))
		{
			return;
		}

		bool isType5 = !new Cells { corner1, corner2 }.InOneRegion;
		accumulator.Add(
			new UniqueRectangleType2Step(
				elimMap.ToImmutableConclusions(extraDigit),
				ImmutableArray.Create(new PresentationData
				{
					Cells = arMode ? GetHighlightCells(urCells) : null,
					Candidates = candidateOffsets
				}),
				d1,
				d2,
				(IsAvoidableRectangle: arMode, IsType5: isType5) switch
				{
					(IsAvoidableRectangle: true, IsType5: true) => Technique.AvoidableRectangleType5,
					(IsAvoidableRectangle: true, IsType5: false) => Technique.AvoidableRectangleType2,
					(IsAvoidableRectangle: false, IsType5: true) => Technique.UniqueRectangleType5,
					(IsAvoidableRectangle: false, IsType5: false) => Technique.UniqueRectangleType2
				},
				urCells,
				arMode,
				extraDigit,
				index
			)
		);
	}

	/// <summary>
	/// Check type 3.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">
	/// The map of other cells during the current UR searching.
	/// </param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	/// |  ↓ corner1, corner2
	/// | (ab ) (ab )
	/// |  abx   aby
	/// ]]></code>
	/// </remarks>
	private void CheckType3Naked(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		bool arMode,
		short comparer,
		int d1,
		int d2,
		int corner1,
		int corner2,
		in Cells otherCellsMap,
		int index
	)
	{
		bool notSatisfiedType3 = false;
		foreach (int cell in otherCellsMap)
		{
			short currentMask = grid.GetCandidates(cell);
			if ((currentMask & comparer) == 0
				|| currentMask == comparer || arMode && grid.GetStatus(cell) != CellStatus.Empty)
			{
				notSatisfiedType3 = true;
				break;
			}
		}
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer || notSatisfiedType3)
		{
			return;
		}

		short mask = 0;
		foreach (int cell in otherCellsMap)
		{
			mask |= grid.GetCandidates(cell);
		}
		if ((mask & comparer) != comparer)
		{
			return;
		}

		short otherDigitsMask = (short)(mask ^ comparer);
		foreach (int region in otherCellsMap.CoveredRegions)
		{
			if (!((ValueMaps[d1] | ValueMaps[d2]) & RegionMaps[region]).IsEmpty)
			{
				return;
			}

			var iterationMap = (RegionMaps[region] & EmptyMap) - otherCellsMap;
			for (
				int size = PopCount((uint)otherDigitsMask) - 1, count = iterationMap.Count;
				size < count;
				size++
			)
			{
				foreach (int[] iteratedCells in iterationMap.ToArray().GetSubsets(size))
				{
					short tempMask = 0;
					foreach (int cell in iteratedCells)
					{
						tempMask |= grid.GetCandidates(cell);
					}
					if ((tempMask & comparer) != 0 || PopCount((uint)tempMask) - 1 != size
						|| (tempMask & otherDigitsMask) != otherDigitsMask)
					{
						continue;
					}

					var conclusions = new List<Conclusion>(16);
					foreach (int digit in tempMask)
					{
						foreach (int cell in (iterationMap - iteratedCells) & CandMaps[digit])
						{
							conclusions.Add(new(ConclusionType.Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var cellOffsets = new List<(int, ColorIdentifier)>();
					foreach (int cell in urCells)
					{
						if (grid.GetStatus(cell) != CellStatus.Empty)
						{
							cellOffsets.Add((cell, (ColorIdentifier)0));
						}
					}

					var candidateOffsets = new List<(int, ColorIdentifier)>();
					foreach (int cell in urCells)
					{
						if (grid.GetStatus(cell) == CellStatus.Empty)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(
									(
										cell * 9 + digit,
										(ColorIdentifier)((tempMask >> digit & 1) != 0 ? 1 : 0)
									)
								);
							}
						}
					}
					foreach (int cell in iteratedCells)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)1));
						}
					}

					accumulator.Add(
						new UniqueRectangleType3Step(
							ImmutableArray.CreateRange(conclusions),
							ImmutableArray.Create(new PresentationData
							{
								Cells = arMode ? cellOffsets : null,
								Candidates = candidateOffsets,
								Regions = new[] { (region, (ColorIdentifier)0) }
							}),
							d1,
							d2,
							urCells,
							iteratedCells,
							otherDigitsMask,
							region,
							arMode,
							index
						)
					);
				}
			}
		}
	}

	/// <summary>
	/// Check type 4.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">
	/// The map of other cells during the current UR searching.
	/// </param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	/// |  ↓ corner1, corner2
	/// | (ab ) ab
	/// |  abx  aby
	/// ]]></code>
	/// </remarks>
	private void CheckType4(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		bool arMode,
		short comparer,
		int d1,
		int d2,
		int corner1,
		int corner2,
		in Cells otherCellsMap,
		int index
	)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		int* p = stackalloc[] { d1, d2 };
		foreach (int region in otherCellsMap.CoveredRegions)
		{
			if (region < 9)
			{
				// Process the case in lines.
				continue;
			}

			for (int digitIndex = 0; digitIndex < 2; digitIndex++)
			{
				int digit = p[digitIndex];
				if (!IsConjugatePair(digit, otherCellsMap, region))
				{
					continue;
				}

				// Yes, Type 4 found.
				// Now check elimination.
				int elimDigit = TrailingZeroCount(comparer ^ (1 << digit));
				var elimMap = otherCellsMap & CandMaps[elimDigit];
				if (elimMap.IsEmpty)
				{
					continue;
				}

				var candidateOffsets = new List<(int, ColorIdentifier)>(6);
				foreach (int cell in urCells)
				{
					if (grid.GetStatus(cell) != CellStatus.Empty)
					{
						continue;
					}

					if (otherCellsMap.Contains(cell))
					{
						if (d1 != elimDigit && grid.Exists(cell, d1) is true)
						{
							candidateOffsets.Add((cell * 9 + d1, (ColorIdentifier)1));
						}
						if (d2 != elimDigit && grid.Exists(cell, d2) is true)
						{
							candidateOffsets.Add((cell * 9 + d2, (ColorIdentifier)1));
						}
					}
					else
					{
						// Corner1 and corner2.
						foreach (int d in grid.GetCandidates(cell))
						{
							candidateOffsets.Add((cell * 9 + d, (ColorIdentifier)0));
						}
					}
				}

				var conclusions = elimMap.ToImmutableConclusions(elimDigit);
				if (!AllowIncompleteUniqueRectangles && (candidateOffsets.Count, conclusions.Length) != (6, 2))
				{
					continue;
				}

				int[] offsets = otherCellsMap.ToArray();
				accumulator.Add(
					new UniqueRectangleWithConjugatePairStep(
						conclusions,
						ImmutableArray.Create(new PresentationData
						{
							Cells = arMode ? GetHighlightCells(urCells) : null,
							Candidates = candidateOffsets,
							Regions = new[] { (region, (ColorIdentifier)0) }
						}),
						Technique.UniqueRectangleType4,
						d1,
						d2,
						urCells,
						arMode,
						new ConjugatePair[] { new(offsets[0], offsets[1], digit) },
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check type 5.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	/// |  ↓ cornerCell
	/// | (ab ) abc
	/// |  abc  abc
	/// ]]></code>
	/// </remarks>
	private void CheckType5(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		bool arMode,
		short comparer,
		int d1,
		int d2,
		int cornerCell,
		in Cells otherCellsMap,
		int index
	)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		// Get the summary mask.
		short otherCellsMask = 0;
		foreach (int cell in otherCellsMap)
		{
			otherCellsMask |= grid.GetCandidates(cell);
		}

		// Degenerated to type 1.
		short extraMask = (short)(otherCellsMask ^ comparer);
		if ((extraMask & extraMask - 1) != 0)
		{
			return;
		}

		// Type 5 found. Now check elimination.
		int extraDigit = TrailingZeroCount(extraMask);
		var cellsThatContainsExtraDigit = otherCellsMap & CandMaps[extraDigit];

		// Degenerated to type 1.
		if (cellsThatContainsExtraDigit.Count == 1)
		{
			return;
		}

		var elimMap = cellsThatContainsExtraDigit.PeerIntersection & CandMaps[extraDigit];
		if (elimMap.IsEmpty)
		{
			return;
		}

		var candidateOffsets = new List<(int, ColorIdentifier)>(16);
		foreach (int cell in urCells)
		{
			if (grid.GetStatus(cell) != CellStatus.Empty)
			{
				continue;
			}

			foreach (int digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)(digit == extraDigit ? 1 : 0)));
			}
		}
		if (IsIncompleteUr(candidateOffsets))
		{
			return;
		}

		accumulator.Add(
			new UniqueRectangleType2Step(
				elimMap.ToImmutableConclusions(extraDigit),
				ImmutableArray.Create(new PresentationData
				{
					Cells = arMode ? GetHighlightCells(urCells) : null,
					Candidates = candidateOffsets
				}),
				d1,
				d2,
				arMode ? Technique.AvoidableRectangleType5 : Technique.UniqueRectangleType5,
				urCells,
				arMode,
				extraDigit,
				index
			)
		);
	}

	/// <summary>
	/// Check type 6.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
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
	/// The structure:
	/// <code><![CDATA[
	/// |  ↓ corner1
	/// | (ab )  aby
	/// |  abx  (ab)
	/// |        ↑corner2
	/// ]]></code>
	/// </remarks>
	private void CheckType6(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		bool arMode,
		short comparer,
		int d1,
		int d2,
		int corner1,
		int corner2,
		in Cells otherCellsMap,
		int index
	)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		int o1 = otherCellsMap[0], o2 = otherCellsMap[1];
		int r1 = corner1.ToRegion(RegionLabel.Row), c1 = corner1.ToRegion(RegionLabel.Column);
		int r2 = corner2.ToRegion(RegionLabel.Row), c2 = corner2.ToRegion(RegionLabel.Column);
		int* p = stackalloc[] { d1, d2 };
		var q = stackalloc[] { (r1, r2), (c1, c2) };
		for (int digitIndex = 0; digitIndex < 2; digitIndex++)
		{
			int digit = p[digitIndex];
			for (int regionPairIndex = 0; regionPairIndex < 2; regionPairIndex++)
			{
				var (region1, region2) = q[regionPairIndex];
				gather(grid, otherCellsMap, region1 is >= 9 and < 18, digit, region1, region2);
			}
		}


		void gather(in Grid grid, in Cells otherCellsMap, bool isRow, int digit, int region1, int region2)
		{
			if (
				(
					!isRow
					|| !IsConjugatePair(digit, new() { corner1, o1 }, region1)
					|| !IsConjugatePair(digit, new() { corner2, o2 }, region2)
				) && (
					isRow
					|| !IsConjugatePair(digit, new() { corner1, o2 }, region1)
					|| !IsConjugatePair(digit, new() { corner2, o1 }, region2)
				)
			)
			{
				return;
			}

			// Check eliminations.
			var elimMap = otherCellsMap & CandMaps[digit];
			if (elimMap.IsEmpty)
			{
				return;
			}

			var candidateOffsets = new List<(int, ColorIdentifier)>(6);
			foreach (int cell in urCells)
			{
				if (otherCellsMap.Contains(cell))
				{
					if (d1 != digit && grid.Exists(cell, d1) is true)
					{
						candidateOffsets.Add((cell * 9 + d1, (ColorIdentifier)0));
					}
					if (d2 != digit && grid.Exists(cell, d2) is true)
					{
						candidateOffsets.Add((cell * 9 + d2, (ColorIdentifier)0));
					}
				}
				else
				{
					foreach (int d in grid.GetCandidates(cell))
					{
						candidateOffsets.Add((cell * 9 + d, (ColorIdentifier)(d == digit ? 1 : 0)));
					}
				}
			}

			var conclusions = elimMap.ToImmutableConclusions(digit);
			if (!AllowIncompleteUniqueRectangles && (candidateOffsets.Count, conclusions.Length) != (6, 2))
			{
				return;
			}

			accumulator.Add(
				new UniqueRectangleWithConjugatePairStep(
					conclusions,
					ImmutableArray.Create(new PresentationData
					{
						Cells = arMode ? GetHighlightCells(urCells) : null,
						Candidates = candidateOffsets,
						Regions = new[] { (region1, (ColorIdentifier)0), (region2, (ColorIdentifier)0) }
					}),
					Technique.UniqueRectangleType6,
					d1,
					d2,
					urCells,
					false,
					new ConjugatePair[]
					{
						new(corner1, isRow ? o1 : o2, digit),
						new(corner2, isRow ? o2 : o1, digit)
					},
					index
				)
			);
		}
	}

	/// <summary>
	/// Check hidden UR.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	/// |  ↓ cornerCell
	/// | (ab ) abx
	/// |  aby  abz
	/// ]]></code>
	/// </remarks>
	private void CheckHidden(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		bool arMode,
		short comparer,
		int d1,
		int d2,
		int cornerCell,
		in Cells otherCellsMap,
		int index
	)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		int abzCell = GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		int abxCell = adjacentCellsMap[0], abyCell = adjacentCellsMap[1];
		int r = abzCell.ToRegion(RegionLabel.Row), c = abzCell.ToRegion(RegionLabel.Column);
		int* p = stackalloc[] { d1, d2 };
		for (int digitIndex = 0; digitIndex < 2; digitIndex++)
		{
			int digit = p[digitIndex];
			Cells map1 = new() { abzCell, abxCell }, map2 = new() { abzCell, abyCell };
			if (!IsConjugatePair(digit, map1, map1.CoveredLine) || !IsConjugatePair(digit, map2, map2.CoveredLine))
			{
				continue;
			}

			// Hidden UR found. Now check eliminations.
			int elimDigit = TrailingZeroCount(comparer ^ (1 << digit));
			if (grid.Exists(abzCell, elimDigit) is not true)
			{
				continue;
			}

			var candidateOffsets = new List<(int, ColorIdentifier)>();
			foreach (int cell in urCells)
			{
				if (grid.GetStatus(cell) != CellStatus.Empty)
				{
					continue;
				}

				if (otherCellsMap.Contains(cell))
				{
					if ((cell != abzCell || d1 != elimDigit) && grid.Exists(cell, d1) is true)
					{
						candidateOffsets.Add((cell * 9 + d1, (ColorIdentifier)(d1 != elimDigit ? 1 : 0)));
					}
					if ((cell != abzCell || d2 != elimDigit) && grid.Exists(cell, d2) is true)
					{
						candidateOffsets.Add((cell * 9 + d2, (ColorIdentifier)(d2 != elimDigit ? 1 : 0)));
					}
				}
				else
				{
					foreach (int d in grid.GetCandidates(cell))
					{
						candidateOffsets.Add((cell * 9 + d, (ColorIdentifier)0));
					}
				}
			}

			if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
			{
				continue;
			}

			accumulator.Add(
				new HiddenUniqueRectangleStep(
					ImmutableArray.Create(new Conclusion(ConclusionType.Elimination, abzCell, elimDigit)),
					ImmutableArray.Create(new PresentationData
					{
						Cells = arMode ? GetHighlightCells(urCells) : null,
						Candidates = candidateOffsets,
						Regions = new[] { (r, (ColorIdentifier)0), (c, (ColorIdentifier)0) }
					}),
					d1,
					d2,
					urCells,
					arMode,
					new ConjugatePair[]
					{
						new(abzCell, abxCell, digit),
						new(abzCell, abyCell, digit),
					},
					index
				)
			);
		}
	}
	#endregion

	#region Extended types implementation
	/// <summary>
	/// Check UR+2D.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
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
	/// The structure:
	/// <code><![CDATA[
	/// |   ↓ corner1
	/// | (ab )  abx
	/// |  aby  (ab )  xy  *
	/// |         ↑ corner2
	/// ]]></code>
	/// </remarks>
	private void Check2D(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		bool arMode,
		short comparer,
		int d1,
		int d2,
		int corner1,
		int corner2,
		in Cells otherCellsMap,
		int index
	)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		short o1 = grid.GetCandidates(otherCellsMap[0]), o2 = grid.GetCandidates(otherCellsMap[1]);
		short o = (short)(o1 | o2);
		if (
			(
				TotalNumbersCount: PopCount((uint)o),
				OtherCell1NumbersCount: PopCount((uint)o1),
				OtherCell2NumbersCount: PopCount((uint)o2),
				OtherCell1Intersetion: o1 & comparer,
				OtherCell2Intersetion: o2 & comparer
			) is not (
				TotalNumbersCount: 4,
				OtherCell1NumbersCount: <= 3,
				OtherCell2NumbersCount: <= 3,
				OtherCell1Intersetion: not 0,
				OtherCell2Intersetion: not 0
			) || (o & comparer) != comparer
		)
		{
			return;
		}

		short xyMask = (short)(o ^ comparer);
		int x = TrailingZeroCount(xyMask), y = xyMask.GetNextSet(x);
		var inter = otherCellsMap.PeerIntersection - urCells;
		foreach (int possibleXyCell in inter)
		{
			if (grid.GetCandidates(possibleXyCell) != xyMask)
			{
				continue;
			}

			// 'xy' found.
			// Now check eliminations.
			var elimMap = inter & PeerMaps[possibleXyCell];
			var conclusions = new List<Conclusion>(10);
			foreach (int cell in elimMap)
			{
				if (grid.Exists(cell, x) is true)
				{
					conclusions.Add(new(ConclusionType.Elimination, cell, x));
				}
				if (grid.Exists(cell, y) is true)
				{
					conclusions.Add(new(ConclusionType.Elimination, cell, y));
				}
			}
			if (conclusions.Count == 0)
			{
				continue;
			}

			var candidateOffsets = new List<(int, ColorIdentifier)>(10);
			foreach (int cell in urCells)
			{
				if (otherCellsMap.Contains(cell))
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							(
								cell * 9 + digit,
								(ColorIdentifier)((comparer >> digit & 1) == 0 ? 1 : 0)
							)
						);
					}
				}
				else
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)0));
					}
				}
			}
			foreach (int digit in xyMask)
			{
				candidateOffsets.Add((possibleXyCell * 9 + digit, (ColorIdentifier)1));
			}

			if (IsIncompleteUr(candidateOffsets))
			{
				return;
			}

			accumulator.Add(
				new UniqueRectangle2DOr3XStep(
					ImmutableArray.CreateRange(conclusions),
					ImmutableArray.Create(new PresentationData
					{
						Cells = arMode ? GetHighlightCells(urCells) : null,
						Candidates = candidateOffsets
					}),
					arMode ? Technique.AvoidableRectangle2D : Technique.UniqueRectangle2D,
					d1,
					d2,
					urCells,
					arMode,
					x,
					y,
					possibleXyCell,
					index
				)
			);
		}
	}

	/// <summary>
	/// Check UR+2B/1SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
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
	/// The structure:
	/// <code><![CDATA[
	///'   ↓ corner1, corner2
	///' (ab )  (ab )
	///'  |
	///'  | a
	///'  |
	///'  abx    aby
	/// ]]></code>
	/// </remarks>
	private void Check2B1SL(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		bool arMode,
		short comparer,
		int d1,
		int d2,
		int corner1,
		int corner2,
		in Cells otherCellsMap,
		int index
	)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		int* corners = stackalloc[] { corner1, corner2 };
		int* digits = stackalloc[] { d1, d2 };
		for (int cellIndex = 0; cellIndex < 2; cellIndex++)
		{
			int cell = corners[cellIndex];
			foreach (int otherCell in otherCellsMap)
			{
				if (!IsSameRegionCell(cell, otherCell, out int regions))
				{
					continue;
				}

				foreach (int region in regions)
				{
					if (region < 9)
					{
						continue;
					}

					for (int digitIndex = 0; digitIndex < 2; digitIndex++)
					{
						int digit = digits[digitIndex];
						if (!IsConjugatePair(digit, new() { cell, otherCell }, region))
						{
							continue;
						}

						int elimCell = (otherCellsMap - otherCell)[0];
						if (grid.Exists(otherCell, digit) is not true)
						{
							continue;
						}

						int elimDigit = TrailingZeroCount(comparer ^ (1 << digit));
						var conclusions = new List<Conclusion>(4);
						if (grid.Exists(elimCell, elimDigit) is true)
						{
							conclusions.Add(new(ConclusionType.Elimination, elimCell, elimDigit));
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<(int, ColorIdentifier)>(10);
						foreach (int urCell in urCells)
						{
							if (urCell == corner1 || urCell == corner2)
							{
								int coveredRegions = new Cells { urCell, otherCell }.CoveredRegions;
								if ((coveredRegions >> region & 1) != 0)
								{
									foreach (int d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add(
											(
												urCell * 9 + d,
												(ColorIdentifier)(d == digit ? 1 : 0)
											)
										);
									}
								}
								else
								{
									foreach (int d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add((urCell * 9 + d, (ColorIdentifier)0));
									}
								}
							}
							else if (urCell == otherCell || urCell == elimCell)
							{
								if (grid.Exists(urCell, d1) is true)
								{
									if (urCell != elimCell || d1 != elimDigit)
									{
										candidateOffsets.Add(
											(
												urCell * 9 + d1,
												(ColorIdentifier)(urCell == elimCell ? 0 : (d1 == digit ? 1 : 0))
											)
										);
									}
								}
								if (grid.Exists(urCell, d2) is true)
								{
									if (urCell != elimCell || d2 != elimDigit)
									{
										candidateOffsets.Add(
											(
												urCell * 9 + d2,
												(ColorIdentifier)(urCell == elimCell ? 0 : (d2 == digit ? 1 : 0))
											)
										);
									}
								}
							}
						}

						if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
						{
							continue;
						}

						accumulator.Add(
							new UniqueRectangleWithConjugatePairStep(
								ImmutableArray.CreateRange(conclusions),
								ImmutableArray.Create(new PresentationData
								{
									Cells = arMode ? GetHighlightCells(urCells) : null,
									Candidates = candidateOffsets,
									Regions = new[] { (region, (ColorIdentifier)0) }
								}),
								Technique.UniqueRectangle2B1,
								d1,
								d2,
								urCells,
								arMode,
								new ConjugatePair[] { new(cell, otherCell, digit) },
								index
							)
						);
					}
				}
			}
		}
	}

	/// <summary>
	/// Check UR+2D/1SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
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
	/// The structure:
	/// <code><![CDATA[
	///'   ↓ corner1
	///' (ab )   aby
	///'  |
	///'  | a
	///'  |
	///'  abx   (ab )
	///'          ↑ corner2
	/// ]]></code>
	/// </remarks>
	private void Check2D1SL(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		bool arMode,
		short comparer,
		int d1,
		int d2,
		int corner1,
		int corner2,
		in Cells otherCellsMap,
		int index
	)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		int* corners = stackalloc[] { corner1, corner2 };
		int* digits = stackalloc[] { d1, d2 };
		for (int cellIndex = 0; cellIndex < 2; cellIndex++)
		{
			int cell = corners[cellIndex];
			foreach (int otherCell in otherCellsMap)
			{
				if (!IsSameRegionCell(cell, otherCell, out int regions))
				{
					continue;
				}

				foreach (int region in regions)
				{
					if (region < 9)
					{
						continue;
					}

					for (int digitIndex = 0; digitIndex < 2; digitIndex++)
					{
						int digit = digits[digitIndex];
						if (!IsConjugatePair(digit, new() { cell, otherCell }, region))
						{
							continue;
						}

						int elimCell = (otherCellsMap - otherCell)[0];
						if (grid.Exists(otherCell, digit) is not true)
						{
							continue;
						}

						var conclusions = new List<Conclusion>(4);
						if (grid.Exists(elimCell, digit) is true)
						{
							conclusions.Add(new(ConclusionType.Elimination, elimCell, digit));
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<(int, ColorIdentifier)>(10);
						foreach (int urCell in urCells)
						{
							if (urCell == corner1 || urCell == corner2)
							{
								bool flag = false;
								foreach (int r in new Cells { urCell, otherCell }.CoveredRegions)
								{
									if (r == region)
									{
										flag = true;
										break;
									}
								}

								if (flag)
								{
									foreach (int d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add(
											(
												urCell * 9 + d,
												(ColorIdentifier)(d == digit ? 1 : 0)
											)
										);
									}
								}
								else
								{
									foreach (int d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add((urCell * 9 + d, (ColorIdentifier)0));
									}
								}
							}
							else if (urCell == otherCell || urCell == elimCell)
							{
								if (grid.Exists(urCell, d1) is true && (urCell != elimCell || d1 != digit))
								{
									candidateOffsets.Add(
										(
											urCell * 9 + d1,
											(ColorIdentifier)(urCell == elimCell ? 0 : (d1 == digit ? 1 : 0))
										)
									);
								}
								if (grid.Exists(urCell, d2) is true && (urCell != elimCell || d2 != digit))
								{
									candidateOffsets.Add(
										(
											urCell * 9 + d2,
											(ColorIdentifier)(urCell == elimCell ? 0 : (d2 == digit ? 1 : 0))
										)
									);
								}
							}
						}

						if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
						{
							continue;
						}

						accumulator.Add(
							new UniqueRectangleWithConjugatePairStep(
								ImmutableArray.CreateRange(conclusions),
								ImmutableArray.Create(new PresentationData
								{
									Cells = arMode ? GetHighlightCells(urCells) : null,
									Candidates = candidateOffsets,
									Regions = new[] { (region, (ColorIdentifier)0) }
								}),
								Technique.UniqueRectangle2D1,
								d1,
								d2,
								urCells,
								arMode,
								new ConjugatePair[] { new(cell, otherCell, digit) },
								index
							)
						);
					}
				}
			}
		}
	}

	/// <summary>
	/// Check UR+3X.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///'  ↓ cornerCell
	///' (ab )  abx
	///'  aby   abz   xy  *
	/// ]]></code>
	/// Note: <c>z</c> is <c>x</c> or <c>y</c>.
	/// </remarks>
	private void Check3X(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		bool arMode,
		short comparer,
		int d1,
		int d2,
		int cornerCell,
		in Cells otherCellsMap,
		int index
	)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		int c1 = otherCellsMap[0], c2 = otherCellsMap[1], c3 = otherCellsMap[2];
		short m1 = grid.GetCandidates(c1), m2 = grid.GetCandidates(c2), m3 = grid.GetCandidates(c3);
		short mask = (short)((short)(m1 | m2) | m3);

		if (
			(
				TotalNumbersCount: PopCount((uint)mask),
				Cell1NumbersCount: PopCount((uint)m1),
				Cell2NumbersCount: PopCount((uint)m2),
				Cell3NumbersCount: PopCount((uint)m3),
				Cell1Intersection: m1 & comparer,
				Cell2Intersection: m2 & comparer,
				Cell3Intersection: m3 & comparer
			) is not (
				TotalNumbersCount: 4,
				Cell1NumbersCount: <= 3,
				Cell2NumbersCount: <= 3,
				Cell3NumbersCount: <= 3,
				Cell1Intersection: not 0,
				Cell2Intersection: not 0,
				Cell3Intersection: not 0
			) || (mask & comparer) != comparer
		)
		{
			return;
		}

		short xyMask = (short)(mask ^ comparer);
		int x = TrailingZeroCount(xyMask), y = xyMask.GetNextSet(x);
		var inter = otherCellsMap.PeerIntersection - urCells;
		foreach (int possibleXyCell in inter)
		{
			if (grid.GetCandidates(possibleXyCell) != xyMask)
			{
				continue;
			}

			// Possible XY cell found.
			// Now check eliminations.
			var conclusions = new List<Conclusion>(10);
			foreach (int cell in inter & PeerMaps[possibleXyCell])
			{
				if (grid.Exists(cell, x) is true) conclusions.Add(new(ConclusionType.Elimination, cell, x));
				if (grid.Exists(cell, y) is true) conclusions.Add(new(ConclusionType.Elimination, cell, y));
			}
			if (conclusions.Count == 0)
			{
				continue;
			}

			var candidateOffsets = new List<(int, ColorIdentifier)>(10);
			foreach (int cell in urCells)
			{
				if (otherCellsMap.Contains(cell))
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							(
								cell * 9 + digit,
								(ColorIdentifier)((comparer >> digit & 1) == 0 ? 1 : 0)
							)
						);
					}
				}
				else
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)0));
					}
				}
			}
			foreach (int digit in xyMask)
			{
				candidateOffsets.Add((possibleXyCell * 9 + digit, (ColorIdentifier)1));
			}
			if (IsIncompleteUr(candidateOffsets))
			{
				return;
			}

			accumulator.Add(
				new UniqueRectangle2DOr3XStep(
					ImmutableArray.CreateRange(conclusions),
					ImmutableArray.Create(new PresentationData
					{
						Cells = arMode ? GetHighlightCells(urCells) : null,
						Candidates = candidateOffsets
					}),
					arMode ? Technique.AvoidableRectangle3X : Technique.UniqueRectangle3X,
					d1,
					d2,
					urCells,
					arMode,
					x,
					y,
					possibleXyCell,
					index
				)
			);
		}
	}

	/// <summary>
	/// Check UR+3X/2SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///'  ↓ cornerCell
	///' (ab )    abx
	///'           |
	///'           | b
	///'       a   |
	///'  aby-----abz
	/// ]]></code>
	/// </remarks>
	private void Check3X2SL(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		bool arMode,
		short comparer,
		int d1,
		int d2,
		int cornerCell,
		in Cells otherCellsMap,
		int index
	)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		int abzCell = GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		var pairs = stackalloc[] { (d1, d2), (d2, d1) };
		for (int pairIndex = 0; pairIndex < 2; pairIndex++)
		{
			var (a, b) = pairs[pairIndex];
			int abxCell = adjacentCellsMap[0], abyCell = adjacentCellsMap[1];
			Cells map1 = new() { abzCell, abxCell }, map2 = new() { abzCell, abyCell };
			if (!IsConjugatePair(b, map1, map1.CoveredLine) || !IsConjugatePair(a, map2, map2.CoveredLine))
			{
				continue;
			}

			var conclusions = new List<Conclusion>(2);
			if (grid.Exists(abxCell, a) is true) conclusions.Add(new(ConclusionType.Elimination, abxCell, a));
			if (grid.Exists(abyCell, b) is true) conclusions.Add(new(ConclusionType.Elimination, abyCell, b));
			if (conclusions.Count == 0)
			{
				continue;
			}

			var candidateOffsets = new List<(int, ColorIdentifier)>(6);
			foreach (int digit in grid.GetCandidates(abxCell))
			{
				if ((digit == d1 || digit == d2) && digit != a)
				{
					candidateOffsets.Add((abxCell * 9 + digit, (ColorIdentifier)(digit == b ? 1 : 0)));
				}
			}
			foreach (int digit in grid.GetCandidates(abyCell))
			{
				if ((digit == d1 || digit == d2) && digit != b)
				{
					candidateOffsets.Add((abyCell * 9 + digit, (ColorIdentifier)(digit == a ? 1 : 0)));
				}
			}
			foreach (int digit in grid.GetCandidates(abzCell))
			{
				if (digit == a || digit == b)
				{
					candidateOffsets.Add((abzCell * 9 + digit, (ColorIdentifier)1));
				}
			}
			foreach (int digit in comparer)
			{
				candidateOffsets.Add((cornerCell * 9 + digit, (ColorIdentifier)0));
			}
			if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 6)
			{
				continue;
			}

			accumulator.Add(
				new UniqueRectangleWithConjugatePairStep(
					ImmutableArray.CreateRange(conclusions),
					ImmutableArray.Create(new PresentationData
					{
						Cells = arMode ? GetHighlightCells(urCells) : null,
						Candidates = candidateOffsets,
						Regions = new[]
						{
							(map1.CoveredLine, (ColorIdentifier)0),
							(map2.CoveredLine, (ColorIdentifier)1)
						}
					}),
					Technique.UniqueRectangle3X2,
					d1,
					d2,
					urCells,
					arMode,
					new ConjugatePair[] { new(abxCell, abzCell, b), new(abyCell, abzCell, a) },
					index
				)
			);
		}
	}

	/// <summary>
	/// Check UR+3N/2SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///'  ↓ cornerCell
	///' (ab )-----abx
	///'        a   |
	///'            | b
	///'            |
	///'  aby      abz
	/// ]]></code>
	/// </remarks>
	private void Check3N2SL(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		bool arMode,
		short comparer,
		int d1,
		int d2,
		int cornerCell,
		in Cells otherCellsMap,
		int index
	)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		// Step 1: Get the diagonal cell of 'cornerCell' and determine
		// the existence of strong link.
		int abzCell = GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		int abxCell = adjacentCellsMap[0], abyCell = adjacentCellsMap[1];
		var cellPairs = stackalloc[] { (abxCell, abyCell), (abyCell, abxCell) };
		var digitPairs = stackalloc[] { (d1, d2), (d2, d1) };
		int* digits = stackalloc[] { d1, d2 };
		for (int cellPairIndex = 0; cellPairIndex < 2; cellPairIndex++)
		{
			var (begin, end) = cellPairs[cellPairIndex];
			var linkMap = new Cells { begin, abzCell };
			for (int digitPairIndex = 0; digitPairIndex < 2; digitPairIndex++)
			{
				var (a, b) = digitPairs[digitPairIndex];
				if (!IsConjugatePair(b, linkMap, linkMap.CoveredLine))
				{
					continue;
				}

				// Step 2: Get the link cell that is adjacent to 'cornerCell'
				// and check the strong link.
				var secondLinkMap = new Cells { cornerCell, begin };
				if (!IsConjugatePair(a, secondLinkMap, secondLinkMap.CoveredLine))
				{
					continue;
				}

				// Step 3: Check eliminations.
				if (grid.Exists(end, a) is not true)
				{
					continue;
				}

				// Step 4: Check highlight candidates.
				var candidateOffsets = new List<(int, ColorIdentifier)>(7);
				foreach (int d in comparer)
				{
					candidateOffsets.Add((cornerCell * 9 + d, (ColorIdentifier)(d == a ? 1 : 0)));
				}
				for (int digitIndex = 0; digitIndex < 2; digitIndex++)
				{
					int d = digits[digitIndex];
					if (grid.Exists(abzCell, d) is true)
					{
						candidateOffsets.Add((abzCell * 9 + d, (ColorIdentifier)(d == b ? 1 : 0)));
					}
				}
				foreach (int d in grid.GetCandidates(begin))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add((begin * 9 + d, (ColorIdentifier)1));
					}
				}
				foreach (int d in grid.GetCandidates(end))
				{
					if ((d == d1 || d == d2) && d != a)
					{
						candidateOffsets.Add((end * 9 + d, (ColorIdentifier)0));
					}
				}
				if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
				{
					continue;
				}

				var conjugatePairs = new ConjugatePair[] { new(cornerCell, begin, a), new(begin, abzCell, b) };
				accumulator.Add(
					new UniqueRectangleWithConjugatePairStep(
						ImmutableArray.Create(new Conclusion(ConclusionType.Elimination, end, a)),
						ImmutableArray.Create(new PresentationData
						{
							Cells = arMode ? GetHighlightCells(urCells) : null,
							Candidates = candidateOffsets,
							Regions = new[]
							{
								(conjugatePairs[0].Line, (ColorIdentifier)0),
								(conjugatePairs[1].Line, (ColorIdentifier)1)
							}
						}),
						Technique.UniqueRectangle3N2,
						d1,
						d2,
						urCells,
						arMode,
						conjugatePairs,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR+3U/2SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///'  ↓ cornerCell
	///' (ab )-----abx
	///'        a
	///'
	///'        b
	///'  aby -----abz
	/// ]]></code>
	/// </remarks>
	private void Check3U2SL(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		bool arMode,
		short comparer,
		int d1,
		int d2,
		int cornerCell,
		in Cells otherCellsMap,
		int index
	)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		int abzCell = GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		int abxCell = adjacentCellsMap[0], abyCell = adjacentCellsMap[1];
		var cellPairs = stackalloc[] { (abxCell, abyCell), (abyCell, abxCell) };
		var digitPairs = stackalloc[] { (d1, d2), (d2, d1) };
		for (int cellPairIndex = 0; cellPairIndex < 2; cellPairIndex++)
		{
			var (begin, end) = cellPairs[cellPairIndex];
			var linkMap = new Cells { begin, abzCell };
			for (int digitPairIndex = 0; digitPairIndex < 2; digitPairIndex++)
			{
				var (a, b) = digitPairs[digitPairIndex];
				if (!IsConjugatePair(b, linkMap, linkMap.CoveredLine))
				{
					continue;
				}

				var secondLinkMap = new Cells { cornerCell, end };
				if (!IsConjugatePair(a, secondLinkMap, secondLinkMap.CoveredLine))
				{
					continue;
				}

				if (grid.Exists(begin, a) is not true)
				{
					continue;
				}

				var candidateOffsets = new List<(int, ColorIdentifier)>(7);
				foreach (int d in comparer)
				{
					candidateOffsets.Add((cornerCell * 9 + d, (ColorIdentifier)(d == a ? 1 : 0)));
				}
				foreach (int d in grid.GetCandidates(begin))
				{
					if ((d == d1 || d == d2) && d != a)
					{
						candidateOffsets.Add((begin * 9 + d, (ColorIdentifier)1));
					}
				}
				foreach (int d in grid.GetCandidates(end))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add((end * 9 + d, (ColorIdentifier)(d == a ? 1 : 0)));
					}
				}
				foreach (int d in grid.GetCandidates(abzCell))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add((abzCell * 9 + d, (ColorIdentifier)(d == b ? 1 : 0)));
					}
				}
				if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
				{
					continue;
				}

				var conjugatePairs = new ConjugatePair[] { new(cornerCell, end, a), new(begin, abzCell, b) };
				accumulator.Add(
					new UniqueRectangleWithConjugatePairStep(
						ImmutableArray.Create(new Conclusion(ConclusionType.Elimination, begin, a)),
						ImmutableArray.Create(new PresentationData
						{
							Cells = arMode ? GetHighlightCells(urCells) : null,
							Candidates = candidateOffsets,
							Regions = new[]
							{
								(conjugatePairs[0].Line, (ColorIdentifier)0),
								(conjugatePairs[1].Line, (ColorIdentifier)1)
							}
						}),
						Technique.UniqueRectangle3U2,
						d1,
						d2,
						urCells,
						arMode,
						conjugatePairs,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR+3E/2SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///'   ↓ cornerCell
	///' (ab )-----abx
	///'        a
	///'
	///'        a
	///'  aby -----abz
	/// ]]></code>
	/// </remarks>
	private void Check3E2SL(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		bool arMode,
		short comparer,
		int d1,
		int d2,
		int cornerCell,
		in Cells otherCellsMap,
		int index
	)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		int abzCell = GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		int abxCell = adjacentCellsMap[0], abyCell = adjacentCellsMap[1];
		var cellPairs = stackalloc[] { (abxCell, abyCell), (abyCell, abxCell) };
		var digitPairs = stackalloc[] { (d1, d2), (d2, d1) };
		for (int cellPairIndex = 0; cellPairIndex < 2; cellPairIndex++)
		{
			var (begin, end) = cellPairs[cellPairIndex];
			var linkMap = new Cells { begin, abzCell };
			for (int digitPairIndex = 0; digitPairIndex < 2; digitPairIndex++)
			{
				var (a, b) = digitPairs[digitPairIndex];
				if (!IsConjugatePair(a, linkMap, linkMap.CoveredLine))
				{
					continue;
				}

				var secondLinkMap = new Cells { cornerCell, end };
				if (!IsConjugatePair(a, secondLinkMap, secondLinkMap.CoveredLine))
				{
					continue;
				}

				if (grid.Exists(abzCell, b) is not true)
				{
					continue;
				}

				var candidateOffsets = new List<(int, ColorIdentifier)>(7);
				foreach (int d in comparer)
				{
					candidateOffsets.Add((cornerCell * 9 + d, (ColorIdentifier)(d == a ? 1 : 0)));
				}
				foreach (int d in grid.GetCandidates(begin))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add((begin * 9 + d, (ColorIdentifier)(d == a ? 1 : 0)));
					}
				}
				foreach (int d in grid.GetCandidates(end))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add((end * 9 + d, (ColorIdentifier)(d == a ? 1 : 0)));
					}
				}
				foreach (int d in grid.GetCandidates(abzCell))
				{
					if ((d == d1 || d == d2) && d != b)
					{
						candidateOffsets.Add((abzCell * 9 + d, (ColorIdentifier)(d == a ? 1 : 0)));
					}
				}
				if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
				{
					continue;
				}

				var conjugatePairs = new ConjugatePair[] { new(cornerCell, end, a), new(begin, abzCell, a) };
				accumulator.Add(
					new UniqueRectangleWithConjugatePairStep(
						ImmutableArray.Create(new Conclusion(ConclusionType.Elimination, abzCell, b)),
						ImmutableArray.Create(new PresentationData
						{
							Cells = arMode ? GetHighlightCells(urCells) : null,
							Candidates = candidateOffsets,
							Regions = new[]
							{
								(conjugatePairs[0].Line, (ColorIdentifier)0),
								(conjugatePairs[1].Line, (ColorIdentifier)1)
							}
						}),
						Technique.UniqueRectangle3E2,
						d1,
						d2,
						urCells,
						arMode,
						conjugatePairs,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR+4X/3SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
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
	/// The structure:
	/// <code><![CDATA[
	///'   ↓ corner1, corner2
	///' (abx)-----(aby)
	///'        a    |
	///'             | b
	///'        a    |
	///'  abz ----- abw
	/// ]]></code>
	/// </remarks>
	private void Check4X3SL(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		bool arMode,
		short comparer,
		int d1,
		int d2,
		int corner1,
		int corner2,
		in Cells otherCellsMap,
		int index
	)
	{
		var link1Map = new Cells { corner1, corner2 };
		var digitPairs = stackalloc[] { (d1, d2), (d2, d1) };
		for (int digitPairIndex = 0; digitPairIndex < 2; digitPairIndex++)
		{
			var (a, b) = digitPairs[digitPairIndex];
			if (!IsConjugatePair(a, link1Map, link1Map.CoveredLine))
			{
				continue;
			}

			int abwCell = GetDiagonalCell(urCells, corner1);
			int abzCell = (otherCellsMap - abwCell)[0];
			var cellQuadruples = stackalloc[]
			{
				(corner2, corner1, abzCell, abwCell),
				(corner1, corner2, abwCell, abzCell)
			};

			for (int cellQuadrupleIndex = 0; cellQuadrupleIndex < 2; cellQuadrupleIndex++)
			{
				var (head, begin, end, extra) = cellQuadruples[cellQuadrupleIndex];
				var link2Map = new Cells { begin, end };
				if (!IsConjugatePair(b, link2Map, link2Map.CoveredLine))
				{
					continue;
				}

				var link3Map = new Cells { end, extra };
				if (!IsConjugatePair(a, link3Map, link3Map.CoveredLine))
				{
					continue;
				}

				var conclusions = new List<Conclusion>(2);
				if (grid.Exists(head, b) is true) conclusions.Add(new(ConclusionType.Elimination, head, b));
				if (grid.Exists(extra, b) is true) conclusions.Add(new(ConclusionType.Elimination, extra, b));
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<(int, ColorIdentifier)>(6);
				foreach (int d in grid.GetCandidates(head))
				{
					if ((d == d1 || d == d2) && d != b)
					{
						candidateOffsets.Add((head * 9 + d, (ColorIdentifier)1));
					}
				}
				foreach (int d in grid.GetCandidates(extra))
				{
					if ((d == d1 || d == d2) && d != b)
					{
						candidateOffsets.Add((extra * 9 + d, (ColorIdentifier)1));
					}
				}
				foreach (int d in grid.GetCandidates(begin))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add((begin * 9 + d, (ColorIdentifier)1));
					}
				}
				foreach (int d in grid.GetCandidates(end))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add((end * 9 + d, (ColorIdentifier)1));
					}
				}
				if (!AllowIncompleteUniqueRectangles && (candidateOffsets.Count, conclusions.Count) != (6, 2))
				{
					continue;
				}

				var conjugatePairs = new ConjugatePair[]
				{
					new(head, begin, a),
					new(begin, end, b),
					new(end, extra, a)
				};
				accumulator.Add(
					new UniqueRectangleWithConjugatePairStep(
						ImmutableArray.CreateRange(conclusions),
						ImmutableArray.Create(new PresentationData
						{
							Cells = arMode ? GetHighlightCells(urCells) : null,
							Candidates = candidateOffsets,
							Regions = new[]
							{
								(conjugatePairs[0].Line, (ColorIdentifier)0),
								(conjugatePairs[1].Line, (ColorIdentifier)1),
								(conjugatePairs[2].Line, (ColorIdentifier)0)
							}
						}),
						Technique.UniqueRectangle4X3,
						d1,
						d2,
						urCells,
						arMode,
						conjugatePairs,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR+4C/3SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
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
	///'   ↓ corner1, corner2
	///' (abx)-----(aby)
	///'        a    |
	///'             | a
	///'        b    |
	///'  abz ----- abw
	/// ]]></code>
	/// </para>
	/// <para>
	/// Subtype 2:
	/// <code><![CDATA[
	///'   ↓ corner1, corner2
	///' (abx)-----(aby)
	///'   |    a    |
	///'   | b       | a
	///'   |         |
	///'  abz       abw
	/// ]]></code>
	/// </para>
	/// </remarks>
	private void Check4C3SL(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		bool arMode,
		short comparer,
		int d1,
		int d2,
		int corner1,
		int corner2,
		in Cells otherCellsMap,
		int index
	)
	{
		var link1Map = new Cells { corner1, corner2 };
		var innerMaps = stackalloc Cells[2];
		var digitPairs = stackalloc[] { (d1, d2), (d2, d1) };
		for (int digitPairIndex = 0; digitPairIndex < 2; digitPairIndex++)
		{
			var (a, b) = digitPairs[digitPairIndex];
			if (!IsConjugatePair(a, link1Map, link1Map.CoveredLine))
			{
				continue;
			}

			int end = GetDiagonalCell(urCells, corner1);
			int extra = (otherCellsMap - end)[0];
			var cellQuadruples = stackalloc[] { (corner2, corner1, extra, end), (corner1, corner2, end, extra) };
			for (int cellQuadrupleIndex = 0; cellQuadrupleIndex < 2; cellQuadrupleIndex++)
			{
				var (abx, aby, abw, abz) = cellQuadruples[cellQuadrupleIndex];
				var link2Map = new Cells { aby, abw };
				if (!IsConjugatePair(a, link2Map, link2Map.CoveredLine))
				{
					continue;
				}

				Cells link3Map1 = new() { abw, abz }, link3Map2 = new() { abx, abz };
				innerMaps[0] = link3Map1;
				innerMaps[1] = link3Map2;
				for (int i = 0; i < 2; i++)
				{
					var linkMap = innerMaps[i];
					if (!IsConjugatePair(b, link3Map1, link3Map1.CoveredLine))
					{
						continue;
					}

					if (grid.Exists(aby, b) is not true)
					{
						continue;
					}

					var candidateOffsets = new List<(int, ColorIdentifier)>(7);
					foreach (int d in grid.GetCandidates(abx))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add((abx * 9 + d, (ColorIdentifier)(i == 0 ? d == a ? 1 : 0 : 1)));
						}
					}
					foreach (int d in grid.GetCandidates(abz))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add((abz * 9 + d, (ColorIdentifier)(d == b ? 1 : 0)));
						}
					}
					foreach (int d in grid.GetCandidates(aby))
					{
						if ((d == d1 || d == d2) && d != b)
						{
							candidateOffsets.Add((aby * 9 + d, (ColorIdentifier)1));
						}
					}
					foreach (int d in grid.GetCandidates(abw))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add((abw * 9 + d, (ColorIdentifier)1));
						}
					}
					if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
					{
						continue;
					}

					int[] offsets = linkMap.ToArray();
					var conjugatePairs = new ConjugatePair[]
					{
						new(abx, aby, a),
						new(aby, abw, a),
						new(offsets[0], offsets[1], b)
					};
					accumulator.Add(
						new UniqueRectangleWithConjugatePairStep(
							ImmutableArray.Create(new Conclusion(ConclusionType.Elimination, aby, b)),
							ImmutableArray.Create(new PresentationData
							{
								Cells = arMode ? GetHighlightCells(urCells) : null,
								Candidates = candidateOffsets,
								Regions = new[]
								{
									(conjugatePairs[0].Line, (ColorIdentifier)0),
									(conjugatePairs[1].Line, (ColorIdentifier)0),
									(conjugatePairs[2].Line, (ColorIdentifier)1)
								}
							}),
							Technique.UniqueRectangle4C3,
							d1,
							d2,
							urCells,
							arMode,
							conjugatePairs,
							index
						)
					);
				}
			}
		}
	}
	#endregion

	#region With-structured types implementation
	/// <summary>
	/// Check UR-XY-Wing, UR-XYZ-Wing, UR-WXYZ-Wing and AR-XY-Wing, AR-XYZ-Wing and AR-WXYZ-Wing.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="size">The size of the wing to search.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// <para>The structures:</para>
	/// <para>
	/// Subtype 1:
	/// <code><![CDATA[
	///'   ↓ corner1
	///' (ab )  abxy  yz  xz
	///' (ab )  abxy  *
	///'   ↑ corner2
	/// ]]></code>
	/// Note that the pair of cells <c>abxy</c> should be in the same region.
	/// </para>
	/// <para>
	/// Subtype 2:
	/// <code><![CDATA[
	///'   ↓ corner1
	///' (ab )  abx   xz
	///'  aby  (ab )  *   yz
	///'         ↑ corner2
	/// ]]></code>
	/// </para>
	/// </remarks>
	private void CheckWing(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		bool arMode,
		short comparer,
		int d1,
		int d2,
		int corner1,
		int corner2,
		in Cells otherCellsMap,
		int size,
		int index
	)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		if (new Cells { corner1, corner2 }.AllSetsAreInOneRegion(out int region) && region < 9)
		{
			#region Subtype 1
			// Subtype 1.
			int[] offsets = otherCellsMap.ToArray();
			int otherCell1 = offsets[0], otherCell2 = offsets[1];
			short mask1 = grid.GetCandidates(otherCell1);
			short mask2 = grid.GetCandidates(otherCell2);
			short mask = (short)(mask1 | mask2);

			if (PopCount((uint)mask) != 2 + size || (mask & comparer) != comparer
				|| mask1 == comparer || mask2 == comparer)
			{
				return;
			}

			var map = (PeerMaps[otherCell1] | PeerMaps[otherCell2]) & BivalueMap;
			if (map.Count < size)
			{
				return;
			}

			var testMap = new Cells { otherCell1, otherCell2 }.PeerIntersection;
			short extraDigitsMask = (short)(mask ^ comparer);
			int[] cells = map.ToArray();
			for (int i1 = 0, length = cells.Length, outerLength = length - size + 1; i1 < outerLength; i1++)
			{
				int c1 = cells[i1];
				short m1 = grid.GetCandidates(c1);
				if ((m1 & ~extraDigitsMask) == 0)
				{
					continue;
				}

				for (int i2 = i1 + 1, lengthMinusSizePlus2 = length - size + 2; i2 < lengthMinusSizePlus2; i2++)
				{
					int c2 = cells[i2];
					short m2 = grid.GetCandidates(c2);
					if ((m2 & ~extraDigitsMask) == 0)
					{
						continue;
					}

					if (size == 2)
					{
						// Check XY-Wing.
						short m = (short)((short)(m1 | m2) ^ extraDigitsMask);
						if ((PopCount((uint)m), PopCount((uint)(m1 & m2))) != (1, 1))
						{
							continue;
						}

						// Now check whether all cells found should see their corresponding
						// cells in UR structure ('otherCells1' or 'otherCells2').
						bool flag = true;
						int* combinationCells = stackalloc[] { c1, c2 };
						for (int cellIndex = 0; cellIndex < 2; cellIndex++)
						{
							int cell = combinationCells[cellIndex];
							int extraDigit = TrailingZeroCount(grid.GetCandidates(cell) & ~m);
							if (!(testMap & CandMaps[extraDigit]).Contains(cell))
							{
								flag = false;
								break;
							}
						}
						if (!flag)
						{
							continue;
						}

						// Now check eliminations.
						int elimDigit = TrailingZeroCount(m);
						var elimMap = new Cells { c1, c2 }.PeerIntersection & CandMaps[elimDigit];
						if (elimMap.IsEmpty)
						{
							continue;
						}

						var candidateOffsets = new List<(int, ColorIdentifier)>(12);
						foreach (int cell in urCells)
						{
							if (grid.GetStatus(cell) == CellStatus.Empty)
							{
								foreach (int digit in grid.GetCandidates(cell))
								{
									candidateOffsets.Add(
										(
											cell * 9 + digit,
											(ColorIdentifier)(
												digit == elimDigit
													? otherCellsMap.Contains(cell) ? 2 : 0
													: (extraDigitsMask >> digit & 1) != 0 ? 1 : 0
											)
										)
									);
								}
							}
						}
						foreach (int digit in grid.GetCandidates(c1))
						{
							candidateOffsets.Add((c1 * 9 + digit, (ColorIdentifier)(digit == elimDigit ? 2 : 1)));
						}
						foreach (int digit in grid.GetCandidates(c2))
						{
							candidateOffsets.Add((c2 * 9 + digit, (ColorIdentifier)(digit == elimDigit ? 2 : 1)));
						}
						if (IsIncompleteUr(candidateOffsets))
						{
							return;
						}

						accumulator.Add(
							new UniqueRectangleWithWingStep(
								elimMap.ToImmutableConclusions(elimDigit),
								ImmutableArray.Create(new PresentationData
								{
									Cells = arMode ? GetHighlightCells(urCells) : null,
									Candidates = candidateOffsets
								}),
								arMode
									? Technique.AvoidableRectangleXyWing
									: Technique.UniqueRectangleXyWing,
								d1,
								d2,
								urCells,
								arMode,
								new() { c1, c2 },
								otherCellsMap,
								extraDigitsMask,
								index
							)
						);
					}
					else // size > 2
					{
						for (
							int i3 = i2 + 1, lengthMinusSizePlus3 = length - size + 3;
							i3 < lengthMinusSizePlus3;
							i3++
						)
						{
							int c3 = cells[i3];
							short m3 = grid.GetCandidates(c3);
							if ((m3 & ~extraDigitsMask) == 0)
							{
								continue;
							}

							if (size == 3)
							{
								// Check XYZ-Wing.
								short m = (short)(((short)(m1 | m2) | m3) ^ extraDigitsMask);
								if ((PopCount((uint)m), PopCount((uint)(m1 & m2 & m3))) != (1, 1))
								{
									continue;
								}

								// Now check whether all cells found should see their corresponding
								// cells in UR structure ('otherCells1' or 'otherCells2').
								bool flag = true;
								int* combinationCells = stackalloc[] { c1, c2, c3 };
								for (int cellIndex = 0; cellIndex < 3; cellIndex++)
								{
									int cell = combinationCells[cellIndex];
									int extraDigit = TrailingZeroCount(grid.GetCandidates(cell) & ~m);
									if (!(testMap & CandMaps[extraDigit]).Contains(cell))
									{
										flag = false;
										break;
									}
								}
								if (!flag)
								{
									continue;
								}

								// Now check eliminations.
								int elimDigit = TrailingZeroCount(m);
								var elimMap = new Cells { c1, c2, c3 }.PeerIntersection & CandMaps[elimDigit];
								if (elimMap.IsEmpty)
								{
									continue;
								}

								var candidateOffsets = new List<(int, ColorIdentifier)>();
								foreach (int cell in urCells)
								{
									if (grid.GetStatus(cell) == CellStatus.Empty)
									{
										foreach (int digit in grid.GetCandidates(cell))
										{
											candidateOffsets.Add(
												(
													cell * 9 + digit,
													(ColorIdentifier)((extraDigitsMask >> digit & 1) != 0 ? 1 : 0)
												)
											);
										}
									}
								}
								foreach (int digit in grid.GetCandidates(c1))
								{
									candidateOffsets.Add(
										(
											c1 * 9 + digit,
											(ColorIdentifier)(digit == elimDigit ? 2 : 1)
										)
									);
								}
								foreach (int digit in grid.GetCandidates(c2))
								{
									candidateOffsets.Add(
										(
											c2 * 9 + digit,
											(ColorIdentifier)(digit == elimDigit ? 2 : 1)
										)
									);
								}
								foreach (int digit in grid.GetCandidates(c3))
								{
									candidateOffsets.Add(
										(
											c3 * 9 + digit,
											(ColorIdentifier)(digit == elimDigit ? 2 : 1)
										)
									);
								}
								if (IsIncompleteUr(candidateOffsets))
								{
									return;
								}

								accumulator.Add(
									new UniqueRectangleWithWingStep(
										elimMap.ToImmutableConclusions(elimDigit),
										ImmutableArray.Create(new PresentationData
										{
											Cells = arMode ? GetHighlightCells(urCells) : null,
											Candidates = candidateOffsets
										}),
										arMode
											? Technique.AvoidableRectangleXyzWing
											: Technique.UniqueRectangleXyzWing,
										d1,
										d2,
										urCells,
										arMode,
										new() { c1, c2, c3 },
										otherCellsMap,
										extraDigitsMask,
										index
									)
								);
							}
							else // size == 4
							{
								for (int i4 = i3 + 1; i4 < length; i4++)
								{
									int c4 = cells[i4];
									short m4 = grid.GetCandidates(c4);
									if ((m4 & ~extraDigitsMask) == 0)
									{
										continue;
									}

									// Check WXYZ-Wing.
									short m = (short)((short)((short)((short)(m1 | m2) | m3) | m4) ^ extraDigitsMask);
									if ((PopCount((uint)m), PopCount((uint)(m1 & m2 & m3 & m4))) != (1, 1))
									{
										continue;
									}

									// Now check whether all cells found should see their corresponding
									// cells in UR structure ('otherCells1' or 'otherCells2').
									bool flag = true;
									int* combinationCells = stackalloc[] { c1, c2, c3, c4 };
									for (int cellIndex = 0; cellIndex < 4; cellIndex++)
									{
										int cell = combinationCells[cellIndex];
										int extraDigit = TrailingZeroCount(grid.GetCandidates(cell) & ~m);
										if (!(testMap & CandMaps[extraDigit]).Contains(cell))
										{
											flag = false;
											break;
										}
									}
									if (!flag)
									{
										continue;
									}

									// Now check eliminations.
									int elimDigit = TrailingZeroCount(m);
									var elimMap = new Cells { c1, c2, c3, c4 }.PeerIntersection & CandMaps[elimDigit];
									if (elimMap.IsEmpty)
									{
										continue;
									}

									var candidateOffsets = new List<(int, ColorIdentifier)>();
									foreach (int cell in urCells)
									{
										if (grid.GetStatus(cell) == CellStatus.Empty)
										{
											foreach (int digit in grid.GetCandidates(cell))
											{
												candidateOffsets.Add(
													(
														cell * 9 + digit,
														(ColorIdentifier)(
															(extraDigitsMask >> digit & 1) != 0 ? 1 : 0
														)
													)
												);
											}
										}
									}
									foreach (int digit in grid.GetCandidates(c1))
									{
										candidateOffsets.Add(
											(
												c1 * 9 + digit,
												(ColorIdentifier)(digit == elimDigit ? 2 : 1)
											)
										);
									}
									foreach (int digit in grid.GetCandidates(c2))
									{
										candidateOffsets.Add(
											(
												c2 * 9 + digit,
												(ColorIdentifier)(digit == elimDigit ? 2 : 1)
											)
										);
									}
									foreach (int digit in grid.GetCandidates(c3))
									{
										candidateOffsets.Add(
											(
												c3 * 9 + digit,
												(ColorIdentifier)(digit == elimDigit ? 2 : 1)
											)
										);
									}
									foreach (int digit in grid.GetCandidates(c4))
									{
										candidateOffsets.Add(
											(
												c4 * 9 + digit,
												(ColorIdentifier)(digit == elimDigit ? 2 : 1)
											)
										);
									}
									if (IsIncompleteUr(candidateOffsets))
									{
										return;
									}

									accumulator.Add(
										new UniqueRectangleWithWingStep(
											elimMap.ToImmutableConclusions(elimDigit),
											ImmutableArray.Create(new PresentationData
											{
												Cells = arMode ? GetHighlightCells(urCells) : null,
												Candidates = candidateOffsets
											}),
											arMode
												? Technique.AvoidableRectangleWxyzWing
												: Technique.UniqueRectangleWxyzWing,
											d1,
											d2,
											urCells,
											arMode,
											new() { c1, c2, c3, c4 },
											otherCellsMap,
											extraDigitsMask,
											index
										)
									);
								}
							}
						}
					}
				}
			}
			#endregion
		}
		else
		{
			#region Subtype 2
			// TODO: Finish processing Subtype 2.
			#endregion
		}
	}

	/// <summary>
	/// Check UR+SdC.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
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
	/// The structure:
	/// <code><![CDATA[
	///'           |   xyz
	///'  ab+ ab+  | abxyz abxyz
	///'           |   xyz
	///' ----------+------------
	///' (ab)(ab)  |
	///'  ↑ corner1, corner2
	/// ]]></code>
	/// </remarks>
	private void CheckSdc(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		bool arMode,
		short comparer,
		int d1,
		int d2,
		int corner1,
		int corner2,
		in Cells otherCellsMap,
		int index
	)
	{
		bool notSatisfiedType3 = false;
		short mergedMaskInOtherCells = 0;
		foreach (int cell in otherCellsMap)
		{
			short currentMask = grid.GetCandidates(cell);
			mergedMaskInOtherCells |= currentMask;
			if ((currentMask & comparer) == 0
				|| currentMask == comparer || arMode && grid.GetStatus(cell) != CellStatus.Empty)
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

		// Check whether the corners spanned two blocks. If so, UR+SdC can't be found.
		short blockMaskInOtherCells = otherCellsMap.BlockMask;
		if (!IsPow2(blockMaskInOtherCells))
		{
			return;
		}

		short otherDigitsMask = (short)(mergedMaskInOtherCells & ~comparer);
		byte line = (byte)otherCellsMap.CoveredLine;
		byte block = (byte)TrailingZeroCount(otherCellsMap.CoveredRegions & ~(1 << line));
		var (a, _, _, d) = IntersectionMaps[(line, block)];
		var list = new List<Cells>(4);
		bool* cannibalModes = stackalloc[] { false, true };
		for (int cannibalModeIndex = 0; cannibalModeIndex < 2; cannibalModeIndex++)
		{
			bool cannibalMode = cannibalModes[cannibalModeIndex];
			foreach (byte otherBlock in d)
			{
				var emptyCellsInInterMap = RegionMaps[otherBlock] & RegionMaps[line] & EmptyMap;
				if (emptyCellsInInterMap.Count < 2)
				{
					// The intersection needs at least two empty cells.
					continue;
				}

				Cells b = RegionMaps[otherBlock] - RegionMaps[line], c = a & b;

				list.Clear();
				int[] offsets = emptyCellsInInterMap.ToArray();
				switch (emptyCellsInInterMap.Count)
				{
					case 2:
					{
						list.Add(new() { offsets[0], offsets[1] });

						break;
					}
					case 3:
					{
						int i = offsets[0], j = offsets[1], k = offsets[2];
						list.Add(new() { i, j });
						list.Add(new() { j, k });
						list.Add(new() { i, k });
						list.Add(new() { i, j, k });

						break;
					}
				}

				// Iterate on each intersection combination.
				foreach (var currentInterMap in list)
				{
					short selectedInterMask = 0;
					foreach (int cell in currentInterMap)
					{
						selectedInterMask |= grid.GetCandidates(cell);
					}
					if (PopCount((uint)selectedInterMask) <= currentInterMap.Count + 1)
					{
						// The intersection combination is an ALS or a normal subset,
						// which is invalid in SdCs.
						continue;
					}

					var blockMap = (b | c - currentInterMap) & EmptyMap;
					var lineMap = (a & EmptyMap) - otherCellsMap;

					// Iterate on the number of the cells that should be selected in block.
					for (int i = 1; i <= blockMap.Count - 1; i++)
					{
						// Iterate on each combination in block.
						foreach (int[] selectedCellsInBlock in blockMap.ToArray().GetSubsets(i))
						{
							bool flag = false;
							foreach (int digit in otherDigitsMask)
							{
								foreach (int cell in selectedCellsInBlock)
								{
									if (grid.Exists(cell, digit) is true)
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

							var currentBlockMap = new Cells(selectedCellsInBlock);
							Cells elimMapBlock = Cells.Empty, elimMapLine = Cells.Empty;

							// Get the links of the block.
							short blockMask = 0;
							foreach (int cell in selectedCellsInBlock)
							{
								blockMask |= grid.GetCandidates(cell);
							}

							// Get the elimination map in the block.
							foreach (int digit in blockMask)
							{
								elimMapBlock |= CandMaps[digit];
							}
							elimMapBlock &= blockMap - currentBlockMap;

							foreach (int digit in otherDigitsMask)
							{
								elimMapLine |= CandMaps[digit];
							}
							elimMapLine &= lineMap - currentInterMap;

							checkGeneralizedSdc(
								accumulator, grid, arMode, cannibalMode, d1, d2, urCells,
								line, otherBlock, otherDigitsMask, blockMask, selectedInterMask,
								otherDigitsMask, elimMapLine, elimMapBlock, otherCellsMap, currentBlockMap,
								currentInterMap, i, 0, index
							);
						}
					}
				}
			}
		}


		static void checkGeneralizedSdc(
			ICollection<UniqueRectangleStep> accumulator,
			in Grid grid,
			bool arMode,
			bool cannibalMode,
			int digit1,
			int digit2,
			int[] urCells,
			int line,
			int block,
			short lineMask,
			short blockMask,
			short selectedInterMask,
			short otherDigitsMask,
			in Cells elimMapLine,
			in Cells elimMapBlock,
			in Cells currentLineMap,
			in Cells currentBlockMap,
			in Cells currentInterMap,
			int i,
			int j,
			int index
		)
		{
			short maskOnlyInInter = (short)(selectedInterMask & ~(blockMask | lineMask));
			short maskIsolated = (short)(
				cannibalMode ? (lineMask & blockMask & selectedInterMask) : maskOnlyInInter
			);
			if (
				!cannibalMode && (
					(blockMask & lineMask) != 0
					|| maskIsolated != 0 && !IsPow2(maskIsolated)
				) || cannibalMode && !IsPow2(maskIsolated)
			)
			{
				return;
			}

			var elimMapIsolated = Cells.Empty;
			int digitIsolated = TrailingZeroCount(maskIsolated);
			if (digitIsolated != InvalidFirstSet)
			{
				elimMapIsolated = (cannibalMode ? currentBlockMap | currentLineMap : currentInterMap)
					% CandMaps[digitIsolated]
					& EmptyMap;
			}

			if (currentInterMap.Count + i + j + 1 ==
				PopCount((uint)blockMask) + PopCount((uint)lineMask) + PopCount((uint)maskOnlyInInter)
				&& (!elimMapBlock.IsEmpty || !elimMapLine.IsEmpty || !elimMapIsolated.IsEmpty))
			{
				// Check eliminations.
				var conclusions = new List<Conclusion>(10);
				foreach (int cell in elimMapBlock)
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						if ((blockMask >> digit & 1) != 0)
						{
							conclusions.Add(new(ConclusionType.Elimination, cell, digit));
						}
					}
				}
				foreach (int cell in elimMapLine)
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						if ((lineMask >> digit & 1) != 0)
						{
							conclusions.Add(new(ConclusionType.Elimination, cell, digit));
						}
					}
				}
				foreach (int cell in elimMapIsolated)
				{
					conclusions.Add(new(ConclusionType.Elimination, cell, digitIsolated));
				}
				if (conclusions.Count == 0)
				{
					return;
				}

				// Record highlight candidates and cells.
				var candidateOffsets = new List<(int, ColorIdentifier)>();
				foreach (int cell in urCells)
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							(
								cell * 9 + digit,
								(ColorIdentifier)((otherDigitsMask >> digit & 1) != 0 ? 1 : 0)
							)
						);
					}
				}
				foreach (int cell in currentBlockMap)
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							(
								cell * 9 + digit,
								(ColorIdentifier)(!cannibalMode && digit == digitIsolated ? 3 : 2)
							)
						);
					}
				}
				foreach (int cell in currentInterMap)
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							(
								cell * 9 + digit,
								(ColorIdentifier)(
									digitIsolated == digit ? 3 : (otherDigitsMask >> digit & 1) != 0 ? 1 : 2
								)
							)
						);
					}
				}

				accumulator.Add(
					new UniqueRectangleWithSueDeCoqStep(
						ImmutableArray.CreateRange(conclusions),
						ImmutableArray.Create(new PresentationData
						{
							Cells = arMode ? GetHighlightCells(urCells) : null,
							Candidates = candidateOffsets,
							Regions = new[] { (block, (ColorIdentifier)0), (line, (ColorIdentifier)2) }
						}),
						digit1,
						digit2,
						urCells,
						arMode,
						block,
						line,
						blockMask,
						lineMask,
						selectedInterMask,
						cannibalMode,
						maskIsolated,
						currentBlockMap,
						currentLineMap,
						currentInterMap,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR+Unknown covering.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="comparer">The comparer.</param>
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// <para>The structures:</para>
	/// <para>
	/// Subtype 1:
	/// <code><![CDATA[
	///'      ↓urCellInSameBlock
	///' ab  abc      abc  ←anotherCell
	///'
	///'     abcx-----abcy ←resultCell
	///'           c
	///'      ↑targetCell
	/// ]]></code>
	/// Where the digit <c>a</c> and <c>b</c> in the down-left cell <c>abcx</c> can be removed.
	/// </para>
	/// <para>
	/// Subtype 2:
	/// <code><![CDATA[
	///' abcx   | ab  abc
	///'  |     |
	///'  | c   |
	///'  |     |
	///' abcy   |     abc
	/// ]]></code>
	/// </para>
	/// </remarks>
	private void CheckUnknownCoveringUnique(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		short comparer,
		int d1,
		int d2,
		int index
	)
	{
		checkType1(grid);
#if IMPLEMENTED
		checkType2(grid);
#endif

		void checkType1(in Grid grid)
		{
			var cells = new Cells(urCells);

			// Check all cells are empty.
			bool containsValueCells = false;
			foreach (int cell in cells)
			{
				if (grid.GetStatus(cell) != CellStatus.Empty)
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
			foreach (int targetCell in cells)
			{
				int block = targetCell.ToRegion(RegionLabel.Block);
				var bivalueCellsToCheck = (PeerMaps[targetCell] & RegionMaps[block] & BivalueMap) - cells;
				if (bivalueCellsToCheck.IsEmpty)
				{
					continue;
				}

				// Check all bivalue cells.
				foreach (int bivalueCellToCheck in bivalueCellsToCheck)
				{
					if (new Cells { bivalueCellToCheck, targetCell }.CoveredLine != InvalidFirstSet)
					{
						// 'targetCell' and 'bivalueCellToCheck' can't lie on a same line.
						continue;
					}

					if (grid.GetCandidates(bivalueCellToCheck) != comparer)
					{
						// 'bivalueCell' must contain both 'd1' and 'd2'.
						continue;
					}

					int urCellInSameBlock = ((RegionMaps[block] & cells) - targetCell)[0];
					int coveredLine = new Cells { bivalueCellToCheck, urCellInSameBlock }.CoveredLine;
					if (coveredLine == InvalidFirstSet)
					{
						// The bi-value cell 'bivalueCellToCheck' should be lie on a same region
						// as 'urCellInSameBlock'.
						continue;
					}

					int anotherCell = (cells - urCellInSameBlock & RegionMaps[coveredLine])[0];
					foreach (int extraDigit in grid.GetCandidates(targetCell) & ~comparer)
					{
						short abcMask = (short)(comparer | (short)(1 << extraDigit));

						if (grid.GetCandidates(anotherCell) != abcMask)
						{
							continue;
						}

						// Check the conjugate pair of the extra digit.
						int resultCell = (cells - urCellInSameBlock - anotherCell - targetCell)[0];
						var map = new Cells { targetCell, resultCell };
						int line = map.CoveredLine;
						if (!IsConjugatePair(extraDigit, map, line))
						{
							continue;
						}

						#region Subtype 1
						if (grid.GetCandidates(urCellInSameBlock) != abcMask)
						{
							goto SubType2;
						}

						// Here, is the basic sub-type having passed the checking.
						// Gather conclusions.
						var conclusions = new List<Conclusion>();
						foreach (int digit in grid.GetCandidates(targetCell))
						{
							if (digit == d1 || digit == d2)
							{
								conclusions.Add(new(ConclusionType.Elimination, targetCell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							goto SubType2;
						}

						// Gather views.
						var candidateOffsets = new List<(int, ColorIdentifier)>
						{
							(targetCell * 9 + extraDigit, (ColorIdentifier)1)
						};
						if (grid.Exists(resultCell, d1) is true)
						{
							candidateOffsets.Add((resultCell * 9 + d1, (ColorIdentifier)0));
						}
						if (grid.Exists(resultCell, d2) is true)
						{
							candidateOffsets.Add((resultCell * 9 + d2, (ColorIdentifier)0));
						}
						if (grid.Exists(resultCell, extraDigit) is true)
						{
							candidateOffsets.Add((resultCell * 9 + extraDigit, (ColorIdentifier)1));
						}

						foreach (int digit in grid.GetCandidates(urCellInSameBlock) & abcMask)
						{
							candidateOffsets.Add((urCellInSameBlock * 9 + digit, (ColorIdentifier)0));
						}
						foreach (int digit in grid.GetCandidates(anotherCell))
						{
							candidateOffsets.Add((anotherCell * 9 + digit, (ColorIdentifier)0));
						}
						short _xOr_yMask = grid.GetCandidates(bivalueCellToCheck);
						foreach (int digit in _xOr_yMask)
						{
							candidateOffsets.Add((bivalueCellToCheck * 9 + digit, (ColorIdentifier)2));
						}

						// Add into the list.
						char extraDigitId = (char)(extraDigit + '1');
						short extraDigitMask = (short)(1 << extraDigit);
						accumulator.Add(
							new UniqueRectangleWithUnknownCoveringStep(
								ImmutableArray.CreateRange(conclusions),
								ImmutableArray.Create(new PresentationData[]
								{
									new()
									{
										Cells = new[] { (targetCell, (ColorIdentifier)0) },
										Candidates = candidateOffsets,
										Regions = new[] { (block, (ColorIdentifier)0), (line, (ColorIdentifier)1) }
									},
									new()
									{
										Candidates = new[]
										{
											(resultCell * 9 + extraDigit, (ColorIdentifier)1),
											(targetCell * 9 + extraDigit, (ColorIdentifier)1)
										},
										UnknownValues = new (UnknownValue, ColorIdentifier)[]
										{
											(new(bivalueCellToCheck, 'y', _xOr_yMask), (ColorIdentifier)0),
											(new(targetCell, 'x', _xOr_yMask), (ColorIdentifier)0),
											(
												new(urCellInSameBlock, extraDigitId, extraDigitMask),
												(ColorIdentifier)0
											),
											(new(anotherCell, 'x', _xOr_yMask), (ColorIdentifier)0),
											(
												new(resultCell, extraDigitId, extraDigitMask),
												(ColorIdentifier)0
											)
										}
									}
								}),
								d1,
								d2,
								urCells,
								targetCell,
								extraDigit,
								index
							)
						);
					SubType2:
						#endregion

						#region Subtype 2
						// The extra digit should form a conjugate pair in that line.
						var anotherMap = new Cells { urCellInSameBlock, anotherCell };
						int anotherLine = anotherMap.CoveredLine;
						if (!IsConjugatePair(extraDigit, anotherMap, anotherLine))
						{
							continue;
						}

						// Gather conclusions.
						var conclusionsAnotherSubType = new List<Conclusion>();
						foreach (int digit in grid.GetCandidates(targetCell))
						{
							if (digit == d1 || digit == d2)
							{
								conclusionsAnotherSubType.Add(
									new(ConclusionType.Elimination, targetCell, digit)
								);
							}
						}
						if (conclusionsAnotherSubType.Count == 0)
						{
							continue;
						}

						// Gather views.
						var candidateOffsetsAnotherSubtype = new List<(int, ColorIdentifier)>
						{
							(targetCell * 9 + extraDigit, (ColorIdentifier)1)
						};
						if (grid.Exists(resultCell, d1) is true)
						{
							candidateOffsetsAnotherSubtype.Add((resultCell * 9 + d1, (ColorIdentifier)0));
						}
						if (grid.Exists(resultCell, d2) is true)
						{
							candidateOffsetsAnotherSubtype.Add((resultCell * 9 + d2, (ColorIdentifier)0));
						}
						if (grid.Exists(resultCell, extraDigit) is true)
						{
							candidateOffsetsAnotherSubtype.Add((resultCell * 9 + extraDigit, (ColorIdentifier)1));
						}

						var candidateOffsetsAnotherSubtypeLighter = new List<(int, ColorIdentifier)>
						{
							(resultCell * 9 + extraDigit, (ColorIdentifier)1),
							(targetCell * 9 + extraDigit, (ColorIdentifier)1)
						};
						foreach (int digit in grid.GetCandidates(urCellInSameBlock) & abcMask)
						{
							if (digit == extraDigit)
							{
								candidateOffsetsAnotherSubtype.Add(
									(urCellInSameBlock * 9 + digit, (ColorIdentifier)1)
								);
								candidateOffsetsAnotherSubtypeLighter.Add(
									(urCellInSameBlock * 9 + digit, (ColorIdentifier)1)
								);
							}
							else
							{
								candidateOffsetsAnotherSubtype.Add(
									(urCellInSameBlock * 9 + digit, (ColorIdentifier)0)
								);
							}
						}
						foreach (int digit in grid.GetCandidates(anotherCell))
						{
							if (digit == extraDigit)
							{
								candidateOffsetsAnotherSubtype.Add(
									(anotherCell * 9 + digit, (ColorIdentifier)1)
								);
								candidateOffsetsAnotherSubtypeLighter.Add(
									(anotherCell * 9 + digit, (ColorIdentifier)1)
								);
							}
							else
							{
								candidateOffsetsAnotherSubtype.Add(
									(anotherCell * 9 + digit, (ColorIdentifier)0)
								);
							}
						}
						short _xOr_yMask2 = grid.GetCandidates(bivalueCellToCheck);
						foreach (int digit in _xOr_yMask2)
						{
							candidateOffsetsAnotherSubtype.Add(
								(bivalueCellToCheck * 9 + digit, (ColorIdentifier)2)
							);
						}

						// Add into the list.
						char extraDigitId2 = (char)(extraDigit + '1');
						short extraDigitMask2 = (short)(1 << extraDigit);
						accumulator.Add(
							new UniqueRectangleWithUnknownCoveringStep(
								ImmutableArray.CreateRange(conclusionsAnotherSubType),
								ImmutableArray.Create(new PresentationData[]
								{
									new()
									{
										Cells = new[] { (targetCell, (ColorIdentifier)0) },
										Candidates = candidateOffsetsAnotherSubtype,
										Regions = new[]
										{
											(block, (ColorIdentifier)0),
											(line, (ColorIdentifier)1),
											(anotherLine, (ColorIdentifier)1)
										}
									},
									new()
									{
										Candidates = candidateOffsetsAnotherSubtypeLighter,
										UnknownValues = new (UnknownValue, ColorIdentifier)[]
										{
											(new(bivalueCellToCheck, 'y', _xOr_yMask2), (ColorIdentifier)0),
											(new(targetCell, 'x', _xOr_yMask2), (ColorIdentifier)0),
											(
												new(urCellInSameBlock, extraDigitId2, extraDigitMask2),
												(ColorIdentifier)0
											),
											(new(anotherCell, 'x', _xOr_yMask2), (ColorIdentifier)0),
											(new(resultCell, extraDigitId2, extraDigitMask2), (ColorIdentifier)0)
										}
									}
								}),
								d1,
								d2,
								urCells,
								targetCell,
								extraDigit,
								index
							)
						);
						#endregion
					}
				}
			}
		}

#if IMPLEMENTED
		void checkType2(in Grid grid)
		{
			// TODO: Check type 2.
		}
#endif
	}

	/// <summary>
	/// Check UR+Guardian.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="index">The index.</param>
	private void CheckGuardianUniqueStandard(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		short comparer,
		int d1,
		int d2,
		int index
	)
	{
		var cells = new Cells(urCells);

		// TODO: fix here to support incomplete types.
		if ((grid.GetCandidates(urCells[0]) & comparer) != comparer
			|| (grid.GetCandidates(urCells[1]) & comparer) != comparer
			|| (grid.GetCandidates(urCells[2]) & comparer) != comparer
			|| (grid.GetCandidates(urCells[3]) & comparer) != comparer)
		{
			return;
		}

		// Iterate on two regions used.
		foreach (int[] regionCombination in cells.Regions.GetAllSets().GetSubsets(2))
		{
			var regionCells = RegionMaps[regionCombination[0]] | RegionMaps[regionCombination[1]];
			if ((regionCells & cells) != cells)
			{
				// The regions must contain all 4 UR cells.
				continue;
			}

			var guardian1 = regionCells - cells & CandMaps[d1];
			var guardian2 = regionCells - cells & CandMaps[d2];
			if (!(guardian1.IsEmpty ^ guardian2.IsEmpty))
			{
				// Only one digit can contain guardians.
				continue;
			}

			int guardianDigit = -1;
			Cells? targetElimMap = null, targetGuardianMap = null;
			if (!guardian1.IsEmpty && (guardian1.PeerIntersection & CandMaps[d1]) is { IsEmpty: false } a)
			{
				targetElimMap = a;
				guardianDigit = d1;
				targetGuardianMap = guardian1;
			}
			else if (!guardian2.IsEmpty && (guardian2.PeerIntersection & CandMaps[d2]) is { IsEmpty: false } b)
			{
				targetElimMap = b;
				guardianDigit = d2;
				targetGuardianMap = guardian2;
			}

			if (targetElimMap is not { } elimMap || targetGuardianMap is not { } guardianMap || guardianDigit == -1)
			{
				continue;
			}

			var candidateOffsets = new List<(int, ColorIdentifier)>(16);
			foreach (int cell in urCells)
			{
				candidateOffsets.Add((cell * 9 + d1, (ColorIdentifier)0));
				candidateOffsets.Add((cell * 9 + d2, (ColorIdentifier)0));
			}
			foreach (int cell in guardianMap)
			{
				candidateOffsets.Add((cell * 9 + guardianDigit, (ColorIdentifier)1));
			}

			accumulator.Add(
				new UniqueRectangleWithGuardianStep(
					elimMap.ToImmutableConclusions(guardianDigit),
					ImmutableArray.Create(new PresentationData
					{
						Candidates = candidateOffsets,
						Regions = new[]
						{
							(regionCombination[0], (ColorIdentifier)0),
							(regionCombination[1], (ColorIdentifier)0)
						}
					}),
					d1,
					d2,
					urCells,
					guardianMap,
					guardianDigit,
					false,
					index
				)
			);
		}
	}

	/// <summary>
	/// Check UR+Guardian, with the external subset.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="index">The index.</param>
	private void CheckGuardianUniqueSubset(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		short comparer,
		int d1,
		int d2,
		int index
	)
	{
		// TODO: Implement this.

	}

	/// <summary>
	/// Check AR+Hidden single.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
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
	/// <para>The structure:</para>
	/// <para>
	/// <code><![CDATA[
	/// ' ↓corner1
	/// ' a   | aby  -  -
	/// ' abx | a    -  b
	/// '     | -    -  -
	/// '       ↑corner2(cell 'a')
	/// ]]></code>
	/// There's only one cell can be filled with the digit 'b' besides the cell 'aby'.
	/// </para>
	/// </remarks>
	private void CheckHiddenSingleAvoidable(
		ICollection<UniqueRectangleStep> accumulator,
		in Grid grid,
		int[] urCells,
		int d1,
		int d2,
		int corner1,
		int corner2,
		in Cells otherCellsMap,
		int index
	)
	{
		if (grid.GetStatus(corner1) != CellStatus.Modifiable
			|| grid.GetStatus(corner2) != CellStatus.Modifiable
			|| grid[corner1] != grid[corner2]
			|| grid[corner1] != d1 && grid[corner1] != d2)
		{
			return;
		}

		// Get the base digit ('a') and the other digit ('b').
		// Here 'b' is the digit that we should check the possible hidden single.
		int baseDigit = grid[corner1], otherDigit = baseDigit == d1 ? d2 : d1;
		var cellsThatTwoOtherCellsBothCanSee = otherCellsMap.PeerIntersection & CandMaps[otherDigit];

		// Iterate on two cases (because we holds two other cells,
		// and both those two cells may contain possible elimination).
		for (int i = 0; i < 2; i++)
		{
			var (baseCell, anotherCell) = i == 0
				? (otherCellsMap[0], otherCellsMap[1])
				: (otherCellsMap[1], otherCellsMap[0]);

			// Iterate on each region type.
			for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
			{
				int region = baseCell.ToRegion(label);

				// If the region doesn't overlap with the specified region, just skip it.
				if ((cellsThatTwoOtherCellsBothCanSee & RegionMaps[region]).IsEmpty)
				{
					continue;
				}

				var otherCells = RegionMaps[region] & CandMaps[otherDigit] & PeerMaps[anotherCell];
				int sameRegions = (otherCells + anotherCell).CoveredRegions;
				foreach (int sameRegion in sameRegions)
				{
					// Check whether all possible positions of the digit 'b' in this region only
					// lies in the given cells above ('cellsThatTwoOtherCellsBothCanSee').
					if ((RegionMaps[sameRegion] - anotherCell & CandMaps[otherDigit]) != otherCells)
					{
						continue;
					}

					// Possible hidden single found.
					// If the elimination doesn't exist, just skip it.
					if (grid.Exists(baseCell, otherDigit) is not true)
					{
						continue;
					}

					var cellOffsets = new List<(int, ColorIdentifier)>();
					foreach (int cell in urCells) cellOffsets.Add((cell, (ColorIdentifier)0));

					var candidateOffsets = new List<(int, ColorIdentifier)>
					{
						(anotherCell * 9 + otherDigit, (ColorIdentifier)0)
					};
					foreach (int cell in otherCells)
					{
						candidateOffsets.Add((cell * 9 + otherDigit, (ColorIdentifier)1));
					}

					accumulator.Add(
						new AvoidableRectangleWithHiddenSingleStep(
							ImmutableArray.Create(new Conclusion(ConclusionType.Elimination, baseCell, otherDigit)),
							ImmutableArray.Create(new PresentationData
							{
								Cells = cellOffsets,
								Candidates = candidateOffsets,
								Regions = new[] { (sameRegion, (ColorIdentifier)0) }
							}),
							d1,
							d2,
							urCells,
							baseCell,
							anotherCell,
							sameRegion,
							index
						)
					);
				}
			}
		}
	}
	#endregion


	#region Patterns
	/// <summary>
	/// The table of all <b>Unique Rectangle</b> cells.
	/// </summary>
	private static readonly int[][] Patterns =
	{
		new[] { 0, 3, 9, 12 }, new[] { 0, 4, 9, 13 }, new[] { 0, 5, 9, 14 }, new[] { 0, 6, 9, 15 }, new[] { 0, 7, 9, 16 }, new[] { 0, 8, 9, 17 }, new[] { 1, 3, 10, 12 }, new[] { 1, 4, 10, 13 }, new[] { 1, 5, 10, 14 },
		new[] { 1, 6, 10, 15 }, new[] { 1, 7, 10, 16 }, new[] { 1, 8, 10, 17 }, new[] { 2, 3, 11, 12 }, new[] { 2, 4, 11, 13 }, new[] { 2, 5, 11, 14 }, new[] { 2, 6, 11, 15 }, new[] { 2, 7, 11, 16 }, new[] { 2, 8, 11, 17 },
		new[] { 3, 6, 12, 15 }, new[] { 3, 7, 12, 16 }, new[] { 3, 8, 12, 17 }, new[] { 4, 6, 13, 15 }, new[] { 4, 7, 13, 16 }, new[] { 4, 8, 13, 17 }, new[] { 5, 6, 14, 15 }, new[] { 5, 7, 14, 16 }, new[] { 5, 8, 14, 17 },
		new[] { 0, 3, 18, 21 }, new[] { 0, 4, 18, 22 }, new[] { 0, 5, 18, 23 }, new[] { 0, 6, 18, 24 }, new[] { 0, 7, 18, 25 }, new[] { 0, 8, 18, 26 }, new[] { 1, 3, 19, 21 }, new[] { 1, 4, 19, 22 }, new[] { 1, 5, 19, 23 },
		new[] { 1, 6, 19, 24 }, new[] { 1, 7, 19, 25 }, new[] { 1, 8, 19, 26 }, new[] { 2, 3, 20, 21 }, new[] { 2, 4, 20, 22 }, new[] { 2, 5, 20, 23 }, new[] { 2, 6, 20, 24 }, new[] { 2, 7, 20, 25 }, new[] { 2, 8, 20, 26 },
		new[] { 3, 6, 21, 24 }, new[] { 3, 7, 21, 25 }, new[] { 3, 8, 21, 26 }, new[] { 4, 6, 22, 24 }, new[] { 4, 7, 22, 25 }, new[] { 4, 8, 22, 26 }, new[] { 5, 6, 23, 24 }, new[] { 5, 7, 23, 25 }, new[] { 5, 8, 23, 26 },
		new[] { 9, 12, 18, 21 }, new[] { 9, 13, 18, 22 }, new[] { 9, 14, 18, 23 }, new[] { 9, 15, 18, 24 }, new[] { 9, 16, 18, 25 }, new[] { 9, 17, 18, 26 }, new[] { 10, 12, 19, 21 }, new[] { 10, 13, 19, 22 }, new[] { 10, 14, 19, 23 },
		new[] { 10, 15, 19, 24 }, new[] { 10, 16, 19, 25 }, new[] { 10, 17, 19, 26 }, new[] { 11, 12, 20, 21 }, new[] { 11, 13, 20, 22 }, new[] { 11, 14, 20, 23 }, new[] { 11, 15, 20, 24 }, new[] { 11, 16, 20, 25 }, new[] { 11, 17, 20, 26 },
		new[] { 12, 15, 21, 24 }, new[] { 12, 16, 21, 25 }, new[] { 12, 17, 21, 26 }, new[] { 13, 15, 22, 24 }, new[] { 13, 16, 22, 25 }, new[] { 13, 17, 22, 26 }, new[] { 14, 15, 23, 24 }, new[] { 14, 16, 23, 25 }, new[] { 14, 17, 23, 26 },
		new[] { 27, 30, 36, 39 }, new[] { 27, 31, 36, 40 }, new[] { 27, 32, 36, 41 }, new[] { 27, 33, 36, 42 }, new[] { 27, 34, 36, 43 }, new[] { 27, 35, 36, 44 }, new[] { 28, 30, 37, 39 }, new[] { 28, 31, 37, 40 }, new[] { 28, 32, 37, 41 },
		new[] { 28, 33, 37, 42 }, new[] { 28, 34, 37, 43 }, new[] { 28, 35, 37, 44 }, new[] { 29, 30, 38, 39 }, new[] { 29, 31, 38, 40 }, new[] { 29, 32, 38, 41 }, new[] { 29, 33, 38, 42 }, new[] { 29, 34, 38, 43 }, new[] { 29, 35, 38, 44 },
		new[] { 30, 33, 39, 42 }, new[] { 30, 34, 39, 43 }, new[] { 30, 35, 39, 44 }, new[] { 31, 33, 40, 42 }, new[] { 31, 34, 40, 43 }, new[] { 31, 35, 40, 44 }, new[] { 32, 33, 41, 42 }, new[] { 32, 34, 41, 43 }, new[] { 32, 35, 41, 44 },
		new[] { 27, 30, 45, 48 }, new[] { 27, 31, 45, 49 }, new[] { 27, 32, 45, 50 }, new[] { 27, 33, 45, 51 }, new[] { 27, 34, 45, 52 }, new[] { 27, 35, 45, 53 }, new[] { 28, 30, 46, 48 }, new[] { 28, 31, 46, 49 }, new[] { 28, 32, 46, 50 },
		new[] { 28, 33, 46, 51 }, new[] { 28, 34, 46, 52 }, new[] { 28, 35, 46, 53 }, new[] { 29, 30, 47, 48 }, new[] { 29, 31, 47, 49 }, new[] { 29, 32, 47, 50 }, new[] { 29, 33, 47, 51 }, new[] { 29, 34, 47, 52 }, new[] { 29, 35, 47, 53 },
		new[] { 30, 33, 48, 51 }, new[] { 30, 34, 48, 52 }, new[] { 30, 35, 48, 53 }, new[] { 31, 33, 49, 51 }, new[] { 31, 34, 49, 52 }, new[] { 31, 35, 49, 53 }, new[] { 32, 33, 50, 51 }, new[] { 32, 34, 50, 52 }, new[] { 32, 35, 50, 53 },
		new[] { 36, 39, 45, 48 }, new[] { 36, 40, 45, 49 }, new[] { 36, 41, 45, 50 }, new[] { 36, 42, 45, 51 }, new[] { 36, 43, 45, 52 }, new[] { 36, 44, 45, 53 }, new[] { 37, 39, 46, 48 }, new[] { 37, 40, 46, 49 }, new[] { 37, 41, 46, 50 },
		new[] { 37, 42, 46, 51 }, new[] { 37, 43, 46, 52 }, new[] { 37, 44, 46, 53 }, new[] { 38, 39, 47, 48 }, new[] { 38, 40, 47, 49 }, new[] { 38, 41, 47, 50 }, new[] { 38, 42, 47, 51 }, new[] { 38, 43, 47, 52 }, new[] { 38, 44, 47, 53 },
		new[] { 39, 42, 48, 51 }, new[] { 39, 43, 48, 52 }, new[] { 39, 44, 48, 53 }, new[] { 40, 42, 49, 51 }, new[] { 40, 43, 49, 52 }, new[] { 40, 44, 49, 53 }, new[] { 41, 42, 50, 51 }, new[] { 41, 43, 50, 52 }, new[] { 41, 44, 50, 53 },
		new[] { 54, 57, 63, 66 }, new[] { 54, 58, 63, 67 }, new[] { 54, 59, 63, 68 }, new[] { 54, 60, 63, 69 }, new[] { 54, 61, 63, 70 }, new[] { 54, 62, 63, 71 }, new[] { 55, 57, 64, 66 }, new[] { 55, 58, 64, 67 }, new[] { 55, 59, 64, 68 },
		new[] { 55, 60, 64, 69 }, new[] { 55, 61, 64, 70 }, new[] { 55, 62, 64, 71 }, new[] { 56, 57, 65, 66 }, new[] { 56, 58, 65, 67 }, new[] { 56, 59, 65, 68 }, new[] { 56, 60, 65, 69 }, new[] { 56, 61, 65, 70 }, new[] { 56, 62, 65, 71 },
		new[] { 57, 60, 66, 69 }, new[] { 57, 61, 66, 70 }, new[] { 57, 62, 66, 71 }, new[] { 58, 60, 67, 69 }, new[] { 58, 61, 67, 70 }, new[] { 58, 62, 67, 71 }, new[] { 59, 60, 68, 69 }, new[] { 59, 61, 68, 70 }, new[] { 59, 62, 68, 71 },
		new[] { 54, 57, 72, 75 }, new[] { 54, 58, 72, 76 }, new[] { 54, 59, 72, 77 }, new[] { 54, 60, 72, 78 }, new[] { 54, 61, 72, 79 }, new[] { 54, 62, 72, 80 }, new[] { 55, 57, 73, 75 }, new[] { 55, 58, 73, 76 }, new[] { 55, 59, 73, 77 },
		new[] { 55, 60, 73, 78 }, new[] { 55, 61, 73, 79 }, new[] { 55, 62, 73, 80 }, new[] { 56, 57, 74, 75 }, new[] { 56, 58, 74, 76 }, new[] { 56, 59, 74, 77 }, new[] { 56, 60, 74, 78 }, new[] { 56, 61, 74, 79 }, new[] { 56, 62, 74, 80 },
		new[] { 57, 60, 75, 78 }, new[] { 57, 61, 75, 79 }, new[] { 57, 62, 75, 80 }, new[] { 58, 60, 76, 78 }, new[] { 58, 61, 76, 79 }, new[] { 58, 62, 76, 80 }, new[] { 59, 60, 77, 78 }, new[] { 59, 61, 77, 79 }, new[] { 59, 62, 77, 80 },
		new[] { 63, 66, 72, 75 }, new[] { 63, 67, 72, 76 }, new[] { 63, 68, 72, 77 }, new[] { 63, 69, 72, 78 }, new[] { 63, 70, 72, 79 }, new[] { 63, 71, 72, 80 }, new[] { 64, 66, 73, 75 }, new[] { 64, 67, 73, 76 }, new[] { 64, 68, 73, 77 },
		new[] { 64, 69, 73, 78 }, new[] { 64, 70, 73, 79 }, new[] { 64, 71, 73, 80 }, new[] { 65, 66, 74, 75 }, new[] { 65, 67, 74, 76 }, new[] { 65, 68, 74, 77 }, new[] { 65, 69, 74, 78 }, new[] { 65, 70, 74, 79 }, new[] { 65, 71, 74, 80 },
		new[] { 66, 69, 75, 78 }, new[] { 66, 70, 75, 79 }, new[] { 66, 71, 75, 80 }, new[] { 67, 69, 76, 78 }, new[] { 67, 70, 76, 79 }, new[] { 67, 71, 76, 80 }, new[] { 68, 69, 77, 78 }, new[] { 68, 70, 77, 79 }, new[] { 68, 71, 77, 80 },
		new[] { 0, 1, 27, 28 }, new[] { 0, 1, 36, 37 }, new[] { 0, 1, 45, 46 }, new[] { 0, 1, 54, 55 }, new[] { 0, 1, 63, 64 }, new[] { 0, 1, 72, 73 }, new[] { 9, 10, 27, 28 }, new[] { 9, 10, 36, 37 }, new[] { 9, 10, 45, 46 },
		new[] { 9, 10, 54, 55 }, new[] { 9, 10, 63, 64 }, new[] { 9, 10, 72, 73 }, new[] { 18, 19, 27, 28 }, new[] { 18, 19, 36, 37 }, new[] { 18, 19, 45, 46 }, new[] { 18, 19, 54, 55 }, new[] { 18, 19, 63, 64 }, new[] { 18, 19, 72, 73 },
		new[] { 27, 28, 54, 55 }, new[] { 27, 28, 63, 64 }, new[] { 27, 28, 72, 73 }, new[] { 36, 37, 54, 55 }, new[] { 36, 37, 63, 64 }, new[] { 36, 37, 72, 73 }, new[] { 45, 46, 54, 55 }, new[] { 45, 46, 63, 64 }, new[] { 45, 46, 72, 73 },
		new[] { 0, 2, 27, 29 }, new[] { 0, 2, 36, 38 }, new[] { 0, 2, 45, 47 }, new[] { 0, 2, 54, 56 }, new[] { 0, 2, 63, 65 }, new[] { 0, 2, 72, 74 }, new[] { 9, 11, 27, 29 }, new[] { 9, 11, 36, 38 }, new[] { 9, 11, 45, 47 },
		new[] { 9, 11, 54, 56 }, new[] { 9, 11, 63, 65 }, new[] { 9, 11, 72, 74 }, new[] { 18, 20, 27, 29 }, new[] { 18, 20, 36, 38 }, new[] { 18, 20, 45, 47 }, new[] { 18, 20, 54, 56 }, new[] { 18, 20, 63, 65 }, new[] { 18, 20, 72, 74 },
		new[] { 27, 29, 54, 56 }, new[] { 27, 29, 63, 65 }, new[] { 27, 29, 72, 74 }, new[] { 36, 38, 54, 56 }, new[] { 36, 38, 63, 65 }, new[] { 36, 38, 72, 74 }, new[] { 45, 47, 54, 56 }, new[] { 45, 47, 63, 65 }, new[] { 45, 47, 72, 74 },
		new[] { 1, 2, 28, 29 }, new[] { 1, 2, 37, 38 }, new[] { 1, 2, 46, 47 }, new[] { 1, 2, 55, 56 }, new[] { 1, 2, 64, 65 }, new[] { 1, 2, 73, 74 }, new[] { 10, 11, 28, 29 }, new[] { 10, 11, 37, 38 }, new[] { 10, 11, 46, 47 },
		new[] { 10, 11, 55, 56 }, new[] { 10, 11, 64, 65 }, new[] { 10, 11, 73, 74 }, new[] { 19, 20, 28, 29 }, new[] { 19, 20, 37, 38 }, new[] { 19, 20, 46, 47 }, new[] { 19, 20, 55, 56 }, new[] { 19, 20, 64, 65 }, new[] { 19, 20, 73, 74 },
		new[] { 28, 29, 55, 56 }, new[] { 28, 29, 64, 65 }, new[] { 28, 29, 73, 74 }, new[] { 37, 38, 55, 56 }, new[] { 37, 38, 64, 65 }, new[] { 37, 38, 73, 74 }, new[] { 46, 47, 55, 56 }, new[] { 46, 47, 64, 65 }, new[] { 46, 47, 73, 74 },
		new[] { 3, 4, 30, 31 }, new[] { 3, 4, 39, 40 }, new[] { 3, 4, 48, 49 }, new[] { 3, 4, 57, 58 }, new[] { 3, 4, 66, 67 }, new[] { 3, 4, 75, 76 }, new[] { 12, 13, 30, 31 }, new[] { 12, 13, 39, 40 }, new[] { 12, 13, 48, 49 },
		new[] { 12, 13, 57, 58 }, new[] { 12, 13, 66, 67 }, new[] { 12, 13, 75, 76 }, new[] { 21, 22, 30, 31 }, new[] { 21, 22, 39, 40 }, new[] { 21, 22, 48, 49 }, new[] { 21, 22, 57, 58 }, new[] { 21, 22, 66, 67 }, new[] { 21, 22, 75, 76 },
		new[] { 30, 31, 57, 58 }, new[] { 30, 31, 66, 67 }, new[] { 30, 31, 75, 76 }, new[] { 39, 40, 57, 58 }, new[] { 39, 40, 66, 67 }, new[] { 39, 40, 75, 76 }, new[] { 48, 49, 57, 58 }, new[] { 48, 49, 66, 67 }, new[] { 48, 49, 75, 76 },
		new[] { 3, 5, 30, 32 }, new[] { 3, 5, 39, 41 }, new[] { 3, 5, 48, 50 }, new[] { 3, 5, 57, 59 }, new[] { 3, 5, 66, 68 }, new[] { 3, 5, 75, 77 }, new[] { 12, 14, 30, 32 }, new[] { 12, 14, 39, 41 }, new[] { 12, 14, 48, 50 },
		new[] { 12, 14, 57, 59 }, new[] { 12, 14, 66, 68 }, new[] { 12, 14, 75, 77 }, new[] { 21, 23, 30, 32 }, new[] { 21, 23, 39, 41 }, new[] { 21, 23, 48, 50 }, new[] { 21, 23, 57, 59 }, new[] { 21, 23, 66, 68 }, new[] { 21, 23, 75, 77 },
		new[] { 30, 32, 57, 59 }, new[] { 30, 32, 66, 68 }, new[] { 30, 32, 75, 77 }, new[] { 39, 41, 57, 59 }, new[] { 39, 41, 66, 68 }, new[] { 39, 41, 75, 77 }, new[] { 48, 50, 57, 59 }, new[] { 48, 50, 66, 68 }, new[] { 48, 50, 75, 77 },
		new[] { 4, 5, 31, 32 }, new[] { 4, 5, 40, 41 }, new[] { 4, 5, 49, 50 }, new[] { 4, 5, 58, 59 }, new[] { 4, 5, 67, 68 }, new[] { 4, 5, 76, 77 }, new[] { 13, 14, 31, 32 }, new[] { 13, 14, 40, 41 }, new[] { 13, 14, 49, 50 },
		new[] { 13, 14, 58, 59 }, new[] { 13, 14, 67, 68 }, new[] { 13, 14, 76, 77 }, new[] { 22, 23, 31, 32 }, new[] { 22, 23, 40, 41 }, new[] { 22, 23, 49, 50 }, new[] { 22, 23, 58, 59 }, new[] { 22, 23, 67, 68 }, new[] { 22, 23, 76, 77 },
		new[] { 31, 32, 58, 59 }, new[] { 31, 32, 67, 68 }, new[] { 31, 32, 76, 77 }, new[] { 40, 41, 58, 59 }, new[] { 40, 41, 67, 68 }, new[] { 40, 41, 76, 77 }, new[] { 49, 50, 58, 59 }, new[] { 49, 50, 67, 68 }, new[] { 49, 50, 76, 77 },
		new[] { 6, 7, 33, 34 }, new[] { 6, 7, 42, 43 }, new[] { 6, 7, 51, 52 }, new[] { 6, 7, 60, 61 }, new[] { 6, 7, 69, 70 }, new[] { 6, 7, 78, 79 }, new[] { 15, 16, 33, 34 }, new[] { 15, 16, 42, 43 }, new[] { 15, 16, 51, 52 },
		new[] { 15, 16, 60, 61 }, new[] { 15, 16, 69, 70 }, new[] { 15, 16, 78, 79 }, new[] { 24, 25, 33, 34 }, new[] { 24, 25, 42, 43 }, new[] { 24, 25, 51, 52 }, new[] { 24, 25, 60, 61 }, new[] { 24, 25, 69, 70 }, new[] { 24, 25, 78, 79 },
		new[] { 33, 34, 60, 61 }, new[] { 33, 34, 69, 70 }, new[] { 33, 34, 78, 79 }, new[] { 42, 43, 60, 61 }, new[] { 42, 43, 69, 70 }, new[] { 42, 43, 78, 79 }, new[] { 51, 52, 60, 61 }, new[] { 51, 52, 69, 70 }, new[] { 51, 52, 78, 79 },
		new[] { 6, 8, 33, 35 }, new[] { 6, 8, 42, 44 }, new[] { 6, 8, 51, 53 }, new[] { 6, 8, 60, 62 }, new[] { 6, 8, 69, 71 }, new[] { 6, 8, 78, 80 }, new[] { 15, 17, 33, 35 }, new[] { 15, 17, 42, 44 }, new[] { 15, 17, 51, 53 },
		new[] { 15, 17, 60, 62 }, new[] { 15, 17, 69, 71 }, new[] { 15, 17, 78, 80 }, new[] { 24, 26, 33, 35 }, new[] { 24, 26, 42, 44 }, new[] { 24, 26, 51, 53 }, new[] { 24, 26, 60, 62 }, new[] { 24, 26, 69, 71 }, new[] { 24, 26, 78, 80 },
		new[] { 33, 35, 60, 62 }, new[] { 33, 35, 69, 71 }, new[] { 33, 35, 78, 80 }, new[] { 42, 44, 60, 62 }, new[] { 42, 44, 69, 71 }, new[] { 42, 44, 78, 80 }, new[] { 51, 53, 60, 62 }, new[] { 51, 53, 69, 71 }, new[] { 51, 53, 78, 80 },
		new[] { 7, 8, 34, 35 }, new[] { 7, 8, 43, 44 }, new[] { 7, 8, 52, 53 }, new[] { 7, 8, 61, 62 }, new[] { 7, 8, 70, 71 }, new[] { 7, 8, 79, 80 }, new[] { 16, 17, 34, 35 }, new[] { 16, 17, 43, 44 }, new[] { 16, 17, 52, 53 },
		new[] { 16, 17, 61, 62 }, new[] { 16, 17, 70, 71 }, new[] { 16, 17, 79, 80 }, new[] { 25, 26, 34, 35 }, new[] { 25, 26, 43, 44 }, new[] { 25, 26, 52, 53 }, new[] { 25, 26, 61, 62 }, new[] { 25, 26, 70, 71 }, new[] { 25, 26, 79, 80 },
		new[] { 34, 35, 61, 62 }, new[] { 34, 35, 70, 71 }, new[] { 34, 35, 79, 80 }, new[] { 43, 44, 61, 62 }, new[] { 43, 44, 70, 71 }, new[] { 43, 44, 79, 80 }, new[] { 52, 53, 61, 62 }, new[] { 52, 53, 70, 71 }, new[] { 52, 53, 79, 80 }
	};
	#endregion
}
