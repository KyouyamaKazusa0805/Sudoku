namespace System.CommandLine;

/// <summary>
/// Represents a command line exception that will be thrown while parsing or executing a command.
/// </summary>
public abstract class CommandLineException : Exception
{
	/// <summary>
	/// Initializes the property <see cref="ErrorCode"/> with the specified value.
	/// </summary>
	/// <param name="errorCode">The error code value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private protected CommandLineException(int errorCode) : base() => ErrorCode = errorCode;

	/// <summary>
	/// Initializes the property <see cref="ErrorCode"/> and <see cref="Exception.Message"/>
	/// with the specified value.
	/// </summary>
	/// <param name="errorCode">The error code value.</param>
	/// <param name="message">The error message.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private protected CommandLineException(int errorCode, string message) : base(message) =>
		ErrorCode = errorCode;


	/// <summary>
	/// Indicates the error code.
	/// </summary>
	public int ErrorCode { get; }
}
