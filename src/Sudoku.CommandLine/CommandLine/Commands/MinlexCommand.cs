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

		CommonPreprocessors.OutputIfPuzzleNotUnique(grid, new BitwiseSolver(), out var solution);
		if (!solution.IsUndefined)
		{
			var minlexGrid = grid.GetMinLexGrid();
			Console.WriteLine(minlexGrid.ToString("."));
		}
	}
}
