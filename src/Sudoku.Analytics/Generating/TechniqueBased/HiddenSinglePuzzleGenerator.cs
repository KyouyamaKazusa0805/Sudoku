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
		=> new(GeneratingResult.NotSupported);

	/// <inheritdoc/>
	public override GeneratingResult GenerateUnique(out Grid result, CancellationToken cancellationToken = default)
	{
		result = Grid.Undefined;
		return GeneratingResult.Unnecessary;
	}
}
