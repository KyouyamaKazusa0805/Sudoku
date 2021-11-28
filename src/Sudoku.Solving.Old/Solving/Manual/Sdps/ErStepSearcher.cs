namespace Sudoku.Solving.Manual.Sdps;

/// <summary>
/// Encapsulates an <b>empty rectangle</b> (<b>ER</b>) technique searcher.
/// </summary>
public sealed partial class ErStepSearcher : SdpStepSearcher
{
	/// <summary>
	/// Indicates the searcher properties.
	/// </summary>
	/// <remarks>
	/// Please note that all technique searches should contain
	/// this static property in order to display on settings window. If the searcher doesn't contain,
	/// when we open the settings window, it'll throw an exception to report about this.
	/// </remarks>
	public static TechniqueProperties Properties { get; } = new(12, nameof(Technique.EmptyRectangle))
	{
		DisplayLevel = 2
	};


	/// <inheritdoc/>
	public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
	{
		for (int digit = 0; digit < 9; digit++)
		{
			for (int block = 0; block < 9; block++)
			{
				// Check the empty rectangle occupies more than 2 cells.
				// and the structure forms an empty rectangle.
				var erMap = CandMaps[digit] & RegionMaps[block];
				if (erMap.Count < 2 || !erMap.IsEmptyRectangle(block, out int row, out int column))
				{
					continue;
				}

				// Search for conjugate pair.
				for (int i = 0; i < 12; i++)
				{
					var linkMap = CandMaps[digit] & RegionMaps[LinkIds[block, i]];
					if (linkMap.Count != 2)
					{
						continue;
					}

					short blockMask = linkMap.BlockMask;
					if (IsPow2(blockMask)
						|| i < 6 && (linkMap & RegionMaps[column]).IsEmpty
						|| i >= 6 && (linkMap & RegionMaps[row]).IsEmpty)
					{
						continue;
					}

					int[] t = (linkMap - (i < 6 ? RegionMaps[column] : RegionMaps[row])).ToArray();
					int elimRegion = i < 6 ? t[0] % 9 + 18 : t[0] / 9 + 9;
					var elimCellMap = i < 6
					? CandMaps[digit] & RegionMaps[elimRegion] & RegionMaps[row]
					: CandMaps[digit] & RegionMaps[elimRegion] & RegionMaps[column];

					if (elimCellMap.IsEmpty)
					{
						continue;
					}

					int elimCell = elimCellMap[0];
					if (grid.Exists(elimCell, digit) is not true)
					{
						continue;
					}

					// Record all highlight candidates.
					var candidateOffsets = new List<DrawingInfo>();
					var cpCells = new List<int>(2);
					foreach (int cell in RegionMaps[block] & CandMaps[digit])
					{
						candidateOffsets.Add(new(1, cell * 9 + digit));
					}
					foreach (int cell in linkMap)
					{
						candidateOffsets.Add(new(0, cell * 9 + digit));
						cpCells.Add(cell);
					}

					// Empty rectangle.
					accumulator.Add(
						new ErStepInfo(
							new Conclusion[] { new(ConclusionType.Elimination, elimCell, digit) },
							new View[]
							{
								new()
								{
									Candidates = candidateOffsets,
									Regions = new DrawingInfo[] { new(0, block) }
								}
							},
							digit,
							block,
							new(cpCells[0], cpCells[1], digit)
						)
					);
				}
			}
		}
	}
}
