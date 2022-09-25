namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// Defines a list of project names to be used.
/// </summary>
internal static class Projects
{
	/// <summary>
	/// Indicates the system extensions project.
	/// </summary>
	public const string System = "SystemExtensions";

	/// <summary>
	/// Indicates the sudoku core project.
	/// </summary>
	[Obsolete("The field is now deprecated.")]
	public const string SudokuCore = "Sudoku.Core";

	/// <summary>
	/// Indicates the solving logical project.
	/// </summary>
	[Obsolete("The field is now deprecated.")]
	public const string SolvingLogical = "Sudoku.Solving.Logical";
}
