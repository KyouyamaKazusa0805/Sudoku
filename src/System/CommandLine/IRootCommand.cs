namespace System.CommandLine;

/// <summary>
/// Represents a root command.
/// </summary>
/// <typeparam name="TErrorCode">The type of the error.</typeparam>
public interface IRootCommand<TErrorCode> where TErrorCode : Enum
{
	/// <summary>
	/// Indicates the name of the root command.
	/// </summary>
	static abstract string Name { get; }

	/// <summary>
	/// Indicates the description of the root command.
	/// </summary>
	static abstract string Description { get; }

	/// <summary>
	/// Indicates the supported commands.
	/// </summary>
	static abstract string[] SupportedCommands { get; }

	/// <summary>
	/// Indicates the commands that is provided for the usages.
	/// </summary>
	static abstract IEnumerable<IRootCommand<TErrorCode>>? UsageCommands { get; }


	/// <summary>
	/// Try to execute the command, and returns the result reflected the execution.
	/// </summary>
	/// <returns>An enumeration typed instance to indicate the error.</returns>
	TErrorCode Execute();
}
