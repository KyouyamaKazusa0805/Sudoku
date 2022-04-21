namespace System.CommandLine;

/// <summary>
/// Defines an error that is raised by the command-line handler runtime.
/// </summary>
/// <remarks><b>
/// All possible internal error uses the integers between 1001 and 2000. If you has defined a new error code type,
/// please avoid the range of these integers.
/// </b></remarks>
public enum CommandLineInternalError
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
	/// Indicates the error that the command line arguments is empty.
	/// </summary>
	[Description("The command line argument cannot be empty.")]
	ArgumentIsEmpty,

	/// <summary>
	/// Indicates the error that the command converter cannot convert the specified text into the target type
	/// due to invalid text.
	/// </summary>
	[Description("The command convert cannot convert the value into the target type due to invalid text.")]
	ConverterError = 1101,
}
