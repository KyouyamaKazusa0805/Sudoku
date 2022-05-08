namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// 获取频道可用权限列表
	/// <para>
	/// 获取机器人在频道 guild_id 内可以使用的权限列表
	/// </para>
	/// </summary>
	/// <param name="guild_id">频道Id</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<List<ApiPermission>?> GetGuildPermissionsAsync(string guild_id, Sender? sender = null)
	{
		var api = BotApis.获取频道可用权限列表;
		var response = await HttpSendAsync(api.Path.Replace("{guild_id}", guild_id), api.Method, null, sender);
		var permissions = response is null ? null : await response.Content.ReadFromJsonAsync<ApiPermissions?>();
		return permissions?.List;
	}

	/// <summary>
	/// 创建频道 API 接口权限授权链接
	/// </summary>
	/// <param name="guild_id">频道Id</param>
	/// <param name="channel_id">子频道Id</param>
	/// <param name="api_identify">权限需求标识对象</param>
	/// <param name="desc">机器人申请对应的 API 接口权限后可以使用功能的描述</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<ApiPermissionDemand?> SendPermissionDemandAsync(
		string guild_id, string channel_id, ApiPermissionDemandIdentify api_identify,
		string desc = "", Sender? sender = null)
	{
		var api = BotApis.创建频道接口授权链接;
		var response = await HttpSendAsync(
			api.Path.Replace("{guild_id}", guild_id),
			api.Method,
			JsonContent.Create(
				new
				{
					channel_id,
					api_identify,
					desc
				}
			),
			sender
		);

		return response is null ? null : await response.Content.ReadFromJsonAsync<ApiPermissionDemand?>();
	}
}
