namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Represents analyze command.
/// </summary>
public sealed class AnalyzeCommand : Command, ICommand
{
	/// <summary>
	/// Initializes an <see cref="AnalyzeCommand"/> instance.
	/// </summary>
	public AnalyzeCommand() : base("analyze", "Analyzes the specified puzzle")
	{
		OptionsCore = [new GridOption()];
		this.AddRange(OptionsCore);
		this.SetHandler(HandleCore);
	}


	/// <inheritdoc/>
	public SymbolList<Option> OptionsCore { get; }

	/// <inheritdoc/>
	public SymbolList<Argument> ArgumentsCore => [];


	/// <inheritdoc/>
	public void HandleCore(InvocationContext context)
	{
		var result = context.ParseResult;
		var grid = result.GetValueForOption((Option<Grid>)OptionsCore[0]);
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
