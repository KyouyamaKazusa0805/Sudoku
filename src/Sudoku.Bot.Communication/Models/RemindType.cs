namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Defines a remind type. The enumeration type is used for the scheduling.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/schedule/model.html#remindtype">this link</see>.
/// </remarks>
public enum RemindType
{
	/// <summary>
	/// Indicates the schedule never reminds me.
	/// </summary>
	Never,

	/// <summary>
	/// Indicates the schedule reminds me on starting.
	/// </summary>
	OnStart,

	/// <summary>
	/// Indicates the schedule reminds me before starting the activity 5 minutes.
	/// </summary>
	Early5Min,

	/// <summary>
	/// Indicates the schedule reminds me before starting the activity 15 minutes.
	/// </summary>
	Early15Min,

	/// <summary>
	/// Indicates the schedule reminds me before starting the activity 30 minutes.
	/// </summary>
	Early30Min,

	/// <summary>
	/// Indicates the schedule reminds me before starting the activity 60 minutes.
	/// </summary>
	Early60Min
}
