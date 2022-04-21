namespace System.CommandLine;

/// <summary>
/// Defines a parser error.
/// </summary>
public enum ParserError
{
	/// <summary>
	/// Indicates the error that the unexpected arguments.
	/// </summary>
	[Description("Don't require any command line arguments now.")]
	SpecialCommandDoNotRequireOtherArguments = 1001,

	/// <summary>
	/// Indicates the error that the command line arguments is invalid.
	/// </summary>
	[Description("The command line arguments is invalid.")]
	ArgumentFormatInvalid,

	/// <summary>
	/// Indicates the error that the command name is invalid.
	/// </summary>
	[Description("The command name is invalid.")]
	CommandNameIsInvalid,

	/// <summary>
	/// Indicates the error that ambiguous matched or mismatched.
	/// </summary>
	[Description("Ambiguous matched or mismatched.")]
	ArgumentsAmbiguousMatchedOrMismatched,

	/// <summary>
	/// Indicates the error that the argument is mismatched.
	/// </summary>
	[Description("The argument mismatched.")]
	ArgumentMismatched,

	/// <summary>
	/// Indicates the error that the argument is expected.
	/// </summary>
	[Description("Cannot operate due to the lack of the real value.")]
	ArgumentExpected,

	/// <summary>
	/// Indicates the error that the converted type must be a <see cref="string"/>.
	/// </summary>
	[Description("The target type must be a string.")]
	ConvertedTypeMustBeString,

	/// <summary>
	/// Indicates the error tha the command line arguments is empty.
	/// </summary>
	[Description("The command line argument cannot be empty.")]
	ArgumentIsEmpty
}
