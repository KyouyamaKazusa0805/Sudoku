namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// 获取子频道详情
	/// </summary>
	/// <param name="channel_id">子频道Id</param>
	/// <param name="sender"></param>
	/// <returns>Channel?</returns>
	public async Task<Channel?> GetChannelAsync(string channel_id, Sender? sender = null)
	{
		_ = BotApis.GetChannelDetail is { Path: var path, Method: var method };
		var response = await HttpSendAsync(path.Replace("{channel_id}", channel_id), method, null, sender);
		return response is null ? null : await response.Content.ReadFromJsonAsync<Channel?>();
	}

	/// <summary>
	/// 获取频道下的子频道列表
	/// </summary>
	/// <param name="guild_id">频道Id</param>
	/// <param name="channelType">筛选子频道类型</param>
	/// <param name="channelSubType">筛选子频道子类型</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<List<Channel>?> GetChannelsAsync(
		string guild_id, ChannelType? channelType = null, ChannelSubtype? channelSubType = null, Sender? sender = null)
	{
		_ = BotApis.GetChannelsInGuild is { Path: var path, Method: var method };
		var response = await HttpSendAsync(path.Replace("{guild_id}", guild_id), method, null, sender);
		var channels = response is null ? null : await response.Content.ReadFromJsonAsync<List<Channel>?>();
		if (channels is not null)
		{
			if (channelType is not null)
			{
				channels = (
					from channel in channels
					where channel.Type == channelType
					select channel
				).ToList();
			}

			if (channelSubType is not null)
			{
				channels = (
					from channel in channels
					where channel.Subtype == channelSubType
					select channel
				).ToList();
			}
		}

		return channels;
	}

	/// <summary>
	/// 创建子频道（仅私域可用）
	/// </summary>
	/// <param name="channel">用于创建子频道的对象（需提前填充必要字段）</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<Channel?> CreateChannelAsync(Channel channel, Sender? sender = null)
	{
		_ = BotApis.CreateChannel is { Path: var path, Method: var method };
		var response = await HttpSendAsync(
			path.Replace("{guild_id}", channel.GuildId),
			method,
			JsonContent.Create(channel),
			sender
		);

		return response is null ? null : await response.Content.ReadFromJsonAsync<Channel?>();
	}

	/// <summary>
	/// 修改子频道（仅私域可用）
	/// </summary>
	/// <param name="channel">修改属性后的子频道对象</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<Channel?> EditChannelAsync(Channel channel, Sender? sender = null)
	{
		_ = BotApis.ModifyChannel is { Path: var path, Method: var method };
		var response = await HttpSendAsync(
			path.Replace("{channel_id}", channel.Id),
			method,
			JsonContent.Create(channel),
			sender
		);

		return response is null ? null : await response.Content.ReadFromJsonAsync<Channel?>();
	}

	/// <summary>
	/// 删除指定子频道（仅私域可用）
	/// </summary>
	/// <param name="channel_id">要删除的子频道Id</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<bool> DeleteChannelAsync(string channel_id, Sender? sender = null)
	{
		_ = BotApis.DeleteChannel is { Path: var path, Method: var method };
		var response = await HttpSendAsync(path.Replace("{channel_id}", channel_id), method, null, sender);
		return response?.IsSuccessStatusCode ?? false;
	}
}
