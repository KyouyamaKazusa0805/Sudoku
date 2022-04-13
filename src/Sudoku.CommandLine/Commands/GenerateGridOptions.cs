namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Introduces the options that are used for generating a sudoku grid.
/// </summary>
[Verb("generate", HelpText = "To generate a sudoku puzzle.")]
public sealed class GenerateGridOptions
{
	/// <summary>
	/// Indicates whether the generator will use hard-puzzle pattern to generate puzzles.
	/// The puzzle may be hard than the normal cases.
	/// </summary>
	/// <!--Today the value may be useless because other kinds of generators hasn't implemented.-->
	[Option('h', "hard-pattern", HelpText = "Generates the puzzle with the hard pattern.", Default = false)]
	public bool WithHardPattern { get; set; }

	/// <summary>
	/// Indicates the range pattern that the number of givens in the puzzle generated should contain.
	/// </summary>
	[Option('c', "count", HelpText = "Indicates the puzzle generated will contain the specified range of number of given digits.", Default = "..30")]
	public string Range { get; set; } = "..30";
}
