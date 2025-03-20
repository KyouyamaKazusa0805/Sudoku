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
		this.SetHandler(HandleCore, (Option<Grid>)options[0], (Option<ISolver>)options[1]);
	}


	/// <inheritdoc/>
	public static ReadOnlySpan<Option> OptionsCore => (Option[])[new GridOption(), new SolvingMethodOption()];


	/// <inheritdoc/>
	static void ICommand.HandleCore(__arglist)
	{
		var iterator = new ArgIterator(__arglist);
		var grid = __refvalue(iterator.GetNextArg(), Grid);
		var solver = __refvalue(iterator.GetNextArg(), ISolver);
		HandleCore(grid, solver);
	}

	/// <inheritdoc cref="ICommand.HandleCore"/>
	private static void HandleCore(Grid grid, ISolver solver)
	{
		var result = solver.Solve(grid, out var solution);
		if (result is true)
		{
			Console.WriteLine(solution);
			return;
		}

		var text = result is false ? "The puzzle has multiple solutions." : "The puzzle has no valid solutions.";
		Console.WriteLine($"\e[31m{text}\e[0m");
	}
}
