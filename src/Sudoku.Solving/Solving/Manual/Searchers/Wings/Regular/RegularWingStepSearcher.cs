namespace Sudoku.Solving.Manual.Wings.Regular;

/// <summary>
/// Encapsulates a <b>regular wing</b> technique solver.
/// </summary>
public sealed class RegularWingStepSearcher : WingStepSearcher
{
	/// <summary>
	/// Indicates the maximum size the searcher will search for. The maximum possible value is 9.
	/// </summary>
	public int MaxSize { get; set; }

	/// <inheritdoc/>
	public override SearchingOptions Options { get; set; } = new(6, DisplayingLevel.B);

	/// <summary>
	/// Indicates the searcher properties.
	/// </summary>
	/// <remarks>
	/// Please note that all technique searches should contain
	/// this static property in order to display on settings window. If the searcher doesn't contain,
	/// when we open the settings window, it'll throw an exception to report about this.
	/// </remarks>
	[Obsolete("Please use the property '" + nameof(Options) + "' instead.", false)]
	public static TechniqueProperties Properties { get; } = new(6, nameof(Technique.XyWing))
	{
		DisplayLevel = 2
	};


	/// <inheritdoc/>
	public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
	{
		// Iterate on the size.
		// Note that the greatest size is determined by two factors: the size that you specified
		// and the number of bi-value cells in the grid.
		for (int size = 3, count = Math.Min(MaxSize, BivalueMap.Count); size <= count; size++)
		{
			// Iterate on each pivot cell.
			foreach (int pivot in EmptyMap)
			{
				short mask = grid.GetCandidates(pivot);
				int candsCount = PopCount((uint)mask);
				if (candsCount != size && candsCount != size - 1)
				{
					// Candidates are not enough.
					continue;
				}

				var map = PeerMaps[pivot] & BivalueMap;
				if (map.Count < size - 1)
				{
					// Bivalue cells are not enough.
					continue;
				}

				// Iterate on each cell combination.
				foreach (int[] cells in map.ToArray().GetSubsets(size - 1))
				{
					// Check duplicate.
					// If two cells contain same candidates, the wing can't be formed.
					bool flag = false;
					for (
						int i = 0, length = cells.Length, iterationLength = length - 1;
						i < iterationLength;
						i++
					)
					{
						for (int j = i + 1; j < length; j++)
						{
							if (grid.GetMask(cells[i]) == grid.GetMask(cells[j]))
							{
								flag = true;
								goto CheckWhetherTwoCellsContainSameCandidateKind;
							}
						}
					}

				CheckWhetherTwoCellsContainSameCandidateKind:
					if (flag)
					{
						continue;
					}

					short union = mask, inter = (short)(SudokuGrid.MaxCandidatesMask & mask);
					foreach (int cell in cells)
					{
						short m = grid.GetCandidates(cell);
						union |= m;
						inter &= m;
					}

					if (PopCount((uint)union) != size || inter != 0
						&& (inter == 0 || (inter & inter - 1) != 0))
					{
						continue;
					}

					// Get the Z digit (The digit to be removed).
					bool isIncomplete = inter == 0;
					short interWithoutPivot = (short)(union & ~grid.GetCandidates(pivot));
					short maskToCheck = isIncomplete ? interWithoutPivot : inter;
					if (maskToCheck == 0 || (maskToCheck & maskToCheck - 1) != 0)
					{
						continue;
					}

					// The pattern should be "az, bz, cz, dz, ... , abcd(z)".
					int zDigit = TrailingZeroCount(maskToCheck);
					var cellsMap = new Cells(cells);
					if (
						(new Cells(cellsMap) { pivot } & CandMaps[zDigit]).Count
						!= (isIncomplete ? size - 1 : size)
					)
					{
						continue;
					}

					// Check elimination map.
					var elimMap = cellsMap.PeerIntersection;
					if (!isIncomplete)
					{
						elimMap &= PeerMaps[pivot];
					}
					elimMap &= CandMaps[zDigit];
					if (elimMap.IsEmpty)
					{
						continue;
					}

					// Gather the eliminations.
					var conclusions = new List<Conclusion>();
					foreach (int cell in elimMap)
					{
						conclusions.Add(new(ConclusionType.Elimination, cell, zDigit));
					}

					// Gather highlight candidates.
					var candidateOffsets = new List<DrawingInfo>();
					foreach (int cell in cells)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(digit == zDigit ? 1 : 0, cell * 9 + digit));
						}
					}
					foreach (int digit in grid.GetCandidates(pivot))
					{
						candidateOffsets.Add(new(digit == zDigit ? 1 : 0, pivot * 9 + digit));
					}

					accumulator.Add(
						new RegularWingStepInfo(
							conclusions,
							new View[] { new() { Candidates = candidateOffsets } },
							pivot,
							PopCount((uint)mask),
							union,
							cells
						)
					);
				}
			}
		}
	}
}
