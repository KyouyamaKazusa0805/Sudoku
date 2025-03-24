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
		var options = OptionsCore;
		this.AddRange(options);
		this.SetHandler(HandleCore, (Option<Grid>)options[0], (Option<SolverType>)options[1]);
	}


	/// <inheritdoc/>
	public ReadOnlySpan<Option> OptionsCore => (Option[])[new GridOption(), new SolvingMethodOption()];

	/// <inheritdoc/>
	public ReadOnlySpan<Argument> ArgumentsCore => [];


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
