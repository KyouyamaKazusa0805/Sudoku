namespace Sudoku.Runtime.GeneratingServices;

using static IAlignedJustOneCellGenerator;

/// <summary>
/// Represents a generator that supports generating for puzzles that can be solved by only using Full Houses.
/// </summary>
/// <seealso cref="Technique.FullHouse"/>
public sealed class FullHousePrimaryGenerator :
	IAlignedJustOneCellGenerator,
	IJustOneCellGenerator,
	IPrimaryGenerator,
	ITechniqueBasedGenerator
{
	/// <summary>
	/// Represents an analyzer.
	/// </summary>
	private static readonly Analyzer Analyzer = Analyzer.Default.WithStepSearchers(new SingleStepSearcher { EnableFullHouse = true });


	/// <inheritdoc/>
	public TechniqueSet SupportedTechniques => [Technique.FullHouse];


	/// <inheritdoc cref="ITechniqueBasedGenerator.GenerateUnique"/>
	public Grid GenerateUnique(Cell emptyCellsCount, CancellationToken cancellationToken = default)
	{
		if (emptyCellsCount is not (-1 or >= 1 and <= PeersCount + 1))
		{
			emptyCellsCount = Math.Clamp(emptyCellsCount, 1, PeersCount + 1);
		}

		var generator = new Generator();
		while (true)
		{
			// Try generating a solution.
			var grid = generator.Generate(cancellationToken: cancellationToken);
			if (grid.IsUndefined)
			{
				return Grid.Undefined;
			}

			// Replace with solution grid.
			grid = grid.GetSolutionGrid().UnfixedGrid;

			// Then randomly removed some digits in some cells, and keeps the grid valid.
			ShuffleSequence(CellSeed);

			// Removes the selected cells.
			var pattern = CellSeed[..(emptyCellsCount == -1 ? Rng.Next(1, 22) : emptyCellsCount)].AsCellMap();
			foreach (var cell in pattern)
			{
				grid.SetDigit(cell, -1);
			}

			// Fix the grid and check validity.
			grid.Fix();
			if (grid.GetIsValid()
				&& Analyzer.Analyze(in grid, cancellationToken: cancellationToken) is { IsSolved: true, InterimSteps: var steps }
				&& new SortedSet<Technique>(from step in steps select step.Code).Max == Technique.FullHouse)
			{
				return grid.FixedGrid;
			}

			cancellationToken.ThrowIfCancellationRequested();
		}
	}

	/// <inheritdoc/>
	public Grid GenerateJustOneCell(ConclusionCellAlignment alignment, out Step? step, CancellationToken cancellationToken = default)
	{
		// Choose the target house.
		var selectedHouse = RandomlySelectHouse(alignment);

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
		var (targetCell, targetDigit) = (alignment, selectedHouse) switch
		{
			(ConclusionCellAlignment.NotLimited or ConclusionCellAlignment.CenterHouse, _) when Rng.NextDigit() is var missingPos
				=> (HousesCells[selectedHouse][missingPos], DigitSeed[missingPos]),
			(ConclusionCellAlignment.CenterBlock, 4) when Rng.NextDigit() is var missingPos
				=> (HousesCells[selectedHouse][missingPos], DigitSeed[missingPos]),
			(ConclusionCellAlignment.CenterBlock, _)
				when (HousesMap[selectedHouse] & HousesMap[4]) is var a && a[Rng.Next(0, a.Count)] is var t
				=> (t, puzzle.GetDigit(t)),
			(ConclusionCellAlignment.CenterCell, _) => (40, puzzle.GetDigit(40)),
			_ => (-1, -1)
		};
		if ((targetCell, targetDigit) == (-1, -1))
		{
			throw new InvalidOperationException();
		}

		// Leave the target cell to be empty.
		puzzle.SetDigit(targetCell, -1);

		step = new FullHouseStep(
			null!,
			null,
			null!,
			selectedHouse,
			targetCell,
			targetDigit,
			SingleModule.GetLasting(in puzzle, targetCell, selectedHouse)
		);
		return puzzle.FixedGrid;
	}

	/// <inheritdoc/>
	public Grid GenerateJustOneCell(out Grid phasedGrid, out Step? step, CancellationToken cancellationToken = default)
	{
		while (true)
		{
			var puzzle = new Generator().Generate(cancellationToken: cancellationToken);
			var analysisResult = SingleAnalyzer.Analyze(in puzzle, cancellationToken: cancellationToken);
			if (analysisResult is not { IsSolved: true, InterimGrids: var interimGrids, InterimSteps: var interimSteps })
			{
				cancellationToken.ThrowIfCancellationRequested();
				continue;
			}

			foreach (var (currentGrid, s) in StepMarshal.Combine(interimGrids, interimSteps))
			{
				if (s is not FullHouseStep { House: var house })
				{
					continue;
				}

				var extractedGrid = currentGrid;
				extractedGrid.Unfix();
				for (var c = 0; c < 81; c++)
				{
					if (!HousesMap[house].Contains(c))
					{
						extractedGrid.SetDigit(c, -1);
					}
				}

				(phasedGrid, step) = (currentGrid, s);
				return extractedGrid.FixedGrid;
			}
		}
	}

	/// <inheritdoc/>
	Grid ITechniqueBasedGenerator.GenerateUnique(CancellationToken cancellationToken) => GenerateUnique(21, cancellationToken);

	/// <inheritdoc/>
	Grid IPrimaryGenerator.GeneratePrimary(CancellationToken cancellationToken) => GenerateUnique(21, cancellationToken);
}
