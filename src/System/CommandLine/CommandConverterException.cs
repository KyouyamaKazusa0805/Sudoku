namespace System.CommandLine;

/// <summary>
/// Represents an error case that is thrown if command converter has encountered it.
/// </summary>
public sealed class CommandConverterException : CommandLineException
{
	/// <summary>
	/// Initializes a <see cref="CommandConverterAttribute{TConverter}"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CommandConverterException() : base((int)CommandLineInternalError.ConverterError)
	{
	}

	/// <summary>
	/// Initializes a <see cref="CommandConverterAttribute{TConverter}"/> instance via the specified error message.
	/// </summary>
	/// <param name="message">The error message.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CommandConverterException(string message) : base((int)CommandLineInternalError.ConverterError, message)
	{
	}
}
