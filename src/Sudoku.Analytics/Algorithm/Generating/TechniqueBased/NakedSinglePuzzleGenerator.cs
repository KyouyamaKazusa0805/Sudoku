namespace Sudoku.Algorithm.Generating.TechniqueBased;

/// <summary>
/// Represents a puzzle generator that can create puzzles using naked singles.
/// </summary>
public sealed class NakedSinglePuzzleGenerator : SinglePuzzleGenerator
{
	/// <inheritdoc/>
	public override TechniqueSet SupportedTechniques => [Technique.NakedSingle];


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
