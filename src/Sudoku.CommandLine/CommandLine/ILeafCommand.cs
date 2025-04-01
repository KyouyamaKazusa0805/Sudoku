namespace Sudoku.CommandLine;

/// <summary>
/// Represents a command that is the leaf node that cannot contain any subcommands.
/// </summary>
public interface ILeafCommand
{
	/// <summary>
	/// Indicates the options.
	/// </summary>
	public abstract SymbolList<Option> OptionsCore { get; }

	/// <summary>
	/// Indicates the arguments.
	/// </summary>
	public abstract SymbolList<Argument> ArgumentsCore { get; }

	/// <summary>
	/// Indicates the parent command. The value can be <see langword="null"/> if the command has no parent.
	/// </summary>
	public abstract ICommand? Parent { get; init; }


	/// <summary>
	/// <para>The backing handler method.</para>
	/// <para>
	/// This method only provides a constraint on implementation for multiple arguments with different types.
	/// <b>Do not consume this method or expose it outside.</b>
	/// </para>
	/// </summary>
	/// <param name="context">
	/// The context. Use property <see cref="InvocationContext.ParseResult"/> to retrieve the target option result.
	/// </param>
	/// <seealso cref="InvocationContext.ParseResult"/>
	public abstract void HandleCore(InvocationContext context);
}
