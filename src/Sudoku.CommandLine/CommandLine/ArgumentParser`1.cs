namespace Sudoku.CommandLine;

/// <inheritdoc/>
/// <typeparam name="T">The type of the argument to be recognized as target value.</typeparam>
public abstract class ArgumentParser<T> : ArgumentParser
{
	/// <summary>
	/// Indicates the result to be set.
	/// </summary>
	public T? Result { get; protected set; }


	/// <summary>
	/// Parses the command line with the specified range, and set the value to property <see cref="Result"/>
	/// if valid values are created. If failed, you should throw any exceptions to report such invalid case.
	/// </summary>
	/// <param name="args">The arguments to be parsed.</param>
	/// <param name="startIndex">The start index of the argument to be parsed.</param>
	/// <seealso cref="Result"/>
	public abstract void AssignResult(ReadOnlySpan<string> args, int startIndex);
}
