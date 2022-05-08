namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 日程提醒方式
/// </summary>
public enum RemindType
{
	/// <summary>
	/// 不提醒
	/// </summary>
	Never,
	/// <summary>
	/// 开始时提醒
	/// </summary>
	OnStart,
	/// <summary>
	/// 开始前5分钟提醒
	/// </summary>
	Early5Min,
	/// <summary>
	/// 开始前15分钟提醒
	/// </summary>
	Early15Min,
	/// <summary>
	/// 开始前30分钟提醒
	/// </summary>
	Early30Min,
	/// <summary>
	/// 开始前60分钟提醒
	/// </summary>
	Early60Min
}
