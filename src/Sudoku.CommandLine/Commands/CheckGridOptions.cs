namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Introduces the options that are used for checking attributes for a sudoku grid.
/// </summary>
[Verb("check", HelpText = "To check the attributes for a sudoku grid.")]
public sealed class CheckGridOptions
{
	/// <summary>
	/// Indicates the grid value.
	/// </summary>
	[Value(0, HelpText = "Indicates the sudoku grid as string representation.", Required = true)]
	public string? GridValue { get; set; }

	/// <summary>
	/// Indicates the operation is for checking the validity of the grid.
	/// </summary>
	[Option('v', "validity", HelpText = "Indicates to check the validity for a sudoku grid.", SetName = "check-validity")]
	public bool ChecksForValidity { get; set; }
}
