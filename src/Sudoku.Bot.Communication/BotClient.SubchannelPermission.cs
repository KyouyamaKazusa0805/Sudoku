namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// 获取用户在指定子频道的权限
	/// </summary>
	/// <param name="channel_id">子频道Id</param>
	/// <param name="user_id">用户Id</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<ChannelPermissions?> GetChannelPermissionsAsync(
		string channel_id, string user_id, Sender? sender = null)
	{
		var api = BotApis.获取子频道用户权限;
		var response = await HttpSendAsync(
			api.Path.Replace("{channel_id}", channel_id).Replace("{user_id}", user_id),
			api.Method,
			null,
			sender
		);

		return response is null ? null : await response.Content.ReadFromJsonAsync<ChannelPermissions?>();
	}

	/// <summary>
	/// 修改用户在指定子频道的权限
	/// </summary>
	/// <param name="channel_id">子频道Id</param>
	/// <param name="user_id">用户Id</param>
	/// <param name="add">添加的权限</param>
	/// <param name="remove">删除的权限</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<bool> EditChannelPermissionsAsync(
		string channel_id, string user_id, string add = "0", string remove = "0", Sender? sender = null)
	{
		var api = BotApis.修改子频道用户权限;
		var response = await HttpSendAsync(
			api.Path.Replace("{channel_id}", channel_id).Replace("{user_id}", user_id),
			api.Method,
			JsonContent.Create(new { add, remove }),
			sender
		);

		return response?.IsSuccessStatusCode ?? false;
	}

	/// <summary>
	/// 获取指定身份组在指定子频道的权限
	/// </summary>
	/// <param name="channel_id">子频道Id</param>
	/// <param name="role_id">身份组Id</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<ChannelPermissions?> GetMemberChannelPermissionsAsync(
		string channel_id, string role_id, Sender? sender = null)
	{
		var api = BotApis.获取子频道身份组权限;
		var response = await HttpSendAsync(
			api.Path.Replace("{channel_id}", channel_id).Replace("{role_id}", role_id),
			api.Method,
			null,
			sender
		);

		return response is null ? null : await response.Content.ReadFromJsonAsync<ChannelPermissions?>();
	}

	/// <summary>
	/// 修改指定身份组在指定子频道的权限
	/// </summary>
	/// <param name="channel_id">子频道Id</param>
	/// <param name="role_id">身份组Id</param>
	/// <param name="add">添加的权限</param>
	/// <param name="remove">删除的权限</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<bool> EditMemberChannelPermissionsAsync(
		string channel_id, string role_id, string add = "0", string remove = "0", Sender? sender = null)
	{
		var api = BotApis.修改子频道身份组权限;
		var response = await HttpSendAsync(
			api.Path.Replace("{channel_id}", channel_id).Replace("{role_id}", role_id),
			api.Method,
			JsonContent.Create(new { add, remove }),
			sender
		);

		return response?.IsSuccessStatusCode ?? false;
	}
}
