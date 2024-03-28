namespace Sudoku.Generating.TechniqueBased;

/// <summary>
/// Represents a puzzle generator that can create puzzles using naked singles.
/// </summary>
public sealed class NakedSinglePuzzleGenerator : SinglePuzzleGenerator
{
	/// <inheritdoc/>
	public override TechniqueSet SupportedTechniques => [Technique.NakedSingle];


	/// <inheritdoc/>
	public override JustOneCellPuzzle GenerateJustOneCell(CancellationToken cancellationToken = default)
		=> new() { Result = GeneratingResult.NotSupported };

	/// <inheritdoc/>
	public override FullPuzzle GenerateUnique(CancellationToken cancellationToken = default)
		=> new() { Result = GeneratingResult.Unnecessary };
}
