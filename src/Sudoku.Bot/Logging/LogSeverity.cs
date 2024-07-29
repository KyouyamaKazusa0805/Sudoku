namespace Sudoku.Bot.Logging;

/// <summary>
/// 表示一种日志的严重级别。
/// </summary>
public enum LogSeverity
{
	/// <summary>
	/// 表示默认数值，不属于任何的一种严重级别。
	/// </summary>
	None,

	/// <summary>
	/// 表示信息。
	/// </summary>
	Info,

	/// <summary>
	/// 表示警告。
	/// </summary>
	Warning,

	/// <summary>
	/// 表示错误。
	/// </summary>
	Error
}
