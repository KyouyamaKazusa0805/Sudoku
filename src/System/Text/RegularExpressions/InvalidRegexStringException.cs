namespace System.Text.RegularExpressions;

/// <summary>
/// Indicates an error for reporting a string is an invalid regular expression.
/// </summary>
public sealed class InvalidRegexStringException : Exception
{
	/// <inheritdoc/>
	public InvalidRegexStringException() : base()
	{
	}

	/// <inheritdoc/>
	public InvalidRegexStringException(string message) : base(message)
	{
	}

	/// <summary>
	/// Initializes an instance with the message and the regular expression.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="regex">The regular expression.</param>
	public InvalidRegexStringException(string message, string regex) : base(message) =>
		WrongRegexString = regex;

	/// <inheritdoc/>
	public InvalidRegexStringException(string message, Exception innerException)
	: base(message, innerException)
	{
	}

	/// <summary>
	/// Initializes an instance with the message, wrong regular expression and the inner exception.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="regex">The regular expression.</param>
	/// <param name="innerException">The inner expression.</param>
	public InvalidRegexStringException(string message, string regex, Exception innerException)
	: base(message, innerException) =>
		WrongRegexString = regex;


	/// <summary>
	/// Indicates the wrong regex string.
	/// </summary>
	public string? WrongRegexString { get; init; }

	/// <inheritdoc/>
	public override string Message =>
		$"The specified regex string is invalid{(WrongRegexString is null ? string.Empty : $": {WrongRegexString}")}.";
}
