namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Represents analyze command.
/// </summary>
internal sealed class AnalyzeCommand : CommandBase
{
	/// <summary>
	/// Initializes an <see cref="AnalyzeCommand"/> instance.
	/// </summary>
	public AnalyzeCommand() : base("analyze", "Analyzes the specified puzzle")
	{
		ArgumentsCore = [new GridArgument()];
		this.AddRange(ArgumentsCore);

		this.SetHandler(HandleCore);

		var commands = (SymbolList<Command>)[new FindCommand { Parent = this }];
		this.AddRange(commands);
	}


	/// <inheritdoc/>
	public override bool HasSubcommands => true;

	/// <inheritdoc/>
	public override SymbolList<Argument> ArgumentsCore { get; }


	/// <inheritdoc/>
	protected override void HandleCore(InvocationContext context)
	{
		if (this is not (_, [GridArgument a1]))
		{
			return;
		}

		var result = context.ParseResult;
		var grid = result.GetValueForArgument(a1);
		CommonPreprocessors.PrintInvalidIfWorth(grid, new BitwiseSolver(), out var solution);
		if (solution.IsUndefined)
		{
			return;
		}

		var analyzer = new Analyzer();
		var r = analyzer.Analyze(grid);
		Console.WriteLine(r.ToString());
	}
}
