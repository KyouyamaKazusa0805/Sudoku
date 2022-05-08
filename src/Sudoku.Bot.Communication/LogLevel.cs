namespace Sudoku.Bot.Communication;

/// <summary>
/// Defines a level that controls the output message limited to it.
/// </summary>
public enum LogLevel
{
	/// <summary>
	/// Indicates the log level is limited to debugging.
	/// </summary>
	DEBUG,

	/// <summary>
	/// Indicates the log level is limited to information.
	/// </summary>
	INFO,

	/// <summary>
	/// Indicates the log level is limited to warning.
	/// </summary>
	WARRNING,

	/// <summary>
	/// Indicates the log level is limited to error.
	/// </summary>
	ERROR
}
