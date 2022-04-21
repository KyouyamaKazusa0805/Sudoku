namespace System.CommandLine;

/// <summary>
/// Represents a root command.
/// </summary>
public interface IRootCommand
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
	static abstract IEnumerable<(string CommandLine, string Meaning)>? UsageCommands { get; }


	/// <summary>
	/// Try to execute the command, and returns the result reflected the execution.
	/// </summary>
	/// <exception cref="CommandLineRuntimeException">Throws when an error has been encountered.</exception>
	void Execute();
}
