namespace Sudoku.Diagnostics.LanguageFeatures;

/// <summary>
/// Represents an exception to display the error information about a code is invalid to process.
/// </summary>
public sealed class CodeIsInvalidException : InvalidOperationException
{
	/// <summary>
	/// Indicates the message to represent the error.
	/// </summary>
	private const string InvalidMessage = "The specified code is invalid to process.";


	/// <summary>
	/// Initializes a <see cref="CodeIsInvalidException"/> instance without any items to pass.
	/// </summary>
	public CodeIsInvalidException() : base()
	{
	}

	/// <summary>
	/// Initializes a <see cref="CodeIsInvalidException"/> instance with the specified code.
	/// </summary>
	/// <param name="code">The code.</param>
	public CodeIsInvalidException(string? code) : base(InvalidMessage) => Code = code;

	/// <summary>
	/// Initializes a <see cref="CodeIsInvalidException"/> instance with the specified error message
	/// and the code.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="code">The code.</param>
	public CodeIsInvalidException(string message, string? code) : base(message) => Code = code;

	/// <summary>
	/// Initializes a <see cref="CodeIsInvalidException"/> instance
	/// with the specified code and the inner exception instance.
	/// </summary>
	/// <param name="code">The code.</param>
	/// <param name="innerException">The inner exception.</param>
	public CodeIsInvalidException(string? code, Exception innerException)
	: base(InvalidMessage, innerException) =>
		Code = code;

	/// <summary>
	/// Initializes a <see cref="CodeIsInvalidException"/> instance with the specified error message, the code
	/// and the inner exception instance.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="code">The code.</param>
	/// <param name="innerException">The inner exception.</param>
	public CodeIsInvalidException(string message, string? code, Exception innerException)
	: base(message, innerException) =>
		Code = code;


	/// <inheritdoc/>
	public override string Message => InvalidMessage;

	/// <summary>
	/// Indicates the error code.
	/// </summary>
	public string? Code { get; }
}
