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
		this.SetHandler(HandleCore, (Option<Grid>)OptionsCore[0]);
	}


	/// <inheritdoc/>
	public SymbolList<Option> OptionsCore { get; }

	/// <inheritdoc/>
	public SymbolList<Argument> ArgumentsCore => [];


	/// <inheritdoc/>
	void ICommand.HandleCore(__arglist)
	{
		var iterator = new ArgIterator(__arglist);
		var grid = __refvalue(iterator.GetNextArg(), Grid);
		HandleCore(grid);
	}

	/// <inheritdoc cref="ICommand.HandleCore"/>
	private void HandleCore(Grid grid)
	{
		CommonPreprocessors.OutputIfPuzzleNotUnique(grid, new BitwiseSolver(), out var solution);
		if (solution.IsUndefined)
		{
			return;
		}

		var analyzer = new Analyzer();
		var result = analyzer.Analyze(grid);
		Console.WriteLine(result.ToString());
	}
}
