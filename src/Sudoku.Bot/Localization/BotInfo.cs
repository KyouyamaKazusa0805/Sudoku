namespace Sudoku.Bot.Localization;

/// <summary>
/// 表示一个机器人的基本信息。
/// </summary>
public sealed class BotInfo
{
	/// <summary>
	/// 表示机器人的 ID。
	/// </summary>
	public string Id { get; set; } = "";

	/// <summary>
	/// 表示机器人的应用 ID（App ID）。
	/// </summary>
	public string AppId { get; set; } = "";

	/// <summary>
	/// 表示 token。
	/// </summary>
	public string Token { get; set; } = "";

	/// <summary>
	/// 表示 secret。
	/// </summary>
	public string Secret { get; set; } = "";
}
