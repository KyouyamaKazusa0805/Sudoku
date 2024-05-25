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
			(ConlusionCellAlignment.NotLimited or ConlusionCellAlignment.CenterHouse, _) when Rng.NextDigit() is var missingPos
				=> (HousesCells[selectedHouse][missingPos], DigitSeed[missingPos]),
			(ConlusionCellAlignment.CenterBlock, 4) when Rng.NextDigit() is var missingPos
				=> (HousesCells[selectedHouse][missingPos], DigitSeed[missingPos]),
			(ConlusionCellAlignment.CenterBlock, _) when (HousesMap[selectedHouse] & HousesMap[4]) is var a && a[Rng.Next(0, a.Count)] is var t
				=> (t, puzzle.GetDigit(t)),
			(ConlusionCellAlignment.CenterCell, _) => (40, puzzle.GetDigit(40)),
			_ => (-1, -1)
		};
		if ((targetCell, targetDigit) == (-1, -1))
		{
			return new JustOneCellPuzzleFailed(GeneratingFailedReason.InvalidData);
		}

		// Leave the target cell to be empty.
		puzzle.SetDigit(targetCell, -1);

		// Append interfering digits if worth.
		AppendInterferingDigitsNoBaseGrid(ref puzzle, targetCell, out var interferingCells);

		return new JustOneCellPuzzleSuccessful(
			puzzle.FixedGrid,
			targetCell,
			targetDigit,
			new FullHouseStep(
				null!,
				null,
				null!,
				selectedHouse,
				targetCell,
				targetDigit,
				SingleModule.GetLasting(in puzzle, targetCell, selectedHouse)
			),
			in interferingCells,
			InterferingPercentage
		);
	}

	/// <inheritdoc/>
	public override PhasedJustOneCellPuzzle GenerateJustOneCellPhased(SingleSubtype subtype = SingleSubtype.None, CancellationToken cancellationToken = default)
	{
		try
		{
			return Enum.IsDefined(subtype) && subtype != SingleSubtype.Unknown
				? g(subtype, cancellationToken)
				: new PhasedJustOneCellPuzzleFailed(GeneratingFailedReason.InvalidData);
		}
		catch (OperationCanceledException)
		{
			return new PhasedJustOneCellPuzzleFailed(GeneratingFailedReason.Canceled);
		}


		PhasedJustOneCellPuzzle g(SingleSubtype subtype, CancellationToken cancellationToken)
		{
			while (true)
			{
				var puzzle = new Generator().Generate(cancellationToken: cancellationToken);
				if (SingleAnalyzer.Analyze(in puzzle, cancellationToken: cancellationToken) is
					{
						IsSolved: true,
						InterimGrids: var interimGrids,
						InterimSteps: var interimSteps
					})
				{
					foreach (var (currentGrid, step) in StepMarshal.Combine(interimGrids, interimSteps))
					{
						if (step is not FullHouseStep { Cell: var cell, Digit: var digit, House: var house, Subtype: var currentSubtype })
						{
							continue;
						}

						if (subtype != SingleSubtype.None && subtype != currentSubtype)
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

						// Append interfering digits.
						AppendInterferingDigitsBaseGrid(ref extractedGrid, in currentGrid, cell, in HousesMap[house], out var interferingCells);

						// Found. Now return the value.
						return new PhasedJustOneCellPuzzleSuccessful(
							extractedGrid.FixedGrid,
							in currentGrid,
							cell,
							digit,
							step,
							in interferingCells,
							InterferingPercentage
						);
					}
				}

				cancellationToken.ThrowIfCancellationRequested();
			}
		}
	}

	/// <inheritdoc/>
	public override FullPuzzle GenerateUnique(CancellationToken cancellationToken = default)
	{
		if (EmptyCellsCount is not (-1 or >= 1 and <= PeersCount + 1))
		{
			EmptyCellsCount = Math.Clamp(EmptyCellsCount, 1, PeersCount + 1);
		}

		for (var i = 1; ; i++)
		{
			// Try generating a solution.
			var grid = new Generator().Generate(cancellationToken: cancellationToken);
			if (grid.IsUndefined)
			{
				return new FullPuzzleFailed(GeneratingFailedReason.Canceled);
			}

			// Replace with solution grid.
			grid = grid.SolutionGrid.UnfixedGrid;

			// Then randomly removed some digits in some cells, and keeps the grid valid.
			ShuffleSequence(CellSeed);

			// Removes the selected cells.
			var pattern = CellSeed[..(EmptyCellsCount == -1 ? Rng.Next(1, 22) : EmptyCellsCount)].AsCellMap();
			foreach (var cell in pattern)
			{
				grid.SetDigit(cell, -1);
			}

			// Fix the grid and check validity.
			grid.Fix();
			if (grid.IsValid
				&& Analyzer.Analyze(in grid, cancellationToken: cancellationToken) is { IsSolved: true, InterimSteps: var steps }
				&& new SortedSet<Technique>(from step in steps select step.Code).Max == Technique.FullHouse)
			{
				// Check validity of the puzzle.
				return new FullPuzzleSuccessful(grid.FixedGrid);
			}

			cancellationToken.ThrowIfCancellationRequested();
		}
	}
}
