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
		var arguments = ArgumentsCore;
		var options = OptionsCore;
		this.AddRange(arguments);
		this.AddRange(options);
		this.SetHandler(HandleCore, (Argument<(Grid Left, Grid Right)>)arguments[0], (Option<BoardComparison>)options[0]);
	}


	/// <inheritdoc/>
	public static ReadOnlySpan<Option> OptionsCore => (Option[])[new ComparingMethodOption()];

	/// <inheritdoc/>
	public static ReadOnlySpan<Argument> ArgumentsCore => (Argument[])[new TwoGridArgument()];


	/// <inheritdoc/>
	static void ICommand.HandleCore(__arglist)
	{
		var iterator = new ArgIterator(__arglist);
		var grids = __refvalue(iterator.GetNextArg(), (Grid, Grid));
		var comparison = __refvalue(iterator.GetNextArg(), BoardComparison);
		HandleCore(grids, comparison);
	}

	/// <inheritdoc cref="ICommand.HandleCore"/>
	private static void HandleCore((Grid Left, Grid Right) grids, BoardComparison comparison)
	{
		var result = grids.Left.Equals(grids.Right, comparison);
		Console.WriteLine(result);
	}
}
