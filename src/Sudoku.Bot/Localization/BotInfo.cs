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


	/// <summary>
	/// 允许 <see cref="BotInfo"/> 到 <see cref="OpenApiAccessInfo"/> 的显式转换。
	/// </summary>
	/// <param name="info"><see cref="BotInfo"/> 对象。</param>
	public static explicit operator OpenApiAccessInfo?(BotInfo? info)
		=> info is null ? null : new() { BotQQ = info.Id, BotAppId = info.AppId, BotToken = info.Token, BotSecret = info.Secret };
}
