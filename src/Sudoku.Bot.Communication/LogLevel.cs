namespace Sudoku.Bot.Communication;

/// <summary>
/// Defines a level that controls the output message limited to it.
/// </summary>
public enum LogLevel
{
	/// <summary>
	/// Indicates the log level is limited to debugging.
	/// </summary>
	Debug,

	/// <summary>
	/// Indicates the log level is limited to information.
	/// </summary>
	Info,

	/// <summary>
	/// Indicates the log level is limited to warning.
	/// </summary>
	Warning,

	/// <summary>
	/// Indicates the log level is limited to error.
	/// </summary>
	Error
}
