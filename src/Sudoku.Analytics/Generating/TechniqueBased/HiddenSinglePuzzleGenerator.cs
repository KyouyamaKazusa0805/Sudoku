namespace Sudoku.Generating.TechniqueBased;

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
	public override JustOneCellPuzzle GenerateJustOneCell(CancellationToken cancellationToken = default)
		=> new() { Result = GeneratingResult.NotSupported };

	/// <inheritdoc/>
	public override FullPuzzle GenerateUnique(CancellationToken cancellationToken = default)
		=> new() { Result = GeneratingResult.Unnecessary };
}
