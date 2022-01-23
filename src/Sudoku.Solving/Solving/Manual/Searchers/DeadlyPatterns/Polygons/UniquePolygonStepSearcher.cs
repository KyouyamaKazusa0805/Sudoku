namespace Sudoku.Solving.Manual.Searchers.DeadlyPatterns.Polygons;

/// <summary>
/// Provides with a <b>Unique Polygon</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Unique Polygon Type 1</item>
/// <item>Unique Polygon Type 2</item>
/// <item>Unique Polygon Type 3</item>
/// <item>Unique Polygon Type 4</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe class UniquePolygonStepSearcher : IUniquePolygonStepSearcher
{
	/// <summary>
	/// Indicates all possible patterns to iterate.
	/// </summary>
	/// <remarks>
	/// Please note that all possible heptagons and octagons are in here.
	/// </remarks>
	private static readonly UniquePolygonPattern[] Patterns = new UniquePolygonPattern[BdpTemplatesSize3Count];


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static UniquePolygonStepSearcher()
	{
		int[][] quads =
		{
			new[] { 0, 1, 3, 4 }, new[] { 1, 2, 4, 5 }, new[] { 3, 4, 6, 7 }, new[] { 4, 5, 7, 8 },
			new[] { 0, 2, 3, 5 }, new[] { 3, 5, 6, 8 }, new[] { 0, 1, 6, 7 }, new[] { 1, 2, 7, 8 },
			new[] { 0, 2, 6, 8 }
		};

		int count = 0;
		for (int block = 0; block < 9; block++)
		{
			for (int i = 0; i < 9; i++) // 9 cases.
			{
				int[] quad = quads[i];
				int[] tempQuad = new int[4];
				for (int j = 0; j < 4; j++)
				{
					// Set all indices to cell offsets.
					tempQuad[j] = (block / 3 * 3 + quad[j] / 3) * 9 + block % 3 * 3 + quad[j] % 3;
				}

				gatherHeptagons(block, i, tempQuad, ref count);
				gatherOctagons(block, i, tempQuad, ref count);
			}
		}


		static void gatherHeptagons(int block, int i, int[] quad, ref int count)
		{
			int[][] triplets = new int[4][]
			{
				new[] { quad[0], quad[1], quad[2] }, // (0, 1) and (0, 2) is same region.
				new[] { quad[1], quad[0], quad[3] }, // (0, 1) and (1, 3) is same region.
				new[] { quad[2], quad[0], quad[3] }, // (0, 2) and (2, 3) is same region.
				new[] { quad[3], quad[1], quad[2] }, // (1, 3) and (2, 3) is same region.
			};

			for (int j = 0; j < 4; j++)
			{
				int[] triplet = triplets[j];
				int region1 = new Cells { triplet[0], triplet[1] }.CoveredLine;
				int region2 = new Cells { triplet[0], triplet[2] }.CoveredLine;
				int[,] pair1 = new int[6, 2], pair2 = new int[6, 2];
				var (incre1, incre2) = i switch
				{
					0 or 1 or 2 or 3 => (9, 1),
					4 or 5 => (9, 2),
					6 or 7 => (18, 1),
					8 => (18, 2)
				};
				if (region1 is >= 9 and < 18)
				{
					// 'region1' is a row and 'region2' is a column.
					r(block, region1, pair1, incre1, j);
					r(block, region2, pair2, incre2, j);
				}
				else
				{
					// 'region1' is a column and 'region2' is a row.
					r(block, region1, pair1, incre2, j);
					r(block, region2, pair2, incre1, j);
				}

				for (int i1 = 0; i1 < 6; i1++)
				{
					for (int i2 = 0; i2 < 6; i2++)
					{
						// Now check extra digits.
						var allCells = new List<int>(triplet)
						{
							pair1[i1, 0], pair1[i1, 1], pair2[i2, 0], pair2[i2, 1]
						};

						long v = 0;
						for (int z = 0, iterationCount = allCells.Count; z < iterationCount; z++)
						{
							v |= (long)allCells[z];

							if (z != allCells.Count - 1)
							{
								v <<= 7;
							}

							if (z == 2)
							{
								v |= 127;
								v <<= 7;
							}
						}

						Patterns[count++] = new(v);
					}
				}
			}
		}

		static void gatherOctagons(int block, int i, int[] quad, ref int count)
		{
			int region1 = new Cells { quad[0], quad[1] }.CoveredLine;
			int region2 = new Cells { quad[0], quad[2] }.CoveredLine;
			int[,] pair1 = new int[6, 2], pair2 = new int[6, 2];
			var (incre1, incre2) = i switch
			{
				0 or 1 or 2 or 3 => (9, 1),
				4 or 5 => (9, 2),
				6 or 7 => (18, 1),
				8 => (18, 2),
			};
			if (region1 is >= 9 and < 18)
			{
				// 'region1' is a row and 'region2' is a column.
				r(block, region1, pair1, incre1, 0);
				r(block, region2, pair2, incre2, 0);
			}
			else
			{
				// 'region1' is a column and 'region2' is a row.
				r(block, region1, pair1, incre2, 0);
				r(block, region2, pair2, incre1, 0);
			}

			for (int i1 = 0; i1 < 6; i1++)
			{
				for (int i2 = 0; i2 < 6; i2++)
				{
					// Now check extra digits.
					var allCells = new List<int>(quad)
					{
						pair1[i1, 0],
						pair1[i1, 1],
						pair2[i2, 0],
						pair2[i2, 1]
					};

					long v = 0;
					for (int z = 0, iterationCount = allCells.Count; z < iterationCount; z++)
					{
						int cell = allCells[z];
						v |= (long)cell;
						if (z != allCells.Count - 1)
						{
							v <<= 7;
						}
					}

					Patterns[count++] = new(v);
				}
			}
		}

		static void r(int block, int region, int[,] pair, int increment, int index)
		{
			for (int i = 0, cur = 0; i < 9; i++)
			{
				int cell = RegionCells[region][i];
				if (block == cell.ToRegionIndex(Region.Block))
				{
					continue;
				}

				(pair[cur, 0], pair[cur, 1]) = index switch
				{
					0 => (cell, cell + increment),
					1 => region is >= 18 and < 27 ? (cell - increment, cell) : (cell, cell + increment),
					2 => region is >= 9 and < 18 ? (cell - increment, cell) : (cell, cell + increment),
					3 => (cell - increment, cell)
				};
				cur++;
			}
		}
	}


	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(17, DisplayingLevel.B);


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		if (EmptyMap.Count < 7)
		{
			return null;
		}

		for (int i = 0, end = EmptyMap.Count == 7 ? BdpTemplatesSize3Count : BdpTemplatesSize4Count; i < end; i++)
		{
			var pattern = Patterns[i];
			if ((EmptyMap & pattern.Map) != pattern.Map)
			{
				// The pattern contains non-empty cells.
				continue;
			}

			var map = pattern.Map;
			var ((p11, p12), (p21, p22), (c1, c2, c3, c4)) = pattern;
			short cornerMask1 = (short)(grid.GetCandidates(p11) | grid.GetCandidates(p12));
			short cornerMask2 = (short)(grid.GetCandidates(p21) | grid.GetCandidates(p22));
			short centerMask = (short)((short)(grid.GetCandidates(c1) | grid.GetCandidates(c2)) | grid.GetCandidates(c3));
			if (map.Count == 8) centerMask |= grid.GetCandidates(c4);

			if (CheckType1(accumulator, grid, pattern, onlyFindOne, cornerMask1, cornerMask2, centerMask, map) is { } type1Step)
			{
				return type1Step;
			}
			if (CheckType2(accumulator, grid, pattern, onlyFindOne, cornerMask1, cornerMask2, centerMask, map) is { } type2Step)
			{
				return type2Step;
			}
			if (CheckType3(accumulator, grid, pattern, onlyFindOne, cornerMask1, cornerMask2, centerMask, map) is { } type3Step)
			{
				return type3Step;
			}
			if (CheckType4(accumulator, grid, pattern, onlyFindOne, cornerMask1, cornerMask2, centerMask, map) is { } type4Step)
			{
				return type4Step;
			}
		}

		return null;
	}


	private static Step? CheckType1(
		ICollection<Step> accumulator,
		in Grid grid,
		UniquePolygonPattern pattern,
		bool findOnlyOne,
		short cornerMask1,
		short cornerMask2,
		short centerMask,
		Cells map
	)
	{
		short orMask = (short)((short)(cornerMask1 | cornerMask2) | centerMask);
		if (PopCount((uint)orMask) != (pattern.IsHeptagon ? 4 : 5))
		{
			goto ReturnNull;
		}

		// Iterate on each combination.
		foreach (int[] digits in orMask.GetAllSets().GetSubsets(pattern.IsHeptagon ? 3 : 4))
		{
			short tempMask = 0;
			foreach (int digit in digits)
			{
				tempMask |= (short)(1 << digit);
			}

			int otherDigit = TrailingZeroCount(orMask & ~tempMask);
			var mapContainingThatDigit = map & CandMaps[otherDigit];
			if (mapContainingThatDigit is not [var elimCell])
			{
				continue;
			}

			short elimMask = (short)(grid.GetCandidates(elimCell) & tempMask);
			if (elimMask == 0)
			{
				continue;
			}

			var conclusions = new List<Conclusion>(4);
			foreach (int digit in elimMask)
			{
				conclusions.Add(new(ConclusionType.Elimination, elimCell, digit));
			}

			var candidateOffsets = new List<(int, ColorIdentifier)>();
			foreach (int cell in map)
			{
				if (mapContainingThatDigit.Contains(cell))
				{
					continue;
				}

				foreach (int digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)0));
				}
			}

			var step = new UniquePolygonType1Step(
				conclusions.ToImmutableArray(),
				ImmutableArray.Create(new PresentationData { Candidates = candidateOffsets }),
				map,
				tempMask
			);
			if (findOnlyOne)
			{
				return step;
			}

			accumulator.Add(step);
		}

	ReturnNull:
		return null;
	}

	private static Step? CheckType2(
		ICollection<Step> accumulator,
		in Grid grid,
		UniquePolygonPattern pattern,
		bool findOnlyOne,
		short cornerMask1,
		short cornerMask2,
		short centerMask,
		Cells map
	)
	{
		short orMask = (short)((short)(cornerMask1 | cornerMask2) | centerMask);
		if (PopCount((uint)orMask) != (pattern.IsHeptagon ? 4 : 5))
		{
			goto ReturnNull;
		}

		// Iterate on each combination.
		foreach (int[] digits in orMask.GetAllSets().ToArray().GetSubsets(pattern.IsHeptagon ? 3 : 4))
		{
			short tempMask = 0;
			foreach (int digit in digits)
			{
				tempMask |= (short)(1 << digit);
			}

			int otherDigit = TrailingZeroCount(orMask & ~tempMask);
			var mapContainingThatDigit = map & CandMaps[otherDigit];
			var elimMap = (mapContainingThatDigit.PeerIntersection - map) & CandMaps[otherDigit];
			if (elimMap.IsEmpty)
			{
				continue;
			}

			var conclusions = new List<Conclusion>();
			foreach (int cell in elimMap)
			{
				conclusions.Add(new(ConclusionType.Elimination, cell, otherDigit));
			}

			var candidateOffsets = new List<(int, ColorIdentifier)>();
			foreach (int cell in map)
			{
				foreach (int digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)(digit == otherDigit ? 1 : 0)));
				}
			}

			var step = new UniquePolygonType2Step(
				conclusions.ToImmutableArray(),
				ImmutableArray.Create(new PresentationData { Candidates = candidateOffsets }),
				map,
				tempMask,
				otherDigit
			);
			if (findOnlyOne)
			{
				return step;
			}

			accumulator.Add(step);
		}

	ReturnNull:
		return null;
	}

	private static Step? CheckType3(
		ICollection<Step> accumulator,
		in Grid grid,
		UniquePolygonPattern pattern,
		bool findOnlyOne,
		short cornerMask1,
		short cornerMask2,
		short centerMask,
		Cells map
	)
	{
		short orMask = (short)((short)(cornerMask1 | cornerMask2) | centerMask);
		foreach (int region in map.Regions)
		{
			Cells currentMap = RegionMaps[region] & map, otherCellsMap = map - currentMap;
			short otherMask = 0;
			foreach (int cell in otherCellsMap)
			{
				otherMask |= grid.GetCandidates(cell);
			}

			foreach (int[] digits in orMask.GetAllSets().GetSubsets(pattern.IsHeptagon ? 3 : 4))
			{
				short tempMask = 0;
				foreach (int digit in digits)
				{
					tempMask |= (short)(1 << digit);
				}
				if (otherMask != tempMask)
				{
					continue;
				}

				// Iterate on the cells by the specified size.
				var iterationCellsMap = (RegionMaps[region] - currentMap) & EmptyMap;
				int[] iterationCells = iterationCellsMap.ToArray();
				short otherDigitsMask = (short)(orMask & ~tempMask);
				for (
					int size = PopCount((uint)otherDigitsMask) - 1, count = iterationCellsMap.Count;
					size < count;
					size++
				)
				{
					foreach (int[] combination in iterationCells.GetSubsets(size))
					{
						short comparer = 0;
						foreach (int cell in combination)
						{
							comparer |= grid.GetCandidates(cell);
						}
						if ((tempMask & comparer) != 0 || PopCount((uint)tempMask) - 1 != size
							|| (tempMask & otherDigitsMask) != otherDigitsMask)
						{
							continue;
						}

						// Type 3 found.
						// Now check eliminations.
						var conclusions = new List<Conclusion>();
						foreach (int digit in comparer)
						{
							var cells = iterationCellsMap & CandMaps[digit];
							if (cells.IsEmpty)
							{
								continue;
							}

							foreach (int cell in cells)
							{
								conclusions.Add(new(ConclusionType.Elimination, cell, digit));
							}
						}

						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<(int, ColorIdentifier)>();
						foreach (int cell in currentMap)
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
						foreach (int cell in otherCellsMap)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)0));
							}
						}
						foreach (int cell in combination)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)1));
							}
						}

						var step = new UniquePolygonType3Step(
							conclusions.ToImmutableArray(),
							ImmutableArray.Create(new PresentationData
							{
								Candidates = candidateOffsets,
								Regions = new[] { (region, (ColorIdentifier)0) }
							}),
							map,
							tempMask,
							combination,
							otherDigitsMask
						);
						if (findOnlyOne)
						{
							return step;
						}

						accumulator.Add(step);
					}
				}
			}
		}

		return null;
	}

	private static Step? CheckType4(
		ICollection<Step> accumulator,
		in Grid grid,
		UniquePolygonPattern pattern,
		bool findOnlyOne,
		short cornerMask1,
		short cornerMask2,
		short centerMask,
		Cells map
	)
	{
		// The type 4 may be complex and terrible to process.
		// All regions that the pattern lies in should be checked.
		short orMask = (short)((short)(cornerMask1 | cornerMask2) | centerMask);
		foreach (int region in map.Regions)
		{
			Cells currentMap = RegionMaps[region] & map, otherCellsMap = map - currentMap;
			short otherMask = 0;
			foreach (int cell in otherCellsMap)
			{
				otherMask |= grid.GetCandidates(cell);
			}

			// Iterate on each possible digit combination.
			// For example, if values are { 1, 2, 3 }, then all combinations taken 2 values
			// are { 1, 2 }, { 2, 3 } and { 1, 3 }.
			foreach (int[] digits in orMask.GetAllSets().GetSubsets(pattern.IsHeptagon ? 3 : 4))
			{
				short tempMask = 0;
				foreach (int digit in digits)
				{
					tempMask |= (short)(1 << digit);
				}
				if (otherMask != tempMask)
				{
					continue;
				}

				// Iterate on each combination.
				// Only one digit should be eliminated, and other digits should form a "conjugate region".
				// In a so-called conjugate region, the digits can only appear in these cells in this region.
				foreach (int[] combination in (tempMask & orMask).GetAllSets().GetSubsets(currentMap.Count - 1))
				{
					short combinationMask = 0;
					var combinationMap = Cells.Empty;
					bool flag = false;
					foreach (int digit in combination)
					{
						if (!(ValueMaps[digit] & RegionMaps[region]).IsEmpty)
						{
							flag = true;
							break;
						}

						combinationMask |= (short)(1 << digit);
						combinationMap |= CandMaps[digit] & RegionMaps[region];
					}
					if (flag)
					{
						// The region contains digit value, which is not a normal pattern.
						continue;
					}

					if (combinationMap != currentMap)
					{
						// If not equal, the map may contains other digits in this region.
						// Therefore the conjugate region can't form.
						continue;
					}

					// Type 4 forms. Now check eliminations.
					short finalDigits = (short)(tempMask & ~combinationMask);
					var possibleCandMaps = Cells.Empty;
					foreach (int finalDigit in finalDigits)
					{
						possibleCandMaps |= CandMaps[finalDigit];
					}
					var elimMap = combinationMap & possibleCandMaps;
					if (elimMap.IsEmpty)
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					foreach (int cell in elimMap)
					{
						foreach (int digit in finalDigits)
						{
							if (grid.Exists(cell, digit) is true)
							{
								conclusions.Add(new(ConclusionType.Elimination, cell, digit));
							}
						}
					}

					var candidateOffsets = new List<(int, ColorIdentifier)>();
					foreach (int cell in currentMap)
					{
						foreach (int digit in grid.GetCandidates(cell) & combinationMask)
						{
							candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)1));
						}
					}
					foreach (int cell in otherCellsMap)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)0));
						}
					}

					var step = new UniquePolygonType4Step(
						conclusions.ToImmutableArray(),
						ImmutableArray.Create(new PresentationData
						{
							Candidates = candidateOffsets,
							Regions = new[] { (region, (ColorIdentifier)0) }
						}),
						map,
						otherMask,
						currentMap,
						combinationMask
					);
					if (findOnlyOne)
					{
						return step;
					}

					accumulator.Add(step);
				}
			}
		}

		return null;
	}
}
