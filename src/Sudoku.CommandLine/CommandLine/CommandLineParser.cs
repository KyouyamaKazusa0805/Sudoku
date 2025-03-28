namespace Sudoku.CommandLine;

/// <summary>
/// Provides an entry that parses command-line arguments.
/// </summary>
public static class CommandLineParser
{
	/// <summary>
	/// Indicates the root command.
	/// </summary>
	private static readonly RootCommand RootCommand;


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static CommandLineParser()
	{
		RootCommand = [
			new AnalyzeCommand(),
			new CompareCommand(),
			new GenerateCommand(),
			new MinlexCommand(),
			new PrintCommand(),
			new SolveCommand(),
			new TransformCommand()
		];
		RootCommand.Description = "Sudoku Command Line";
	}


	/// <summary>
	/// Try to parse the command-line.
	/// </summary>
	/// <param name="args">The arguments.</param>
	/// <returns>A <see cref="Task"/> object that handles the operation.</returns>
	/// <remarks>
	/// In command line project, use the following code to parse command lines:
	/// <code>
	/// <see langword="await"/> <see cref="CommandLineParser"/>.ParseAsync(<see langword="args"/>);
	/// </code>
	/// </remarks>
	public static async Task ParseAsync(string[] args) => await RootCommand.InvokeAsync(args).ConfigureAwait(true);
}
