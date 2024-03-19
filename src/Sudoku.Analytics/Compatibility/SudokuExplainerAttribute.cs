namespace Sudoku.Compatibility;

/// <summary>
/// Represents compatibility rules for Sudoku Explainer.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class SudokuExplainerAttribute : ProgramMetadataAttribute<double, SudokuExplainerTechnique>
{
	/// <summary>
	/// Indicates the defined technique enumeration field in Sudoku Explainer.
	/// </summary>
	public SudokuExplainerTechnique TechniqueDefined { get; init; }
}
