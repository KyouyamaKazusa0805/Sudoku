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
		var options = OptionsCore;
		this.AddRange(options);
		this.SetHandler(HandleCore, (Option<Grid>)options[0], (Option<TransformType>)options[1]);
	}


	/// <inheritdoc/>
	public static ReadOnlySpan<Option> OptionsCore => (Option[])[new GridOption(true), new TransformatingMethodOption()];

	/// <inheritdoc/>
	public static ReadOnlySpan<Argument> ArgumentsCore => [];


	/// <inheritdoc/>
	static void ICommand.HandleCore(__arglist)
	{
		var iterator = new ArgIterator(__arglist);
		var grid = __refvalue(iterator.GetNextArg(), Grid);
		var types = __refvalue(iterator.GetNextArg(), TransformType);
		HandleCore(grid, types);
	}

	/// <inheritdoc cref="ICommand.HandleCore"/>
	private static void HandleCore(Grid grid, TransformType types)
	{
		grid.Transform(types);
		Console.WriteLine(grid.ToString("."));
	}
}
