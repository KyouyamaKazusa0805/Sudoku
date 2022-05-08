namespace Sudoku.Bot.Communication;

/// <summary>
/// 时间冻结类
/// </summary>
public sealed class FreezeTime
{
	/// <summary>
	/// 结束时间
	/// </summary>
	public DateTime EndTime { get; set; } = DateTime.MinValue;
	/// <summary>
	/// 附加时间
	/// </summary>
	public TimeSpan AddTime { get; set; } = TimeSpan.Zero;
}
