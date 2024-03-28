namespace Sudoku.Generating.TechniqueBased;

/// <summary>
/// Represents a puzzle generator that can create puzzles using naked singles.
/// </summary>
public sealed class NakedSinglePuzzleGenerator : SinglePuzzleGenerator
{
	/// <inheritdoc/>
	public override TechniqueSet SupportedTechniques => [Technique.NakedSingle];


	/// <inheritdoc/>
	public override JustOneCellPuzzle GenerateJustOneCell()
	{
		// Randomly select a cell as target cell.
		var targetCell = Rng.Next(0, 81);
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
			new NakedSingleStep([], null, new(), targetCell, targetDigit, SingleTechniqueSubtype.NakedSingle0 + blockCellsCount)
		);
	}

	/// <inheritdoc/>
	public override FullPuzzle GenerateUnique(CancellationToken cancellationToken = default)
		=> new FullPuzzleFailed(GeneratingFailedReason.Unnecessary);
}
