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
		this.SetHandler(HandleCore);
	}


	/// <inheritdoc/>
	public SymbolList<Option> OptionsCore { get; }

	/// <inheritdoc/>
	public SymbolList<Argument> ArgumentsCore { get; }


	/// <inheritdoc/>
	public void HandleCore(InvocationContext context)
	{
		var result = context.ParseResult;
		var (left, right) = result.GetValueForArgument((Argument<(Grid, Grid)>)ArgumentsCore[0]);
		var comparison = result.GetValueForOption((Option<BoardComparison>)OptionsCore[0]);
		var comparisonResult = left.Equals(right, comparison);
		Console.WriteLine(comparisonResult);
	}
}
