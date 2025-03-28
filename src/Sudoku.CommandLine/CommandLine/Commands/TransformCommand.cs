namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Represents transform command.
/// </summary>
internal sealed class TransformCommand : Command, ICommand
{
	/// <summary>
	/// Initializes a <see cref="TransformCommand"/> instance.
	/// </summary>
	public TransformCommand() : base("transform", "Transform a grid by the specified way")
	{
		OptionsCore = [new TransformatingMethodOption()];
		this.AddRange(OptionsCore);

		ArgumentsCore = [new GridArgument()];
		this.AddRange(ArgumentsCore);

		this.SetHandler(HandleCore);
	}


	/// <inheritdoc/>
	public SymbolList<Option> OptionsCore { get; }

	/// <inheritdoc/>
	public SymbolList<Argument> ArgumentsCore { get; }

	/// <inheritdoc/>
	INonLeafCommand? ICommand.Parent { get; init; }


	/// <inheritdoc/>
	public void HandleCore(InvocationContext context)
	{
		if (this is not ([TransformatingMethodOption o1], [GridArgument a1]))
		{
			return;
		}

		var result = context.ParseResult;
		var grid = result.GetValueForArgument(a1);
		var types = result.GetValueForOption(o1);
		grid.Transform(types);
		Console.WriteLine(grid.ToString("."));
	}
}
