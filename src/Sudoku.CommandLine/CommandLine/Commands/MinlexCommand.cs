namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Represents a minlex command.
/// </summary>
public sealed class MinlexCommand : Command, ICommand
{
	/// <summary>
	/// Initializes a <see cref="MinlexCommand"/> instance.
	/// </summary>
	public MinlexCommand() : base("minlex", "To find a minlex (minimum lexicographical) grid of the specified grid")
	{
		var options = OptionsCore;
		this.AddRange(options);
		this.SetHandler(HandleCore, (Option<Grid>)options[0]);
	}


	/// <inheritdoc/>
	public static ReadOnlySpan<Option> OptionsCore => (Option[])[new GridOption()];


	/// <inheritdoc/>
	static void ICommand.HandleCore(__arglist)
	{
		var iterator = new ArgIterator(__arglist);
		var grid = __refvalue(iterator.GetNextArg(), Grid);
		HandleCore(grid);
	}

	/// <inheritdoc cref="ICommand.HandleCore"/>
	private static void HandleCore(Grid grid)
	{
		var solver = new BitwiseSolver();
		var result = solver.Solve(grid, out _);
		if (result is not true)
		{
			var text = result is false ? "The puzzle has multiple solutions." : "The puzzle has no valid solutions.";
			Console.WriteLine($"\e[31m{text}\e[0m");
			return;
		}

		var minlexGrid = grid.GetMinLexGrid();
		Console.WriteLine(minlexGrid.ToString("."));
	}
}
