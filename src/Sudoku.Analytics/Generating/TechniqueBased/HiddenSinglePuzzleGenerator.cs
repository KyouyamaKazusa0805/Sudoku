namespace Sudoku.Generating.TechniqueBased;

/// <summary>
/// Represents a puzzle generator that uses hidden single.
/// </summary>
public sealed class HiddenSinglePuzzleGenerator : SinglePuzzleGenerator
{
	/// <summary>
	/// Indicates whether the generator will create for block excluders.
	/// This option will only be used if the generator generates for hidden single in lines.
	/// </summary>
	public bool AllowsBlockExcluders { get; set; }

	/// <inheritdoc/>
	public override TechniqueSet SupportedTechniques
		=> [
			Technique.HiddenSingleBlock, Technique.HiddenSingleRow, Technique.HiddenSingleColumn,
			Technique.CrosshatchingBlock, Technique.CrosshatchingRow, Technique.CrosshatchingColumn
		];


	/// <inheritdoc/>
	public override JustOneCellPuzzle GenerateJustOneCell()
	{
		var house = Rng.Next(0, 27);
		var digit = Rng.Next(0, 9);

		ShuffleSequence(DigitSeed);

		var subtypes = Enum.GetValues<SingleTechniqueSubtype>()[house switch { < 9 => 4..13, < 18 => 13..28, _ => 28..43 }];
		SingleTechniqueSubtype subtype;
		do
		{
			subtype = subtypes[Rng.Next(0, subtypes.Length)];
		} while (subtype.IsUnnecessary() || !AllowsBlockExcluders && subtype.GetExcludersCount(HouseType.Block) != 0);

		var a = GenerateForBlock;
		var b = GenerateForLine;
		return (house < 9 ? a : b)(house, subtype);
	}

	/// <inheritdoc/>
	public override FullPuzzle GenerateUnique(CancellationToken cancellationToken = default)
		=> new FullPuzzleFailed(GeneratingFailedReason.Unnecessary);

	/// <summary>
	/// Generate for block.
	/// </summary>
	/// <param name="house">The target house.</param>
	/// <param name="subtype">The selected subtype.</param>
	/// <returns>The final result generated.</returns>
	private JustOneCellPuzzle GenerateForBlock(House house, SingleTechniqueSubtype subtype)
	{
		// Placeholders may not necessary. Just check for excluders.
		var cellsInHouse = HousesMap[house];
		var (excluderRows, excluderColumns) = (0, 0);
		var (rows, columns) = (cellsInHouse.RowMask << 9, cellsInHouse.ColumnMask << 18);
		for (var i = 0; i < subtype.GetExcludersCount(HouseType.Row); i++)
		{
			House excluderHouse;
			do
			{
				excluderHouse = Rng.Next(9, 18);
			} while ((rows >> excluderHouse & 1) == 0);
			_ = (excluderRows |= 1 << excluderHouse, rows &= ~(1 << excluderHouse));
		}
		for (var i = 0; i < subtype.GetExcludersCount(HouseType.Column); i++)
		{
			House excluderHouse;
			do
			{
				excluderHouse = Rng.Next(18, 27);
			} while ((columns >> excluderHouse & 1) == 0);
			_ = (excluderColumns |= 1 << excluderHouse, columns &= ~(1 << excluderHouse));
		}
		var excluders = (CellMap)[];
		foreach (var r in excluderRows)
		{
			var lastCellsAvailable = HousesMap[r] - cellsInHouse - excluders.ExpandedPeers;
			excluders.Add(lastCellsAvailable[Rng.Next(0, lastCellsAvailable.Count)]);
		}
		foreach (var c in excluderColumns)
		{
			var lastCellsAvailable = HousesMap[c] - cellsInHouse - excluders.ExpandedPeers;
			excluders.Add(lastCellsAvailable[Rng.Next(0, lastCellsAvailable.Count)]);
		}

		// Now checks for uncovered cells in the target house, one of the uncovered cells is the target cell,
		// and the others should be placeholders.
		ShuffleSequence(DigitSeed);
		var targetDigit = DigitSeed[Rng.Next(0, 9)];
		var puzzle = Grid.Empty;
		var uncoveredCells = cellsInHouse - excluders.ExpandedPeers;
		var targetCell = uncoveredCells[Rng.Next(0, uncoveredCells.Count)];
		var tempIndex = 0;
		foreach (var placeholderCell in uncoveredCells - targetCell)
		{
			if (DigitSeed[tempIndex] == targetDigit)
			{
				tempIndex++;
			}

			puzzle.SetDigit(placeholderCell, DigitSeed[tempIndex]);
			puzzle.SetState(placeholderCell, CellState.Given);
			tempIndex++;
		}
		foreach (var excluder in excluders)
		{
			puzzle.SetDigit(excluder, targetDigit);
			puzzle.SetState(excluder, CellState.Given);
		}

		return new JustOneCellPuzzleSuccessful(
			in puzzle,
			targetCell,
			targetDigit,
			new HiddenSingleStep([], [], new(), targetCell, targetDigit, house, false, subtype)
		);
	}

	/// <summary>
	/// Generate for line.
	/// </summary>
	/// <param name="house">The target house.</param>
	/// <param name="subtype">The selected subtype.</param>
	/// <returns>The final result generated.</returns>
	private JustOneCellPuzzle GenerateForLine(House house, SingleTechniqueSubtype subtype)
	{
		var cellsInHouse = HousesMap[house];
		while (true)
		{
			var (excluderBlocks, excluderLines) = (0, 0);
			var (blocks, lines) = (cellsInHouse.BlockMask << 9, house < 18 ? cellsInHouse.ColumnMask << 18 : cellsInHouse.RowMask << 9);
			for (var i = 0; i < subtype.GetExcludersCount(HouseType.Block); i++)
			{
				House excluderHouse;
				do
				{
					excluderHouse = Rng.Next(0, 9);
				} while ((blocks >> excluderHouse & 1) == 0);
				_ = (excluderBlocks |= 1 << excluderHouse, blocks &= ~(1 << excluderHouse));
			}
			for (var i = 0; i < subtype.GetExcludersCount(house < 18 ? HouseType.Column : HouseType.Row); i++)
			{
				House excluderHouse;
				do
				{
					excluderHouse = house < 18 ? Rng.Next(18, 27) : Rng.Next(9, 18);
				} while ((lines >> excluderHouse & 1) == 0);
				_ = (excluderLines |= 1 << excluderHouse, lines &= ~(1 << excluderHouse));
			}
			var excluders = (CellMap)[];
			foreach (var r in excluderBlocks)
			{
				var lastCellsAvailable = HousesMap[r] - cellsInHouse - excluders.ExpandedPeers;
				excluders.Add(lastCellsAvailable[Rng.Next(0, lastCellsAvailable.Count)]);
			}
			foreach (var c in excluderLines)
			{
				var lastCellsAvailable = HousesMap[c] - cellsInHouse - excluders.ExpandedPeers;
				excluders.Add(lastCellsAvailable[Rng.Next(0, lastCellsAvailable.Count)]);
			}

			// Now checks for uncovered cells in the target house, one of the uncovered cells is the target cell,
			// and the others should be placeholders.
			ShuffleSequence(DigitSeed);
			var targetDigit = DigitSeed[Rng.Next(0, 9)];
			var puzzle = Grid.Empty;
			var uncoveredCells = cellsInHouse - excluders.ExpandedPeers;
			var targetCell = uncoveredCells[Rng.Next(0, uncoveredCells.Count)];
			var tempIndex = 0;
			foreach (var placeholderCell in uncoveredCells - targetCell)
			{
				if (DigitSeed[tempIndex] == targetDigit)
				{
					tempIndex++;
				}

				puzzle.SetDigit(placeholderCell, DigitSeed[tempIndex]);
				puzzle.SetState(placeholderCell, CellState.Given);
				tempIndex++;
			}
			if (!AllowsBlockExcluders)
			{
				var emptyCellsRelatedBlocksContainAnyExcluder = false;
				foreach (var block in (HousesMap[house] - puzzle.GivenCells).BlockMask)
				{
					if (HousesMap[block] & excluders)
					{
						emptyCellsRelatedBlocksContainAnyExcluder = true;
						break;
					}
				}
				if (emptyCellsRelatedBlocksContainAnyExcluder)
				{
					// Invalid case. Try again.
					continue;
				}
			}

			foreach (var excluder in excluders)
			{
				puzzle.SetDigit(excluder, targetDigit);
				puzzle.SetState(excluder, CellState.Given);
			}

			return new JustOneCellPuzzleSuccessful(
				in puzzle,
				targetCell,
				targetDigit,
				new HiddenSingleStep([], [], new(), targetCell, targetDigit, house, false, subtype)
			);
		}
	}
}
