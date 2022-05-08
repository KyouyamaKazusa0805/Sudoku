namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// 获取频道成员列表（仅私域可用）
	/// <para>
	/// guild_id - 频道Id<br/>
	/// limit - 分页大小1-1000（默认值10）<br/>
	/// after - 上次回包中最后一个Member的用户ID，首次请求填0
	/// </para>
	/// </summary>
	/// <param name="guild_id">频道Id</param>
	/// <param name="limit">分页大小1-1000（默认值10）</param>
	/// <param name="after">上次回包中最后一个Member的用户ID，首次请求填"0"</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<List<Member>?> GetGuildMembersAsync(
		string guild_id, int limit = 10, string? after = null, Sender? sender = null)
	{
		var api = BotApis.获取频道成员列表;
		var response = await HttpSendAsync(
			$"{api.Path.Replace("{guild_id}", guild_id)}?limit={limit}&after={after ?? "0"}",
			api.Method,
			null,
			sender
		);

		return response is null ? null : await response.Content.ReadFromJsonAsync<List<Member>?>();
	}

	/// <summary>
	/// 获取频道成员详情
	/// </summary>
	/// <param name="guild_id">频道Id</param>
	/// <param name="user_id">成员用户Id</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<Member?> GetMemberAsync(string guild_id, string user_id, Sender? sender = null)
	{
		var api = BotApis.获取成员详情;
		var response = await HttpSendAsync(
			api.Path.Replace("{guild_id}", guild_id).Replace("{user_id}", user_id),
			api.Method,
			null,
			sender
		);

		return response is null ? null : await response.Content.ReadFromJsonAsync<Member?>();
	}

	/// <summary>
	/// 删除指定频道成员（仅私域可用）
	/// </summary>
	/// <param name="guild_id">频道Id</param>
	/// <param name="user_id">用户Id</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<bool> DeleteGuildMemberAsync(string guild_id, string user_id, Sender? sender = null)
	{
		var api = BotApis.删除频道成员;
		var response = await HttpSendAsync(
			api.Path.Replace("{guild_id}", guild_id).Replace("{user_id}", user_id),
			api.Method,
			null,
			sender
		);

		return response?.IsSuccessStatusCode ?? false;
	}
}
