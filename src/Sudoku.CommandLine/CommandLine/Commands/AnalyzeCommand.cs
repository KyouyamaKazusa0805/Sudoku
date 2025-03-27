namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Represents analyze command.
/// </summary>
internal sealed class AnalyzeCommand : Command, ICommand
{
	/// <summary>
	/// Initializes an <see cref="AnalyzeCommand"/> instance.
	/// </summary>
	public AnalyzeCommand() : base("analyze", "Analyzes the specified puzzle")
	{
		ArgumentsCore = [new GridArgument()];
		this.AddRange(ArgumentsCore);

		this.SetHandler(HandleCore);
	}


	/// <inheritdoc/>
	public SymbolList<Option> OptionsCore => [];

	/// <inheritdoc/>
	public SymbolList<Argument> ArgumentsCore { get; }

	/// <inheritdoc/>
	Command? ICommand.Parent { get; init; }


	/// <inheritdoc/>
	public void HandleCore(InvocationContext context)
	{
		if (this is not (_, [GridArgument a1]))
		{
			return;
		}

		var result = context.ParseResult;
		var grid = result.GetValueForArgument(a1);
		CommonPreprocessors.OutputIfPuzzleNotUnique(grid, new BitwiseSolver(), out var solution);
		if (solution.IsUndefined)
		{
			return;
		}

		var analyzer = new Analyzer();
		var r = analyzer.Analyze(grid);
		Console.WriteLine(r.ToString());
	}
}
