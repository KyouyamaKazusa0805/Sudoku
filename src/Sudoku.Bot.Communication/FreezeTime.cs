namespace Sudoku.Bot.Communication;

/// <summary>
/// Encapsulates a data type that describes a time span of freezing a bot, if the bot emits
/// some messages that QQ doesn't allow.
/// </summary>
public sealed class FreezeTime
{
	/// <summary>
	/// Indicates the time that ends the freezing.
	/// </summary>
	public DateTime EndTime { get; set; }

	/// <summary>
	/// Indicates the extra time freezing the bot.
	/// </summary>
	public TimeSpan AddTime { get; set; }
}
