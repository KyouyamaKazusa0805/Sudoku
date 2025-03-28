namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Represents a minlex command.
/// </summary>
internal sealed class MinlexCommand : Command, ICommand
{
	/// <summary>
	/// Initializes a <see cref="MinlexCommand"/> instance.
	/// </summary>
	public MinlexCommand() : base("minlex", "To find a minlex (minimal lexicographical) grid of the specified grid")
	{
		ArgumentsCore = [new GridArgument()];
		this.AddRange(ArgumentsCore);

		this.SetHandler(HandleCore);
	}


	/// <inheritdoc/>
	public SymbolList<Option> OptionsCore => [];

	/// <inheritdoc/>
	public SymbolList<Argument> ArgumentsCore { get; }

	/// <inheritdoc/>
	INonLeafCommand? ICommand.Parent { get; init; }


	/// <inheritdoc/>
	public void HandleCore(InvocationContext context)
	{
		if (this is not (_, [GridArgument a1]))
		{
			return;
		}

		var result = context.ParseResult;
		var grid = result.GetValueForArgument(a1);
		CommonPreprocessors.PrintInvalidIfWorth(grid, new BitwiseSolver(), out var solution);
		if (!solution.IsUndefined)
		{
			var minlexGrid = grid.GetMinLexGrid();
			Console.WriteLine(minlexGrid.ToString("."));
		}
	}
}
