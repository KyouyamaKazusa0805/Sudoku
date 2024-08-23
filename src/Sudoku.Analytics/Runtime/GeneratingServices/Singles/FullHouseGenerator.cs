namespace Sudoku.Runtime.GeneratingServices;

/// <summary>
/// Represents a generator that supports generating for puzzles that can be solved by only using Full Houses.
/// </summary>
/// <seealso cref="Technique.FullHouse"/>
public sealed class FullHouseGenerator : SingleGenerator
{
	/// <summary>
	/// Indicates the number of empty cells the current generator will generate on puzzles.
	/// </summary>
	public Cell EmptyCellsCount { get; set; }

	/// <inheritdoc/>
	public override TechniqueSet SupportedTechniques => [Technique.FullHouse];


	/// <inheritdoc/>
	public override bool TryGenerateUnique(out Grid result, CancellationToken cancellationToken = default)
	{
		var emptyCellsCount = EmptyCellsCount;
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
				result = Grid.Undefined;
				return false;
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
				&& Analyzer.Analyze(new AnalyzerContext(in grid) { CancellationToken = cancellationToken }) is { IsSolved: true, InterimSteps: var steps }
				&& new SortedSet<Technique>(from step in steps select step.Code).Max == Technique.FullHouse)
			{
				result = grid.FixedGrid;
				return true;
			}

			cancellationToken.ThrowIfCancellationRequested();
		}
	}

	/// <inheritdoc/>
	public override bool GeneratePrimary(out Grid result, CancellationToken cancellationToken)
		=> TryGenerateUnique(out result, cancellationToken);

	/// <inheritdoc/>
	public override bool TryGenerateJustOneCell(out Grid result, [NotNullWhen(true)] out Step? step, CancellationToken cancellationToken = default)
	{
		// Choose the target house.
		var selectedHouse = RandomlySelectHouse(Alignment);

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
		var (targetCell, targetDigit) = (Alignment, selectedHouse) switch
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
		result = puzzle.FixedGrid;
		return true;
	}

	/// <inheritdoc/>
	public override bool TryGenerateJustOneCell(out Grid result, out Grid phasedGrid, [NotNullWhen(true)] out Step? step, CancellationToken cancellationToken = default)
	{
		var generator = new Generator();
		while (true)
		{
			var puzzle = generator.Generate(cancellationToken: cancellationToken);
			switch (Analyzer.Analyze(new AnalyzerContext(in puzzle) { CancellationToken = cancellationToken }))
			{
				case { FailedReason: FailedReason.UserCancelled }:
				{
					(result, phasedGrid, step) = (Grid.Undefined, Grid.Undefined, null);
					return false;
				}
				case { IsSolved: true, InterimGrids: var interimGrids, InterimSteps: var interimSteps }:
				{
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

						(result, phasedGrid, step) = (extractedGrid.FixedGrid, currentGrid, s);
						return true;
					}
					break;
				}
				default:
				{
					cancellationToken.ThrowIfCancellationRequested();
					break;
				}
			}
		}
	}
}
