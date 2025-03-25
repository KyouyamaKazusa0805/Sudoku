namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Represents a solve command.
/// </summary>
public sealed class SolveCommand : Command, ICommand
{
	/// <summary>
	/// Initializes a <see cref="SolveCommand"/> instance.
	/// </summary>
	public SolveCommand() : base("solve", "To solve a puzzle")
	{
		OptionsCore = [new GridOption(), new SolvingMethodOption()];
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
		var type = result.GetValueForOption((Option<SolverType>)OptionsCore[1]);

		CommonPreprocessors.OutputIfPuzzleNotUnique(grid, type.Create(), out var solution);
		if (!solution.IsUndefined)
		{
			Console.WriteLine(solution);
		}
	}
}
