#nullable disable

namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Introduces the options that are used for getting the solution from a sudoku grid.
/// </summary>
[Verb("solve", HelpText = "To solve a sudoku grid, and get the solution grid.")]
public sealed class SolveGridOptions : IRootCommand
{
	/// <summary>
	/// Indicates the supported method names.
	/// </summary>
	internal static readonly (string FullName, string ShortName, string Index, Type SolverType)[] MethodNames =
	{
		("bitwise", "b", "0", typeof(BitwiseSolver)),
		("linq", "l", "1", typeof(LinqSolver)),
		("backtracking", "t", "2", typeof(BacktrackingSolver)),
		("manual", "m", "3", typeof(ManualSolver))
	};


	/// <summary>
	/// Indicates the grid value.
	/// </summary>
	[Option('g', "grid", HelpText = "Indicates the sudoku grid as string representation.", Required = true)]
	public string GridValue { get; set; }

	/// <summary>
	/// Indicates the method used for solving the sudoku grid.
	/// </summary>
	[Option('m', "method", Default = "bitwise", HelpText = "Indicates the solve method used.")]
	public string Method { get; set; }


	/// <inheritdoc/>
	[Usage(ApplicationAlias = "Sudoku.CommandLine.exe")]
	public static IEnumerable<Example> Examples
	{
		get
		{
			yield return new(
				"A basic way to solve a sudoku puzzle.",
				new SolveGridOptions
				{
					GridValue = "...892.....2...3..75.....69.359.814...........713.659.96.....21..4...6.....621..."
				}
			);
			yield return new(
				"A basic way to solve a sudoku puzzle.",
				new SolveGridOptions
				{
					Method = "manual",
					GridValue = "...892.....2...3..75.....69.359.814...........713.659.96.....21..4...6.....621..."
				}
			);
		}
	}
}
