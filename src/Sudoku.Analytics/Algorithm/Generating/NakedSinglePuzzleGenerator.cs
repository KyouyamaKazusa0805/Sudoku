namespace Sudoku.Algorithm.Generating;

/// <summary>
/// Represents a puzzle generator that can create puzzles using naked singles.
/// </summary>
public sealed class NakedSinglePuzzleGenerator : TechniqueBasedPuzzleGenerator
{
	/// <inheritdoc/>
	public override SudokuType SupportedPuzzleTypes => SudokuType.JustOneCell;

	/// <inheritdoc/>
	public override TechniqueSet SupportedTechniques => [Technique.NakedSingle];


	/// <inheritdoc/>
	public override GenerationResult GenerateJustOneCell(out Grid result, CancellationToken cancellationToken = default)
	{
		result = Grid.Undefined;
		return GenerationResult.NotSupported;
	}

	/// <inheritdoc/>
	public override GenerationResult GenerateUnique(out Grid result, CancellationToken cancellationToken = default)
	{
		result = Grid.Undefined;
		return GenerationResult.Unnecessary;
	}
}
