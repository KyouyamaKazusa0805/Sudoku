namespace Sudoku.Solving.Manual.Subsets
{
	/// <summary>
	/// Encapsulates a <b>subset</b> technique searcher.
	/// </summary>
	public sealed class SubsetStepSearcher : StepSearcher
	{
		/// <inheritdoc/>
		public override SearchingOptions Options { get; set; } = new(3, DisplayingLevel.B);

		/// <summary>
		/// Indicates the searcher properties.
		/// </summary>
		/// <remarks>
		/// Please note that all technique searches should contain
		/// this static property in order to display on settings window. If the searcher doesn't contain,
		/// when we open the settings window, it'll throw an exception to report about this.
		/// </remarks>
		[Obsolete("Please use the property '" + nameof(Options) + "' instead.", false)]
		public static TechniqueProperties Properties { get; } = new(3, nameof(Technique.NakedPair))
		{
			DisplayLevel = 2
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			for (int size = 2; size <= 4; size++)
			{
				#region Naked subsets
				for (int region = 0; region < 27; region++)
				{
					var currentEmptyMap = RegionMaps[region] & EmptyMap;
					if (currentEmptyMap.Count < 2)
					{
						continue;
					}

					// Iterate on each combination.
					foreach (int[] cells in currentEmptyMap.ToArray().GetSubsets(size))
					{
						short mask = 0;
						foreach (int cell in cells)
						{
							mask |= grid.GetCandidates(cell);
						}
						if (PopCount((uint)mask) != size)
						{
							continue;
						}

						// Naked subset found. Now check eliminations.
						short flagMask = 0;
						var conclusions = new List<Conclusion>();
						foreach (int digit in mask)
						{
							var map = cells % CandMaps[digit];
							flagMask |= (short)(map.InOneRegion ? 0 : (1 << digit));

							foreach (int cell in map)
							{
								conclusions.Add(new(ConclusionType.Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<DrawingInfo>();
						foreach (int cell in cells)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(new(0, cell * 9 + digit));
							}
						}

						bool? isLocked = flagMask == mask ? true : flagMask != 0 ? false : null;
						accumulator.Add(
							new NakedSubsetStepInfo(
								conclusions,
								new View[]
								{
									new()
									{
										Candidates = candidateOffsets,
										Regions = new DrawingInfo[] { new(0, region) }
									}
								},
								region,
								new(cells),
								mask.GetAllSets().ToArray(),
								isLocked
							)
						);
					}
				}
				#endregion
				#region Hidden subsets
				for (int region = 0; region < 27; region++)
				{
					var traversingMap = RegionMaps[region] - EmptyMap;
					if (traversingMap.Count >= 8)
					{
						// No available digit (Or hidden single).
						continue;
					}

					short mask = SudokuGrid.MaxCandidatesMask;
					foreach (int cell in traversingMap)
					{
						mask &= (short)~(1 << grid[cell]);
					}
					foreach (int[] digits in mask.GetAllSets().GetSubsets(size))
					{
						short tempMask = mask;
						var map = Cells.Empty;
						foreach (int digit in digits)
						{
							tempMask &= (short)~(1 << digit);
							map |= RegionMaps[region] & CandMaps[digit];
						}
						if (map.Count != size)
						{
							continue;
						}

						// Gather eliminations.
						var conclusions = new List<Conclusion>();
						foreach (int digit in tempMask)
						{
							foreach (int cell in map & CandMaps[digit])
							{
								conclusions.Add(new(ConclusionType.Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						// Gather highlight candidates.
						var candidateOffsets = new List<DrawingInfo>();
						foreach (int digit in digits)
						{
							foreach (int cell in map & CandMaps[digit])
							{
								candidateOffsets.Add(new(0, cell * 9 + digit));
							}
						}

						accumulator.Add(
							new HiddenSubsetStepInfo(
								conclusions,
								new View[]
								{
									new()
									{
										Candidates = candidateOffsets,
										Regions = new DrawingInfo[] { new(0, region) }
									}
								},
								region,
								map,
								digits
							)
						);
					}
				}
				#endregion
			}
		}
	}
}
