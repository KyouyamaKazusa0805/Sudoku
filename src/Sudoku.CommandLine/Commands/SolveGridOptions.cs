namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Introduces the options that are used for getting the solution from a sudoku grid.
/// </summary>
[Verb("solve", HelpText = "To solve a sudoku grid, and get the solution grid.")]
public sealed class SolveGridOptions
{
	/// <summary>
	/// Indicates the grid value.
	/// </summary>
	[Option('g', "grid", HelpText = "Indicates the sudoku grid as string representation.", Required = true)]
	public string? GridValue { get; set; }
}
