namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Represents a command.
/// </summary>
internal interface ICommand
{
	/// <summary>
	/// Indicates the options.
	/// </summary>
	public static abstract ReadOnlySpan<Option> OptionsCore { get; }

	/// <summary>
	/// Indicates the arguments.
	/// </summary>
	public static abstract ReadOnlySpan<Argument> ArgumentsCore { get; }


	/// <summary>
	/// <para>The backing handler method.</para>
	/// <para>
	/// This method only provides a constraint on implementation for multiple arguments with different types.
	/// <b>Do not consume this method or expose it outside.</b>
	/// </para>
	/// </summary>
	protected static abstract void HandleCore(__arglist);
}
