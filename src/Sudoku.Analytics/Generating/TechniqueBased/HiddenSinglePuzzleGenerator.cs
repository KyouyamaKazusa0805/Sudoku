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
	public bool HasBlockExcluders { get; set; }

	/// <inheritdoc/>
	public override TechniqueSet SupportedTechniques
		=> [
			Technique.HiddenSingleBlock, Technique.HiddenSingleRow, Technique.HiddenSingleColumn,
			Technique.CrosshatchingBlock, Technique.CrosshatchingRow, Technique.CrosshatchingColumn
		];


	/// <inheritdoc/>
	public override JustOneCellPuzzle GenerateJustOneCell()
	{
		var selectedHouse = Rng.Next(0, 27);
		var digit = Rng.Next(0, 9);

		ShuffleSequence(DigitSeed);

		var begin = (int)SingleTechniqueSubtype.BlockHiddenSingle000;
		var subtypes = Enum.GetValues<SingleTechniqueSubtype>()[
			selectedHouse switch
			{
				< 9 => begin..(int)SingleTechniqueSubtype.RowHiddenSingle000,
				< 18 => begin..(int)SingleTechniqueSubtype.ColumnHiddenSingle000,
				_ => begin..(int)SingleTechniqueSubtype.NakedSingle0,
			}
		];
		SingleTechniqueSubtype selectedSubtype;
		do
		{
			selectedSubtype = subtypes[Rng.Next(0, subtypes.Length)];
		} while (selectedSubtype.IsUnnecessary());

		var forBlock = g1;
		var forLine = g2;
		return (selectedHouse < 9 ? forBlock : forLine)(selectedHouse, selectedSubtype);


		static JustOneCellPuzzle g1(House targetHouse, SingleTechniqueSubtype selectedSubtype)
		{
			// Placeholders may not necessary. Just check for excluders.
			var subtypeString = selectedSubtype.ToString();
			var neededRowExcludersCount = subtypeString[^2] - '0';
			var neededColumnExcludersCount = subtypeString[^1] - '0';

			// Determine the target cell and excluders.
			var cellsInHouse = HousesMap[targetHouse];
			var (excluderRows, excluderColumns) = (0, 0);
			var (rows, columns) = (cellsInHouse.RowMask << 9, cellsInHouse.ColumnMask << 18);
			for (var i = 0; i < neededRowExcludersCount; i++)
			{
				House excluderHouse;
				do
				{
					excluderHouse = Rng.Next(9, 18);
				} while ((rows >> excluderHouse & 1) == 0);
				_ = (excluderRows |= 1 << excluderHouse, rows &= ~(1 << excluderHouse));
			}
			for (var i = 0; i < neededColumnExcludersCount; i++)
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
			var placeholderCells = uncoveredCells - targetCell;
			var tempIndex = 0;
			foreach (var placeholderCell in placeholderCells)
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
				new HiddenSingleStep([], [], new(), targetCell, targetDigit, targetHouse, false, selectedSubtype)
			);
		}

		static JustOneCellPuzzle g2(House selectedHouse, SingleTechniqueSubtype selectedSubtype)
			=> new JustOneCellPuzzleFailed(GeneratingFailedReason.NotSupported);
	}

	/// <inheritdoc/>
	public override FullPuzzle GenerateUnique(CancellationToken cancellationToken = default)
		=> new FullPuzzleFailed(GeneratingFailedReason.Unnecessary);
}
