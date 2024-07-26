namespace Sudoku.Runtime.GeneratingServices;

/// <summary>
/// Represents a generator that supports generating for puzzles that can be solved by only using Naked Singles.
/// </summary>
/// <seealso cref="Technique.NakedSingle"/>
public sealed class NakedSinglePrimaryGenerator : PrimaryGenerator
{
	/// <inheritdoc/>
	public override TechniqueSet SupportedTechniques => [Technique.NakedSingle];


	/// <inheritdoc/>
	public override Grid GenerateUnique(CancellationToken cancellationToken = default)
	{
		var generator = new Generator();
		while (true)
		{
			var puzzle = generator.Generate(cancellationToken: cancellationToken);
			if (puzzle.IsUndefined)
			{
				return Grid.Undefined;
			}

			if (!puzzle.CanPrimaryNakedSingle())
			{
				cancellationToken.ThrowIfCancellationRequested();
				continue;
			}
			return puzzle;
		}
	}

	/// <inheritdoc/>
	public override Grid GeneratePrimary(CancellationToken cancellationToken) => GenerateUnique(cancellationToken);

	/// <inheritdoc/>
	public override Grid GenerateJustOneCell(out Step? step, CancellationToken cancellationToken = default)
	{
		// Randomly select a cell as target cell.
		var targetCell = Alignment switch
		{
			ConclusionCellAlignment.NotLimited => Rng.NextCell(),
			ConclusionCellAlignment.CenterHouse => (PeersMap[40] + 40)[Rng.Next(PeersCount + 1)],
			ConclusionCellAlignment.CenterBlock => HousesMap[4][Rng.NextDigit()],
			_ => 40
		};
		var peerCells = PeersMap[targetCell].ToArray()[..];

		// Generate extra digits.
		// Different with other techniques, naked single can be failed to be generated - it'll produce a puzzle that has no solution.
		// We should check for this. If so, we should generate puzzle again.
		ShuffleSequence(peerCells);
		ShuffleSequence(DigitSeed);

		// Update values and create a grid.
		var (puzzle, digitsMask, i) = (Grid.Empty, Grid.MaxCandidatesMask, 0);
		foreach (var cell in peerCells[..8])
		{
			var digit = DigitSeed[i++];
			puzzle.SetDigit(cell, digit);
			digitsMask &= (Mask)~(1 << digit);
		}

		var targetDigit = Log2((uint)digitsMask);
		step = new NakedSingleStep(
			null!,
			null,
			null!,
			targetCell,
			targetDigit,
			SingleModule.GetNakedSingleSubtype(in puzzle, targetCell),
			SingleModule.GetLastingAllHouses(in puzzle, targetCell, out _)
		);
		return puzzle.FixedGrid;
	}

	/// <inheritdoc/>
	public override Grid GenerateJustOneCell(out Grid phasedGrid, out Step? step, CancellationToken cancellationToken = default)
	{
		var generator = new Generator();
		while (true)
		{
			var puzzle = generator.Generate(cancellationToken: cancellationToken);
			switch (Analyzer.Analyze(in puzzle, cancellationToken: cancellationToken))
			{
				case { FailedReason: FailedReason.UserCancelled }:
				{
					(phasedGrid, step) = (Grid.Undefined, null);
					return Grid.Undefined;
				}
				case { IsSolved: true, InterimGrids: var interimGrids, InterimSteps: var interimSteps }:
				{
					foreach (var (currentGrid, s) in StepMarshal.Combine(interimGrids, interimSteps))
					{
						if (s is not NakedSingleStep { Cell: var cell, Digit: var digit })
						{
							continue;
						}

						var excluderCells = Excluder.GetNakedSingleExcluderCells(in currentGrid, cell, digit, out _);
						var extractedGrid = currentGrid;
						extractedGrid.Unfix();

						for (var c = 0; c < 81; c++)
						{
							if (cell != c && !excluderCells.Contains(c))
							{
								extractedGrid.SetDigit(c, -1);
							}
						}

						(phasedGrid, step) = (currentGrid, s);
						return extractedGrid.FixedGrid;
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
