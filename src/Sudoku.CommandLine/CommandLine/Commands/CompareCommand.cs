namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Represents compare command.
/// </summary>
public sealed class CompareCommand : Command, ICommand
{
	/// <summary>
	/// Initializes a <see cref="CompareCommand"/> instance.
	/// </summary>
	public CompareCommand() : base("compare", "Compare two grids and determine the equality result")
	{
		OptionsCore = [new ComparingMethodOption()];
		ArgumentsCore = [new TwoGridArgument()];
		this.AddRange(OptionsCore);
		this.AddRange(ArgumentsCore);
		this.SetHandler(HandleCore, (Argument<(Grid Left, Grid Right)>)ArgumentsCore[0], (Option<BoardComparison>)OptionsCore[0]);
	}


	/// <inheritdoc/>
	public SymbolList<Option> OptionsCore { get; }

	/// <inheritdoc/>
	public SymbolList<Argument> ArgumentsCore { get; }


	/// <inheritdoc/>
	void ICommand.HandleCore(__arglist)
	{
		var iterator = new ArgIterator(__arglist);
		var grids = __refvalue(iterator.GetNextArg(), (Grid, Grid));
		var comparison = __refvalue(iterator.GetNextArg(), BoardComparison);
		HandleCore(grids, comparison);
	}

	/// <inheritdoc cref="ICommand.HandleCore"/>
	private void HandleCore((Grid Left, Grid Right) grids, BoardComparison comparison)
	{
		var result = grids.Left.Equals(grids.Right, comparison);
		Console.WriteLine(result);
	}
}
