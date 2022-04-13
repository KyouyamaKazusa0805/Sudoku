namespace Sudoku.CommandLine;

/// <summary>
/// Provides with an error case that introduces why the runtime has been crashed.
/// </summary>
internal enum ErrorCode
{
	/// <summary>
	/// Indicates that everything looks good.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the error case that the argument is currently <see langword="null"/>.
	/// </summary>
	ArgGridValueIsNull = -1,

	/// <summary>
	/// Indicates the error case that the argument is invalid to be parsed into type <see cref="Grid"/>.
	/// </summary>
	/// <seealso cref="Grid"/>
	ArgGridValueIsInvalidWhileParsing = -2,

	/// <summary>
	/// Indicates the error case that the argument parsed into type <see cref="Grid"/> is not unique.
	/// </summary>
	/// <seealso cref="Grid"/>
	ArgGridValueIsNotUnique = -3,

	/// <summary>
	/// Indicates the error case that the attribute name is invalid.
	/// </summary>
	ArgAttributeNameIsInvalid = -101,

	/// <summary>
	/// Indicates the error case that the website link is failed to be used to visit.
	/// </summary>
	SiteIsFailedToVisit = -201,

	/// <summary>
	/// Indicates the error case that the command line arguments are failed to be parsed.
	/// </summary>
	ParseFailed = -1001,
}
