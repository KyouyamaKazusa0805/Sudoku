namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// 获取当前用户(机器人)信息
	/// <para>此API无需任何权限</para>
	/// </summary>
	/// <param name="sender"></param>
	/// <returns>当前用户对象</returns>
	public async Task<User?> GetMeAsync(Sender? sender = null)
	{
		var api = BotApis.获取用户详情;
		var response = await HttpSendAsync(api.Path, api.Method, null, sender);
		return response is null ? null : await response.Content.ReadFromJsonAsync<User?>();
	}

	/// <summary>
	/// 获取当前用户(机器人)已加入频道列表
	/// <para>此API无需任何权限</para>
	/// </summary>
	/// <param name="guild_id">频道Id（作为拉取下一次列表的分页坐标使用）</param>
	/// <param name="route">数据拉取方向（true-向前查找 | false-向后查找）</param>
	/// <param name="limit">数据分页（默认每次拉取100条）</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<List<Guild>?> GetMeGuildsAsync(
		string? guild_id = null, bool route = false, int limit = 100, Sender? sender = null)
	{
		var api = BotApis.获取用户频道列表;
		guild_id = string.IsNullOrWhiteSpace(guild_id) ? "" : $"&{(route ? "before" : "after")}={guild_id}";
		var response = await HttpSendAsync($"{api.Path}?limit={limit}{guild_id}", api.Method, null, sender);
		return response is null ? null : await response.Content.ReadFromJsonAsync<List<Guild>?>();
	}
}
