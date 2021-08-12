using Pointer = System.Pointer;

namespace Sudoku.Solving.Manual.Symmetry
{
	/// <summary>
	/// Encapsulates a <b>Gurth's symmetrical placement 2</b> (GSP2) technique searcher.
	/// </summary>
	[DirectSearcher, IsOptionsFixed]
	[Obsolete("The searcher can be used only if this attribute is disabled.", true)]
	public sealed class Gsp2StepSearcher : SymmetryStepSearcher
	{
		/// <summary>
		/// Indicates the iteration list.
		/// </summary>
		private static readonly int[][]
			R1 = { new[] { 0, 0 }, new[] { 9, 10 }, new[] { 9, 11 }, new[] { 10, 11 } },
			R2 = { new[] { 0, 0 }, new[] { 12, 13 }, new[] { 12, 14 }, new[] { 13, 14 } },
			R3 = { new[] { 0, 0 }, new[] { 15, 16 }, new[] { 15, 17 }, new[] { 16, 17 } },
			C1 = { new[] { 0, 0 }, new[] { 18, 19 }, new[] { 18, 20 }, new[] { 19, 20 } },
			C2 = { new[] { 0, 0 }, new[] { 21, 22 }, new[] { 21, 23 }, new[] { 22, 23 } },
			C3 = { new[] { 0, 0 }, new[] { 24, 25 }, new[] { 24, 26 }, new[] { 25, 26 } };

		/// <summary>
		/// Indicates the step searcher for GSP technique.
		/// </summary>
		private static readonly GspStepSearcher Searcher = new();


		/// <inheritdoc/>
		public override SearchingOptions Options { get; set; } = new(
			default, default,
			EnabledAreas: EnabledAreas.None,
			DisabledReason: DisabledReason.TooSlow
		);

		/// <summary>
		/// Indicates the searcher properties.
		/// </summary>
		/// <remarks>
		/// Please note that all technique searches should contain
		/// this static property in order to display on settings window. If the searcher doesn't contain,
		/// when we open the settings window, it'll throw an exception to report about this.
		/// </remarks>
		[Obsolete("Please use the property '" + nameof(Options) + "' instead.", false)]
		public static TechniqueProperties Properties { get; } = new(default, nameof(Technique.Gsp2))
		{
			IsReadOnly = true,
			IsEnabled = false,
			DisabledReason = DisabledReason.TooSlow
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			// Iterate on each combination.
			foreach (int[] r1 in R1)
			{
				foreach (int[] r2 in R2)
				{
					foreach (int[] r3 in R3)
					{
						foreach (int[] c1 in C1)
						{
							foreach (int[] c2 in C2)
							{
								foreach (int[] c3 in C3)
								{
									// Now swap the regions.
									var tempGrid = grid;
									if (r1[0] != 0) SwapTwoRegions(ref tempGrid, r1[0], r1[1]);
									if (r2[0] != 0) SwapTwoRegions(ref tempGrid, r2[0], r2[1]);
									if (r3[0] != 0) SwapTwoRegions(ref tempGrid, r3[0], r3[1]);
									if (c1[0] != 0) SwapTwoRegions(ref tempGrid, c1[0], c1[1]);
									if (c2[0] != 0) SwapTwoRegions(ref tempGrid, c2[0], c2[1]);
									if (c3[0] != 0) SwapTwoRegions(ref tempGrid, c3[0], c3[1]);

									// Check whether the grid is a real Gurth's symmetrical grid.
									if (Searcher.GetOne(tempGrid) is not GspStepInfo info)
									{
										continue;
									}

									// Get all possible cells that is already moved,
									// and save then into the list.
									var conclusions = new List<Conclusion>();
									foreach (var conclusionListGroupedByCell in
										from conclusion in info.Conclusions
										group conclusion by conclusion.Cell)
									{
										// Now backtrack to the original cell.
										int cell = conclusionListGroupedByCell.Key, originalCell = cell;
										FullRestore(ref originalCell, r1, r2, r3, c1, c2, c3);

										foreach (var (t, _, d) in conclusionListGroupedByCell)
										{
											conclusions.Add(new(t, originalCell, d));
										}
									}

									// Now gather the view.
									View? newView = null;
									var oldViews = info.Views;
									if (oldViews.Count != 0)
									{
										List<DrawingInfo> newCellOffsets = new(), newCandidateOffsets = new();

										var view = oldViews[0];
										var oldCellOffsets = view.Cells;
										var oldCandidateOffsets = view.Candidates;
										if (oldCellOffsets is not null)
										{
											foreach (var (id, cell) in oldCellOffsets)
											{
												int originalCell = cell;
												FullRestore(ref originalCell, r1, r2, r3, c1, c2, c3);

												newCellOffsets.Add(new(id, originalCell));
											}
										}

										if (oldCandidateOffsets is not null)
										{
											foreach (var (id, candidate) in oldCandidateOffsets)
											{
												int originalCell = candidate / 9, digit = candidate % 9;
												FullRestore(ref originalCell, r1, r2, r3, c1, c2, c3);

												newCandidateOffsets.Add(new(id, originalCell * 9 + digit));
											}
										}

										newView = new()
										{
											Cells = newCellOffsets,
											Candidates = newCandidateOffsets
										};
									}

									// Gather the information.
									accumulator.Add(
										new Gsp2StepInfo(
											conclusions,
											new[] { newView ?? new() },
											info.SymmetryType,
											r1[0] == 0 && r2[0] == 0 && r3[0] == 0
											&& c1[0] == 0 && c2[0] == 0 && c3[0] == 0
											? null
											: new[] { r1, r2, r3, c1, c2, c3 },
											info.MappingTable
										)
									);
								}
							}
						}
					}
				}
			}
		}


		/// <summary>
		/// Full restore a cell.
		/// </summary>
		/// <param name="cell">The cell to restore.</param>
		/// <param name="r1">The region list 1 for a row.</param>
		/// <param name="r2">The region list 2 for a row.</param>
		/// <param name="r3">The region list 3 for a row.</param>
		/// <param name="c1">The region list 1 for a column.</param>
		/// <param name="c2">The region list 2 for a column.</param>
		/// <param name="c3">The region list 3 for a column.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void FullRestore(ref int cell, int[] r1, int[] r2, int[] r3, int[] c1, int[] c2, int[] c3)
		{
			restore(ref cell, c3[0], c3[1]);
			restore(ref cell, c2[0], c2[1]);
			restore(ref cell, c1[0], c1[1]);
			restore(ref cell, r3[0], r3[1]);
			restore(ref cell, r2[0], r2[1]);
			restore(ref cell, r1[0], r1[1]);

			static void restore(ref int cell, int r1, int r2)
			{
				if (r1 != 0)
				{
					if (RegionMaps[r1].Contains(r1))
					{
						for (int i = 0; i < 9; i++)
						{
							if (RegionCells[r1][i] == cell)
							{
								cell = RegionCells[r2][i];
								return;
							}
						}
					}
					else
					{
						for (int i = 0; i < 9; i++)
						{
							if (RegionCells[r2][i] == cell)
							{
								cell = RegionCells[r1][i];
								return;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Swap values on two different regions.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="region1">The region 1.</param>
		/// <param name="region2">The region 2.</param>
		private static unsafe void SwapTwoRegions(ref SudokuGrid grid, int region1, int region2)
		{
			fixed (short* pGrid = grid)
			{
				for (int i = 0; i < 9; i++)
				{
					Pointer.Swap(pGrid + RegionCells[region1][i], pGrid + RegionCells[region2][i]);
				}
			}
		}
	}
}
