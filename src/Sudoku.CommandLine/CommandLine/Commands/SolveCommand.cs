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
		this.SetHandler(HandleCore, (Option<Grid>)OptionsCore[0], (Option<SolverType>)OptionsCore[1]);
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
		var type = __refvalue(iterator.GetNextArg(), SolverType);
		HandleCore(grid, type);
	}

	/// <inheritdoc cref="ICommand.HandleCore"/>
	private void HandleCore(Grid grid, SolverType type)
	{
		CommonPreprocessors.OutputIfPuzzleNotUnique(grid, type.Create(), out var solution);
		if (!solution.IsUndefined)
		{
			Console.WriteLine(solution);
		}
	}
}
