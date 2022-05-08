namespace Sudoku.Bot.Communication;

/// <summary>
/// The identity instance.
/// </summary>
public struct Identity
{
	/// <summary>
	/// The ID of the bot. The value corresponds to the AppID.
	/// </summary>
	public string BotAppId { get; set; }

	/// <summary>
	/// The token of the bot.
	/// </summary>
	public string BotToken { get; set; }

	/// <summary>
	/// The secret key of the bot.
	/// </summary>
	public string BotSecret { get; set; }
}
