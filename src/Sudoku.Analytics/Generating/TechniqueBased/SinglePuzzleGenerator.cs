namespace Sudoku.Generating.TechniqueBased;

/// <summary>
/// Represents a puzzle generator that generates for puzzles using single techniques.
/// </summary>
public abstract class SinglePuzzleGenerator : TechniqueBasedPuzzleGenerator
{
	/// <summary>
	/// Indicates the generator only generates for puzzles aiming to the center houses, i.e. the row 5, column 5 and block 5.
	/// </summary>
	public bool OnlyCenterHouses { get; set; }

	/// <summary>
	/// Indicates whether the generator will also create for interferer digits.
	/// </summary>
	public bool HasInterfererDigits { get; set; }

	/// <inheritdoc cref="GridAlignment"/>
	public GridAlignment Alignment { get; set; }

	/// <inheritdoc/>
	public override SudokuType SupportedTypes => SudokuType.JustOneCell;
}
