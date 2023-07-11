namespace Sudoku.Cli.Commands;

/// <summary>
/// Represents a command.
/// </summary>
/// <typeparam name="TSelf">the type itself.</typeparam>
public interface ICommand<TSelf> where TSelf : Command, ICommand<TSelf>, new()
{
	/// <summary>
	/// Creates a (an) <typeparamref name="TSelf"/> instance.
	/// </summary>
	/// <returns>A (An) <typeparamref name="TSelf"/> instance.</returns>
	static TSelf CreateCommand() => new();
}
