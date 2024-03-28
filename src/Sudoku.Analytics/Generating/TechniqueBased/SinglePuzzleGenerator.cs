namespace Sudoku.Generating.TechniqueBased;

/// <summary>
/// Represents a puzzle generator that generates for puzzles using single techniques.
/// </summary>
public abstract class SinglePuzzleGenerator : TechniqueBasedPuzzleGenerator
{
	/// <summary>
	/// Indicates whether the generator will also create for interferer digits.
	/// </summary>
	public bool HasInterfererDigits { get; set; }

	/// <inheritdoc/>
	public override SudokuType SupportedTypes => SudokuType.JustOneCell;
}
