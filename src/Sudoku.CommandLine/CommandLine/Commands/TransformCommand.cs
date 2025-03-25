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
		this.SetHandler(HandleCore, (Option<Grid>)OptionsCore[0], (Option<TransformType>)OptionsCore[1]);
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
		var types = __refvalue(iterator.GetNextArg(), TransformType);
		HandleCore(grid, types);
	}

	/// <inheritdoc cref="ICommand.HandleCore"/>
	private void HandleCore(Grid grid, TransformType types)
	{
		grid.Transform(types);
		Console.WriteLine(grid.ToString("."));
	}
}
