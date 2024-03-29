namespace Sudoku.Generating.TechniqueBased;

/// <summary>
/// Represents a puzzle generator that generates for puzzles using single techniques.
/// </summary>
/// <typeparam name="TStep">The type of the step supported.</typeparam>
public abstract class SinglePuzzleGenerator<TStep> : TechniqueBasedPuzzleGenerator where TStep : SingleStep
{
	/// <summary>
	/// Indicates center houses.
	/// </summary>
	protected static readonly House[] CenterHouses = [4, 12, 13, 14, 21, 22, 23];

	/// <summary>
	/// Indicates center houses, strictly.
	/// </summary>
	protected static readonly House[] StrictCenterHouses = [4, 13, 22];


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
