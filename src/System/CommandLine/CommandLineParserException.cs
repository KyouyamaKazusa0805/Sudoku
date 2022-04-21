namespace System.CommandLine;

/// <summary>
/// Defines the command line parser exception.
/// </summary>
public sealed class CommandLineParserException : CommandLineException
{
	/// <summary>
	/// Initializes a <see cref="CommandLineParserException"/> instance via the specified error case.
	/// </summary>
	/// <param name="errorCode">The error code.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CommandLineParserException(CommandLineInternalError errorCode) :
		base((int)errorCode, InitializePropertyMessage(errorCode))
	{
	}


	/// <summary>
	/// To initializes the property <see cref="Exception.Message"/> via the specified error code.
	/// </summary>
	/// <param name="errorCode">The error code.</param>
	/// <returns>The message string value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string InitializePropertyMessage(CommandLineInternalError errorCode) =>
		typeof(CommandLineInternalError)
			.GetField(errorCode.ToString())!
			.GetCustomAttribute<DescriptionAttribute>()!
			.Description!;
}
