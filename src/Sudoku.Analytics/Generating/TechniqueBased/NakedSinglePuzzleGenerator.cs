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
			GridAlignment.NotLimited => Rng.Next(0, 81),
			GridAlignment.CenterHouses => (PeersMap[40] + 40)[Rng.Next(0, 21)],
			GridAlignment.CenterBlock => HousesMap[4][Rng.Next(0, 9)],
			_ => 40
		};
		var peerCells = PeersMap[targetCell].ToArray()[..];
		ShuffleSequence(peerCells);
		ShuffleSequence(DigitSeed);

		// Update values and create a grid.
		var puzzle = Grid.Empty;
		var i = 0;
		var digitsMask = (Mask)511;
		foreach (var cell in peerCells[..8])
		{
			var digit = DigitSeed[i++];
			puzzle.SetDigit(cell, digit);
			puzzle.SetState(cell, CellState.Given);

			digitsMask &= (Mask)~(1 << digit);
		}

		var targetDigit = Log2((uint)digitsMask);
		var blockCellsCount = (HousesMap[targetCell.ToHouseIndex(HouseType.Block)] - puzzle.EmptyCells).Count;
		return new JustOneCellPuzzleSuccessful(
			in puzzle,
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
					0 => SingleTechniqueSubtype.NakedSingle0,
					1 => SingleTechniqueSubtype.NakedSingle1,
					2 => SingleTechniqueSubtype.NakedSingle2,
					3 => SingleTechniqueSubtype.NakedSingle3,
					4 => SingleTechniqueSubtype.NakedSingle4,
					5 => SingleTechniqueSubtype.NakedSingle5,
					6 => SingleTechniqueSubtype.NakedSingle6,
					7 => SingleTechniqueSubtype.NakedSingle7,
					8 => SingleTechniqueSubtype.NakedSingle8
				}
			)
		);
	}

	/// <inheritdoc/>
	public override PhasedJustOneCellPuzzle GenerateJustOneCellPhased(SingleTechniqueSubtype? subtype = null, CancellationToken cancellationToken = default)
	{
		return null!;
	}
}
