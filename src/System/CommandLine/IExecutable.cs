namespace System.CommandLine;

/// <summary>
/// Represents an executable command.
/// </summary>
public interface IExecutable
{
	/// <summary>
	/// Try to execute the command, and returns the result reflected the execution.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token that has ability to cancel the operation via itself.</param>
	/// <returns>A task that handles the operation.</returns>
	/// <exception cref="CommandLineRuntimeException">Throws when an error has been encountered.</exception>
	Task ExecuteAsync(CancellationToken cancellationToken = default);
}
