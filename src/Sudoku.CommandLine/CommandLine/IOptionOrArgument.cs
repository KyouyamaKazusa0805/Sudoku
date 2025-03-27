namespace Sudoku.CommandLine;

/// <summary>
/// Represents an argument or option.
/// </summary>
/// <typeparam name="T">The type of the result parsed from the argument or option.</typeparam>
public interface IOptionOrArgument<out T>
{
	/// <summary>
	/// <para>
	/// To parse the argument via <see cref="ArgumentResult"/> instance;
	/// use <see cref="SymbolResult.ErrorMessage"/> to report invalid values.
	/// </para>
	/// <para>
	/// <inheritdoc cref="ICommand.HandleCore" path="/summary/para[2]"/>
	/// </para>
	/// </summary>
	/// <param name="result">The result.</param>
	/// <returns>The result instance parsed.</returns>
	protected static abstract T ParseArgument(ArgumentResult result);
}
