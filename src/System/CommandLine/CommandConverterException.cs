namespace System.CommandLine;

/// <summary>
/// Represents an error case that is thrown if command converter has encountered it.
/// </summary>
public sealed class CommandConverterException : CommandLineException
{
	/// <summary>
	/// Initializes a <see cref="CommandConverterAttribute"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CommandConverterException() : base(1002)
	{
	}

	/// <summary>
	/// Initializes a <see cref="CommandConverterAttribute"/> instance via the specified error message.
	/// </summary>
	/// <param name="message">The error message.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CommandConverterException(string message) : base(1002, message)
	{
	}
}
