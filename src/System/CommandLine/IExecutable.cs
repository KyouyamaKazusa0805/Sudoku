namespace System.CommandLine;

/// <summary>
/// Represents an executable command.
/// </summary>
public interface IExecutable
{
	/// <summary>
	/// Try to execute the command, and returns the result reflected the execution.
	/// </summary>
	/// <exception cref="CommandLineRuntimeException">Throws when an error has been encountered.</exception>
	void Execute();
}
