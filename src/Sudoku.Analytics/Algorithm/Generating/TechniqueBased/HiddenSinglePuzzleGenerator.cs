namespace Sudoku.Algorithm.Generating.TechniqueBased;

/// <summary>
/// Represents a puzzle generator that uses hidden single.
/// </summary>
public sealed class HiddenSinglePuzzleGenerator : SinglePuzzleGenerator
{
	/// <inheritdoc/>
	public override TechniqueSet SupportedTechniques
		=> [
			Technique.HiddenSingleBlock, Technique.HiddenSingleRow, Technique.HiddenSingleColumn,
			Technique.CrosshatchingBlock, Technique.CrosshatchingRow, Technique.CrosshatchingColumn
		];


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
