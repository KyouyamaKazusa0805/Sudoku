namespace Sudoku.CommandLine;

/// <summary>
/// Provides with an error case that introduces why the runtime has been crashed.
/// </summary>
public enum ErrorCode
{
	/// <summary>
	/// Indicates that everything looks good.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the error case that the format string is invalid.
	/// </summary>
	ArgFormatIsInvalid = 101,

	/// <summary>
	/// Indicates the error case that the argument is currently <see langword="null"/>.
	/// </summary>
	ArgGridValueIsNull = 201,

	/// <summary>
	/// Indicates the error case that the argument is invalid to be parsed into type <see cref="Grid"/>.
	/// </summary>
	/// <seealso cref="Grid"/>
	ArgGridValueIsInvalidWhileParsing,

	/// <summary>
	/// Indicates the error case that the argument parsed into type <see cref="Grid"/> is not unique.
	/// </summary>
	/// <seealso cref="Grid"/>
	ArgGridValueIsNotUnique,

	/// <summary>
	/// Indicates the error case that the solving method is invalid.
	/// </summary>
	ArgMethodIsInvalid,

	/// <summary>
	/// Indicates the error case that the attribute name is invalid.
	/// </summary>
	ArgAttributeNameIsInvalid = 301,

	/// <summary>
	/// Indicates the error case that the range pattern is invalid.
	/// </summary>
	RangePatternIsInvalid = 401,

	/// <summary>
	/// Indicates the error case that the method is invalid.
	/// </summary>
	MethodIsInvalid,

	/// <summary>
	/// Indicates the error case that the website link is failed to be used to visit.
	/// </summary>
	SiteIsFailedToVisit = 501,

	/// <summary>
	/// Indicates the error case that the website links is invalid.
	/// </summary>
	SiteLinkIsInvalid,

	/// <summary>
	/// Indicates the error case that the command line arguments are failed to be parsed.
	/// </summary>
	ParseFailed = 1001,

	/// <summary>
	/// Indicates the error case that the command line cannot correctly fetch the assembly name.
	/// </summary>
	AssemblyNameIsNull = 2001,

	/// <summary>
	/// Indicates the error that the command line arguments is empty.
	/// </summary>
	EmptyCommandLineArguments = 9001,

	/// <summary>
	/// Indicates the error case that the command line has encountered an error that isn't recorded
	/// in the current enumeration type.
	/// </summary>
	OtherRuntimeError,
}
