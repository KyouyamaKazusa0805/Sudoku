namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Represents transform command.
/// </summary>
public sealed class TransformCommand : Command, ICommand
{
	/// <summary>
	/// Initializes a <see cref="TransformCommand"/> instance.
	/// </summary>
	public TransformCommand() : base("transform", "Transform a grid by the specified way")
	{
		OptionsCore = [new GridOption(true), new TransformatingMethodOption()];
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
		var types = result.GetValueForOption((Option<TransformType>)OptionsCore[1]);
		grid.Transform(types);
		Console.WriteLine(grid.ToString("."));
	}
}
