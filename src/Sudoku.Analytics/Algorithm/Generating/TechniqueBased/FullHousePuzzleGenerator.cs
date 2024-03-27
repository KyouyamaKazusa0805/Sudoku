namespace Sudoku.Algorithm.Generating.TechniqueBased;

/// <summary>
/// Represents a type that generates puzzles that only contains full house usages.
/// </summary>
public sealed class FullHousePuzzleGenerator : SinglePuzzleGenerator
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
	public override GenerationResult GenerateJustOneCell(out Grid result, CancellationToken cancellationToken = default)
	{
		var selectedHouse = Rng.Next(0, 27);
		var digitMissing = Rng.Next(0, 9);

		var i = 0;
		for (; i < 3; i++)
		{
			Rng.Shuffle(DigitSeed);
		}

		(result, i) = (Grid.Empty, 0);
		foreach (var cell in HousesCells[selectedHouse])
		{
			result.SetDigit(cell, DigitSeed[i++]);
			result.SetState(cell, CellState.Given);
		}

		var targetCell = HousesCells[selectedHouse][digitMissing];
		result.SetDigit(targetCell, -1);
		result.SetState(targetCell, CellState.Empty);
		return GenerationResult.Success;
	}

	/// <inheritdoc/>
	public override GenerationResult GenerateUnique(out Grid result, CancellationToken cancellationToken = default)
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
				result = Grid.Undefined;
				return GenerationResult.Canceled;
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
				result = grid.FixedGrid;
				return GenerationResult.Success;
			}

			cancellationToken.ThrowIfCancellationRequested();
		}
	}
}
