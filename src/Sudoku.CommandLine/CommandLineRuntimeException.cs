namespace Sudoku.CommandLine;

/// <summary>
/// Provides with an exception instance that will be thrown when the runtime crashes with an error code
/// to introduce the reason why the runtime crashed.
/// </summary>
internal sealed class CommandLineRuntimeException : Exception
{
	/// <summary>
	/// Initializes a <see cref="CommandLineRuntimeException"/> instance via the specified error code.
	/// </summary>
	/// <param name="errorCode">The error code.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CommandLineRuntimeException(ErrorCode errorCode) : base() => ErrorCode = errorCode;


	/// <summary>
	/// Indicates the error code introduces the error case that cause the runtime crashes.
	/// </summary>
	public ErrorCode ErrorCode { get; }
}
