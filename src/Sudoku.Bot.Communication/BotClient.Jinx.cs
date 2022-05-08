namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// 频道全局禁言
	/// <para>
	/// muteTime - 禁言时间：<br/>
	/// mute_end_timestamp 禁言到期时间戳，绝对时间戳，单位：秒<br/>
	/// mute_seconds 禁言多少秒
	/// </para>
	/// </summary>
	/// <param name="guild_id">频道Id</param>
	/// <param name="muteTime">禁言模式</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<bool> MuteGuildAsync(string guild_id, JinxTime muteTime, Sender? sender = null)
	{
		var api = BotApis.禁言全员;
		var response = await HttpSendAsync(
			api.Path.Replace("{guild_id}", guild_id),
			api.Method,
			JsonContent.Create(muteTime),
			sender
		);

		return response?.IsSuccessStatusCode ?? false;
	}

	/// <summary>
	/// 频道指定成员禁言
	/// <para>
	/// muteTime - 禁言时间：<br/>
	/// mute_end_timestamp 禁言到期时间戳，绝对时间戳，单位：秒<br/>
	/// mute_seconds 禁言多少秒
	/// </para>
	/// </summary>
	/// <param name="guild_id">频道Id</param>
	/// <param name="user_id">成员Id</param>
	/// <param name="muteTime">禁言时间</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<bool> MuteMemberAsync(string guild_id, string user_id, JinxTime muteTime, Sender? sender = null)
	{
		var api = BotApis.禁言指定成员;
		var response = await HttpSendAsync(
			api.Path.Replace("{guild_id}", guild_id).Replace("{user_id}", user_id),
			api.Method,
			JsonContent.Create(muteTime),
			sender
		);

		return response?.IsSuccessStatusCode ?? false;
	}
}
