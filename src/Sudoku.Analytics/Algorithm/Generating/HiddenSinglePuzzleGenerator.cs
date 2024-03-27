namespace Sudoku.Algorithm.Generating;

/// <summary>
/// Represents a puzzle generator that uses hidden single.
/// </summary>
public sealed class HiddenSinglePuzzleGenerator : TechniqueBasedPuzzleGenerator
{
	/// <inheritdoc/>
	public override SudokuType SupportedPuzzleTypes => SudokuType.JustOneCell;

	/// <inheritdoc/>
	public override TechniqueSet SupportedTechniques
		=> [Technique.HiddenSingleBlock, Technique.HiddenSingleRow, Technique.HiddenSingleColumn];


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
