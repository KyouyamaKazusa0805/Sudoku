namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// 创建私信会话
	/// </summary>
	/// <param name="recipient_id">接收者Id</param>
	/// <param name="source_guild_id">源频道Id</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<DirectMessageSource?> CreateDMSAsync(string recipient_id, string source_guild_id, Sender sender)
	{
		var api = BotApis.创建私信会话;
		var response = await HttpSendAsync(api.Path, api.Method, JsonContent.Create(new { recipient_id, source_guild_id }), sender);
		return response is null ? null : await response.Content.ReadFromJsonAsync<DirectMessageSource?>();
	}

	/// <summary>
	/// 发送私信
	/// <para>用于发送私信消息，前提是已经创建了私信会话。</para>
	/// </summary>
	/// <param name="guild_id">私信频道Id</param>
	/// <param name="message">消息对象</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<Message?> SendPMAsync(string guild_id, MessageToCreate message, Sender? sender = null)
	{
		var api = BotApis.发送私信;
		var response = await HttpSendAsync(
			api.Path.Replace("{guild_id}", guild_id),
			api.Method,
			JsonContent.Create(message),
			sender
		);

		var result = response is null ? null : await response.Content.ReadFromJsonAsync<Message?>();
		return LastMessage(result, true);
	}
}
