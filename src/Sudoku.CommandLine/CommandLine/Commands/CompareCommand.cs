namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Represents compare command.
/// </summary>
internal sealed class CompareCommand : Command, ICommand
{
	/// <summary>
	/// Initializes a <see cref="CompareCommand"/> instance.
	/// </summary>
	public CompareCommand() : base("compare", "Compare two grids and determine the equality result")
	{
		OptionsCore = [new ComparingMethodOption()];
		this.AddRange(OptionsCore);

		ArgumentsCore = [new TwoGridArgument()];
		this.AddRange(ArgumentsCore);

		this.SetHandler(HandleCore);
	}


	/// <inheritdoc/>
	public SymbolList<Option> OptionsCore { get; }

	/// <inheritdoc/>
	public SymbolList<Argument> ArgumentsCore { get; }

	/// <inheritdoc/>
	Command? ICommand.Parent { get; init; }


	/// <inheritdoc/>
	public void HandleCore(InvocationContext context)
	{
		if (this is not { ArgumentsCore: [TwoGridArgument a1], OptionsCore: [ComparingMethodOption o1] })
		{
			return;
		}

		var result = context.ParseResult;
		var (left, right) = result.GetValueForArgument(a1);
		var comparison = result.GetValueForOption(o1);
		var comparisonResult = left.Equals(right, comparison);
		Console.WriteLine(comparisonResult);
	}
}
