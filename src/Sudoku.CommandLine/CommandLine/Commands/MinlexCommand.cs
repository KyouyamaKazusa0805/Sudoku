namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Represents a minlex command.
/// </summary>
public sealed class MinlexCommand : Command, ICommand
{
	/// <summary>
	/// Initializes a <see cref="MinlexCommand"/> instance.
	/// </summary>
	public MinlexCommand() : base("minlex", "To find a minlex (minimal lexicographical) grid of the specified grid")
	{
		var options = OptionsCore;
		this.AddRange(options);
		this.SetHandler(HandleCore, (Option<Grid>)options[0]);
	}


	/// <inheritdoc/>
	public static ReadOnlySpan<Option> OptionsCore => (Option[])[new GridOption(true)];

	/// <inheritdoc/>
	public static ReadOnlySpan<Argument> ArgumentsCore => [];


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
		CommonPreprocessors.OutputIfPuzzleNotUnique(grid, new BitwiseSolver(), out var solution);
		if (!solution.IsUndefined)
		{
			var minlexGrid = grid.GetMinLexGrid();
			Console.WriteLine(minlexGrid.ToString("."));
		}
	}
}
