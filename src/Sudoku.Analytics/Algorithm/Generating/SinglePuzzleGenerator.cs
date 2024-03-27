namespace Sudoku.Algorithm.Generating;

/// <summary>
/// Represents a puzzle generator that generates for puzzles using single techniques.
/// </summary>
public abstract class SinglePuzzleGenerator : TechniqueBasedPuzzleGenerator
{
	/// <inheritdoc/>
	public override SudokuType SupportedTypes => SudokuType.JustOneCell;


	/// <inheritdoc/>
	public sealed override GenerationResult GenerateJustOneCell(out Grid result, CancellationToken cancellationToken = default)
		=> GenerateJustOneCell(false, out result, cancellationToken);

	/// <inheritdoc cref="GenerateJustOneCell(out Grid, CancellationToken)"/>
	/// <param name="interfering">Indicates whether the method will produce extra digits as interfering ones.</param>
	/// <param name="result"><inheritdoc/></param>
	/// <param name="cancellationToken"><inheritdoc/></param>
	public abstract GenerationResult GenerateJustOneCell(bool interfering, out Grid result, CancellationToken cancellationToken = default);
}
