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
		base((int)errorCode, InitializePropertyMessage(errorCode, null))
	{
	}

	/// <summary>
	/// Initializes a <see cref="CommandLineParserException"/> instance via the specified error case,
	/// and the extra message to describe the extra information.
	/// </summary>
	/// <param name="errorCode">The error code.</param>
	/// <param name="extraErrorMessage">The extra error message.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CommandLineParserException(CommandLineInternalError errorCode, string extraErrorMessage) :
		base((int)errorCode, InitializePropertyMessage(errorCode, extraErrorMessage))
	{
	}


	/// <summary>
	/// To initializes the property <see cref="Exception.Message"/> via the specified error code.
	/// </summary>
	/// <param name="errorCode">The error code.</param>
	/// <param name="extraMessage">The extra error message.</param>
	/// <returns>The message string value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string InitializePropertyMessage(CommandLineInternalError errorCode, string? extraMessage)
	{
		var baseMessage = typeof(CommandLineInternalError)
			.GetField(errorCode.ToString())!
			.GetCustomAttribute<DescriptionAttribute>()!
			.Description!;

		return $"{baseMessage}{(extraMessage is null ? string.Empty : $"\r\n\r\n{extraMessage}")}";
	}	
}
