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
		OptionsCore = [new GridOption(true)];
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
		if (!solution.IsUndefined)
		{
			var minlexGrid = grid.GetMinLexGrid();
			Console.WriteLine(minlexGrid.ToString("."));
		}
	}
}
