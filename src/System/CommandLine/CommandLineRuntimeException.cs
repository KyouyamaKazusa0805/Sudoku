namespace System.CommandLine;

/// <summary>
/// Defines an exception type that will be thrown when an error has been encountered while command line handling.
/// </summary>
public sealed class CommandLineRuntimeException : CommandLineException
{
	/// <summary>
	/// Initializes a <see cref="CommandLineRuntimeException"/> instance via the specified error code.
	/// </summary>
	/// <param name="errorCode">The error code.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CommandLineRuntimeException(int errorCode) : base(errorCode)
	{
	}
}
