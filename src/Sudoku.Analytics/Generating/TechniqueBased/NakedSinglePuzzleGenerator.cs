namespace Sudoku.Generating.TechniqueBased;

/// <summary>
/// Represents a puzzle generator that can create puzzles using naked singles.
/// </summary>
public sealed class NakedSinglePuzzleGenerator : SinglePuzzleGenerator<NakedSingleStep>
{
	/// <inheritdoc/>
	public override TechniqueSet SupportedTechniques => [Technique.NakedSingle];


	/// <inheritdoc/>
	public override JustOneCellPuzzle GenerateJustOneCell()
	{
		// Randomly select a cell as target cell.
		var targetCell = Alignment switch
		{
			ConlusionCellAlignment.NotLimited => Rng.Next(0, 81),
			ConlusionCellAlignment.CenterHouse => (PeersMap[40] + 40)[Rng.Next(0, 21)],
			ConlusionCellAlignment.CenterBlock => HousesMap[4][Rng.Next(0, 9)],
			_ => 40
		};
		var peerCells = PeersMap[targetCell].ToArray()[..];

		// Generate extra digits.
		// Different with other techniques, naked single can be failed to be generated - it'll produce a puzzle that has no solution.
		// We should check for this. If so, we should generate puzzle again.
		Grid puzzle;
		Mask digitsMask;
		CellMap interferingCells;
		do
		{
			ShuffleSequence(peerCells);
			ShuffleSequence(DigitSeed);

			// Update values and create a grid.
			(puzzle, digitsMask, var i) = (Grid.Empty, Grid.MaxCandidatesMask, 0);
			foreach (var cell in peerCells[..8])
			{
				var digit = DigitSeed[i++];
				puzzle.SetDigit(cell, digit);

				digitsMask &= (Mask)~(1 << digit);
			}
		} while (AppendInterferingDigitsNoBaseGrid(ref puzzle, targetCell, out interferingCells) == GeneratingFailedReason.InvalidData);

		var targetDigit = Log2((uint)digitsMask);
		var blockCellsCount = (HousesMap[targetCell.ToHouseIndex(HouseType.Block)] - puzzle.EmptyCells).Count;
		return new JustOneCellPuzzleSuccessful(
			puzzle.FixedGrid,
			targetCell,
			targetDigit,
			new NakedSingleStep(
				null!,
				null,
				null!,
				targetCell,
				targetDigit,
				blockCellsCount switch
				{
					0 => SingleSubtype.NakedSingle0,
					1 => SingleSubtype.NakedSingle1,
					2 => SingleSubtype.NakedSingle2,
					3 => SingleSubtype.NakedSingle3,
					4 => SingleSubtype.NakedSingle4,
					5 => SingleSubtype.NakedSingle5,
					6 => SingleSubtype.NakedSingle6,
					7 => SingleSubtype.NakedSingle7,
					8 => SingleSubtype.NakedSingle8
				}
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
				var puzzle = new HodokuPuzzleGenerator().Generate(cancellationToken: cancellationToken);
				if (SingleAnalyzer.Analyze(in puzzle, cancellationToken: cancellationToken) is { IsSolved: true, SolvingPath: var path })
				{
					foreach (var (currentGrid, step) in path)
					{
						if (step is not NakedSingleStep { Cell: var cell, Digit: var digit, Subtype: var currentSubtype })
						{
							continue;
						}

						if (subtype != SingleSubtype.None && subtype != currentSubtype)
						{
							continue;
						}

						var excluderCells = SingleModule.GetNakedSingleExcluderCells(in currentGrid, cell, digit, out _);
						var extractedGrid = currentGrid;
						extractedGrid.Unfix();

						for (var c = 0; c < 81; c++)
						{
							if (cell != c && !excluderCells.Contains(c))
							{
								extractedGrid.SetDigit(c, -1);
							}
						}

						// Append interfering digits.
						AppendInterferingDigitsBaseGrid(ref extractedGrid, in currentGrid, cell, out var interferingCells);

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
}
