namespace Sudoku.Solving.Manual.Uniqueness.Loops;

partial class UlStepSearcher
{
	/// <summary>
	/// Check type 1.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="loop">The loop.</param>
	/// <param name="links">The links.</param>
	/// <param name="extraCellsMap">The extra cells map.</param>
	partial void CheckType1(
		IList<UlStepInfo> accumulator, in SudokuGrid grid, int d1, int d2,
		in Cells loop, IReadOnlyList<Link> links, in Cells extraCellsMap)
	{
		int extraCell = extraCellsMap[0];
		var conclusions = new List<Conclusion>();
		if (grid.Exists(extraCell, d1) is true)
		{
			conclusions.Add(new(ConclusionType.Elimination, extraCell, d1));
		}
		if (grid.Exists(extraCell, d2) is true)
		{
			conclusions.Add(new(ConclusionType.Elimination, extraCell, d2));
		}
		if (conclusions.Count == 0)
		{
			return;
		}

		var candidateOffsets = new List<DrawingInfo>();
		foreach (int cell in new Cells(loop) { ~extraCell })
		{
			candidateOffsets.Add(new(0, cell * 9 + d1));
			candidateOffsets.Add(new(0, cell * 9 + d2));
		}

		accumulator.Add(
			new UlType1StepInfo(
				conclusions,
				new View[] { new() { Candidates = candidateOffsets, Links = links } },
				d1,
				d2,
				loop
			)
		);
	}

	/// <summary>
	/// Check type 2.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="loop">The loop.</param>
	/// <param name="links">The links.</param>
	/// <param name="extraCellsMap">The extra cells map.</param>
	/// <param name="comparer">The comparer mask (equals to <c><![CDATA[1 << d1 | 1 << d2]]></c>).</param>
	partial void CheckType2(
		IList<UlStepInfo> accumulator, in SudokuGrid grid, int d1, int d2,
		in Cells loop, IReadOnlyList<Link> links, in Cells extraCellsMap, short comparer)
	{
		short mask = 0;
		foreach (int cell in extraCellsMap)
		{
			mask |= grid.GetCandidates(cell);
		}
		mask &= (short)~comparer;

		if (mask == 0 || (mask & mask - 1) != 0)
		{
			return;
		}

		int extraDigit = TrailingZeroCount(mask);
		var elimMap = extraCellsMap % CandMaps[extraDigit];
		if (elimMap.IsEmpty)
		{
			return;
		}

		var conclusions = new List<Conclusion>();
		foreach (int cell in elimMap)
		{
			conclusions.Add(new(ConclusionType.Elimination, cell, extraDigit));
		}

		var candidateOffsets = new List<DrawingInfo>();
		foreach (int cell in loop)
		{
			foreach (int digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(new(digit == extraDigit ? 1 : 0, cell * 9 + digit));
			}
		}

		accumulator.Add(
			new UlType2StepInfo(
				conclusions,
				new View[] { new() { Candidates = candidateOffsets, Links = links } },
				d1,
				d2,
				loop,
				extraDigit
			)
		);
	}

	/// <summary>
	/// Check type 3.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="loop">The loop.</param>
	/// <param name="links">The links.</param>
	/// <param name="extraCellsMap">The extra cells map.</param>
	/// <param name="comparer">The comparer mask (equals to <c><![CDATA[1 << d1 | 1 << d2]]></c>).</param>
	partial void CheckType3(
		IList<UlStepInfo> accumulator, in SudokuGrid grid, int d1, int d2,
		in Cells loop, IReadOnlyList<Link> links, in Cells extraCellsMap, short comparer)
	{
		bool notSatisfiedType3 = false;
		foreach (int cell in extraCellsMap)
		{
			short mask = grid.GetCandidates(cell);
			if ((mask & comparer) == 0 || mask == comparer)
			{
				notSatisfiedType3 = true;
				break;
			}
		}
		if (!extraCellsMap.InOneRegion || notSatisfiedType3)
		{
			return;
		}

		short m = 0;
		foreach (int cell in extraCellsMap)
		{
			m |= grid.GetCandidates(cell);
		}
		if ((m & comparer) != comparer)
		{
			return;
		}

		short otherDigitsMask = (short)(m & ~comparer);
		foreach (int region in extraCellsMap.CoveredRegions)
		{
			if (!((ValueMaps[d1] | ValueMaps[d2]) & RegionMaps[region]).IsEmpty)
			{
				return;
			}

			int[] otherCells = ((RegionMaps[region] & EmptyMap) - loop).ToArray();
			for (int size = PopCount((uint)otherDigitsMask) - 1, count = otherCells.Length; size < count; size++)
			{
				foreach (int[] cells in otherCells.GetSubsets(size))
				{
					short mask = 0;
					foreach (int cell in cells)
					{
						mask |= grid.GetCandidates(cell);
					}

					if (PopCount((uint)mask) != size + 1 || (mask & otherDigitsMask) != otherDigitsMask)
					{
						continue;
					}

					var elimMap = (RegionMaps[region] & EmptyMap) - cells - loop;
					if (elimMap.IsEmpty)
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					foreach (int digit in mask)
					{
						foreach (int cell in elimMap & CandMaps[digit])
						{
							conclusions.Add(new(ConclusionType.Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<DrawingInfo>();
					foreach (int cell in loop)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(
								new((otherDigitsMask >> digit & 1) != 0 ? 1 : 0, cell * 9 + digit)
							);
						}
					}
					foreach (int cell in cells)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(1, cell * 9 + digit));
						}
					}

					accumulator.Add(
						new UlType3StepInfo(
							conclusions,
							new View[]
							{
								new()
								{
									Candidates = candidateOffsets,
									Regions = new DrawingInfo[] { new(0, region) },
									Links = links
								}
							},
							d1,
							d2,
							loop,
							mask,
							cells
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
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="loop">The loop.</param>
	/// <param name="links">The links.</param>
	/// <param name="extraCellsMap">The extra cells map.</param>
	/// <param name="comparer">The comparer mask (equals to <c><![CDATA[1 << d1 | 1 << d2]]></c>).</param>
	partial void CheckType4(
		IList<UlStepInfo> accumulator, in SudokuGrid grid, int d1, int d2,
		in Cells loop, IReadOnlyList<Link> links, in Cells extraCellsMap, short comparer)
	{
		if (!extraCellsMap.InOneRegion)
		{
			return;
		}

		foreach (int region in extraCellsMap.CoveredRegions)
		{
			foreach (var (digit, otherDigit) in stackalloc[] { (d1, d2), (d2, d1) })
			{
				var map = RegionMaps[region] & CandMaps[digit];
				if (map != (RegionMaps[region] & loop))
				{
					continue;
				}

				int[] offsets = extraCellsMap.ToArray();
				int first = offsets[0], second = offsets[1];
				var conclusions = new List<Conclusion>();
				if (grid.Exists(first, otherDigit) is true)
				{
					conclusions.Add(new(ConclusionType.Elimination, first, otherDigit));
				}
				if (grid.Exists(second, otherDigit) is true)
				{
					conclusions.Add(new(ConclusionType.Elimination, second, otherDigit));
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<DrawingInfo>();
				foreach (int cell in loop - extraCellsMap)
				{
					foreach (int d in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(0, cell * 9 + d));
					}
				}
				foreach (int cell in extraCellsMap)
				{
					candidateOffsets.Add(new(1, cell * 9 + digit));
				}

				accumulator.Add(
					new UlType4StepInfo(
						conclusions,
						new View[]
						{
							new()
							{
								Candidates = candidateOffsets,
								Regions = new DrawingInfo[] { new(0, region) },
								Links = links
							}
						},
						d1,
						d2,
						loop,
						new(first, second, digit)
					)
				);
			}
		}
	}
}
