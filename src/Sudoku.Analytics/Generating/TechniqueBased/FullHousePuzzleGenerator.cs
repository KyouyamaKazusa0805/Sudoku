namespace Sudoku.Generating.TechniqueBased;

/// <summary>
/// Represents a type that generates puzzles that only contains full house usages.
/// </summary>
public sealed class FullHousePuzzleGenerator : SinglePuzzleGenerator<FullHouseStep>
{
	/// <summary>
	/// Represents an analyzer.
	/// </summary>
	private static readonly Analyzer Analyzer = new() { StepSearchers = [new SingleStepSearcher { EnableFullHouse = true }] };


	/// <summary>
	/// <para>Indicates the number of empty cells that generated puzzles will be used.</para>
	/// <para>
	/// The value can be all possible integers between -1 and 21, without 0.
	/// If the value is -1, all possible number of empty cells in a puzzle can be tried; otherwise set a value between 1 and 21.
	/// </para>
	/// </summary>
	public int EmptyCellsCount { get; set; } = -1;

	/// <inheritdoc/>
	public override SudokuType SupportedTypes => base.SupportedTypes | SudokuType.Standard;

	/// <inheritdoc/>
	public override TechniqueSet SupportedTechniques => [Technique.FullHouse];


	/// <inheritdoc/>
	public override JustOneCellPuzzle GenerateJustOneCell()
	{
		// Choose the target house.
		var selectedHouse = Alignment switch
		{
			GridAlignment.NotLimited => Rng.Next(0, 27),
			GridAlignment.CenterHouses => 9 * Rng.Next(0, 3) + 4,
			GridAlignment.CenterBlock => CenterHouses[Rng.Next(0, CenterHouses.Length)],
			_ => StrictCenterHouses[Rng.Next(0, StrictCenterHouses.Length)]
		};

		// Shuffle the digits.
		ShuffleSequence(DigitSeed);

		// Set the given values.
		var (puzzle, i) = (Grid.Empty, 0);
		foreach (var cell in HousesCells[selectedHouse])
		{
			puzzle.SetDigit(cell, DigitSeed[i++]);
			puzzle.SetState(cell, CellState.Modifiable);
		}

		// Clear the target cell with the value set -1.
		int targetCell, targetDigit;
		switch (Alignment)
		{
			case GridAlignment.NotLimited or GridAlignment.CenterHouses:
			case GridAlignment.CenterBlock when selectedHouse == 4:
			{
				var missingPos = Rng.Next(0, 9);
				targetCell = HousesCells[selectedHouse][missingPos];
				targetDigit = DigitSeed[missingPos];
				break;
			}
			case GridAlignment.CenterBlock:
			{
				var availableCells = HousesMap[selectedHouse] & HousesMap[4];
				targetCell = availableCells[Rng.Next(0, availableCells.Count)];
				targetDigit = puzzle.GetDigit(targetCell);
				break;
			}
			case GridAlignment.OnlyCenterCell:
			{
				targetDigit = puzzle.GetDigit(targetCell = 40);
				break;
			}
			default:
			{
				return new JustOneCellPuzzleFailed(GeneratingFailedReason.InvalidData);
			}
		}

		puzzle.SetDigit(targetCell, -1);
		return new JustOneCellPuzzleSuccessful(
			puzzle.FixedGrid,
			targetCell,
			targetDigit,
			new FullHouseStep(null!, null, null!, selectedHouse, targetCell, targetDigit)
		);
	}

	/// <inheritdoc/>
	public override FullPuzzle GenerateUnique(CancellationToken cancellationToken = default)
	{
		if (EmptyCellsCount is not (-1 or >= 1 and <= 21))
		{
			EmptyCellsCount = Math.Clamp(EmptyCellsCount, 1, 21);
		}

		for (var i = 1; ; i++)
		{
			// Try generating a solution.
			var grid = new HodokuPuzzleGenerator().Generate(cancellationToken: cancellationToken);
			if (grid.IsUndefined)
			{
				return new FullPuzzleFailed(GeneratingFailedReason.Canceled);
			}

			// Replace with solution grid.
			grid = grid.SolutionGrid.UnfixedGrid;

			// Then randomly removed some digits in some cells, and keeps the grid valid.
			for (var times = 0; times < 3; times++)
			{
				Rng.Shuffle(CellSeed);
			}

			// Removes the selected cells.
			var pattern = (CellMap)CellSeed[..(EmptyCellsCount == -1 ? Rng.Next(1, 22) : EmptyCellsCount)];
			foreach (var cell in pattern)
			{
				grid.SetDigit(cell, -1);
			}

			// Fix the grid and check validity.
			grid.Fix();
			if (grid.IsValid
				&& Analyzer.Analyze(in grid, cancellationToken: cancellationToken) is { IsSolved: true, Steps: var steps }
				&& new SortedSet<Technique>(from step in steps select step.Code).Max == Technique.FullHouse)
			{
				// Check validity of the puzzle.
				return new FullPuzzleSuccessful(grid.FixedGrid);
			}

			cancellationToken.ThrowIfCancellationRequested();
		}
	}
}
