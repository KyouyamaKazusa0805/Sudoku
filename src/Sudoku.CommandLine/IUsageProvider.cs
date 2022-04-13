namespace Sudoku.CommandLine;

/// <summary>
/// Defines an option instance that provides with examples introducing the usages of the current command.
/// </summary>
public interface IUsageProvider
{
	/// <summary>
	/// Introduces the usages of the current command.
	/// </summary>
	/// <remarks><b><i>
	/// Due to the bug of the command line nuget package, we should disable the
	/// implicitly-generated nullable attribute and then use this property; otherwise
	/// the <see cref="InvalidCastException"/>-typed exception instance will be thrown.
	/// For more details on this bug, please visit
	/// <see href="https://github.com/commandlineparser/commandline/issues/714">this link</see>.
	/// </i></b></remarks>
	static abstract IEnumerable<Example> Examples { get; }
}
