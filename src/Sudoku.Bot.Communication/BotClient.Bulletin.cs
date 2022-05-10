namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// 创建频道全局公告
	/// </summary>
	/// <param name="guild_id">频道Id</param>
	/// <param name="channel_id">消息所在子频道Id</param>
	/// <param name="message_id">消息Id</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<Announces?> CreateAnnouncesGlobalAsync(
		string guild_id, string channel_id, string message_id, Sender? sender = null)
	{
		_ = BotApis.CreateAnnouncementInGuild is { Path: var path, Method: var method };
		var response = await HttpSendAsync(
			path.Replace("{guild_id}", guild_id),
			method,
			JsonContent.Create(new { channel_id, message_id }), sender
		);

		return response is null ? null : await response.Content.ReadFromJsonAsync<Announces?>();
	}

	/// <summary>
	/// 删除频道全局公告
	/// </summary>
	/// <param name="guild_id">频道Id</param>
	/// <param name="message_id">用于创建公告的消息Id</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<bool> DeleteAnnouncesGlobalAsync(string guild_id, string message_id = "all", Sender? sender = null)
	{
		_ = BotApis.DeleteAnnouncementInGuild is { Path: var path, Method: var method };
		var response = await HttpSendAsync(
			path.Replace("{guild_id}", guild_id).Replace("{message_id}", message_id),
			method,
			null,
			sender
		);

		return response?.IsSuccessStatusCode ?? false;
	}

	/// <summary>
	/// 创建子频道公告
	/// </summary>
	/// <param name="channel_id">子频道Id</param>
	/// <param name="message_id">消息Id</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<Announces?> CreateAnnouncesAsync(string channel_id, string message_id, Sender? sender = null)
	{
		_ = BotApis.CreateAnnouncementInChannel is { Path: var path, Method: var method };
		var response = await HttpSendAsync(
			path.Replace("{channel_id}", channel_id),
			method,
			channel_id is null ? null : JsonContent.Create(new { message_id }),
			sender
		);

		return response is null ? null : await response.Content.ReadFromJsonAsync<Announces?>();
	}

	/// <summary>
	/// 删除子频道公告
	/// </summary>
	/// <param name="channel_id">子频道Id</param>
	/// <param name="message_id">用于创建公告的消息Id</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<bool> DeleteAnnouncesAsync(string channel_id, string message_id = "all", Sender? sender = null)
	{
		_ = BotApis.DeleteAnnouncementInChannel is { Path: var path, Method: var method };
		var response = await HttpSendAsync(
			path.Replace("{channel_id}", channel_id).Replace("{message_id}", message_id),
			method,
			null,
			sender
		);

		return response?.IsSuccessStatusCode ?? false;
	}
}
