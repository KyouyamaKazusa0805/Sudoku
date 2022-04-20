namespace System.CommandLine;

/// <summary>
/// Defines an exception type that will be thrown when an error has been encountered while command line handling.
/// </summary>
public sealed class CommandLineException : Exception
{
	/// <summary>
	/// Initializes a <see cref="CommandLineException"/> instance via the specified error code.
	/// </summary>
	/// <param name="errorCode">The error code.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CommandLineException(int errorCode) : base() => ErrorCode = errorCode;


	/// <summary>
	/// Indicates the error code.
	/// </summary>
	public int ErrorCode { get; }
}
