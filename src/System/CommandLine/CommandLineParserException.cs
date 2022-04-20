namespace System.CommandLine;

/// <summary>
/// Defines the command line parser exception.
/// </summary>
public sealed class CommandLineParserException : Exception
{
	/// <summary>
	/// Initializes a <see cref="CommandLineParserException"/> instance via the specified error case.
	/// </summary>
	/// <param name="errorCode">The error code.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CommandLineParserException(ParserError errorCode) :
		base(
			typeof(ParserError)
				.GetField(errorCode.ToString())!
				.GetCustomAttribute<DescriptionAttribute>()!
				.Description
		) => ErrorCode = errorCode;


	/// <summary>
	/// Indicates the error code.
	/// </summary>
	public ParserError ErrorCode { get; }
}
