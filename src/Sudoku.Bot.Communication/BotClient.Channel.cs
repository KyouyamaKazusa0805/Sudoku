namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// 获取频道详情
	/// </summary>
	/// <param name="guild_id">频道Id</param>
	/// <param name="sender"></param>
	/// <returns>Guild?</returns>
	public async Task<Guild?> GetGuildAsync(string guild_id, Sender? sender = null)
	{
		_ = BotApis.GetGuildDetail is { Path: var path, Method: var method };
		var response = await HttpSendAsync(path.Replace("{guild_id}", guild_id), method, null, sender);
		return response == null ? null : await response.Content.ReadFromJsonAsync<Guild?>();
	}
}
