namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// Gets the details for a channel.
	/// </summary>
	/// <param name="channel_id">The channel ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>A task encapsulates the channel instance as the result value.</returns>
	public async Task<Channel?> GetChannelAsync(string channel_id, Sender? sender)
		=> BotApis.GetChannelDetail is { Path: var path, Method: var method }
		&& path.ReplaceArgument(channel_id) is var replacedPath
		&& await HttpSendAsync(replacedPath, method, null, sender) is { Content: var responseContent }
			? await responseContent.ReadFromJsonAsync<Channel?>()
			: null;

	/// <summary>
	/// Gets a list of channels in the specified GUILD.
	/// </summary>
	/// <param name="guild_id">The GUILD ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="type">The type of the channel.</param>
	/// <param name="subtype">The sub type of the channel.</param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>A task instance encapsulates the channel list as the result value.</returns>
	public async Task<List<Channel>?> GetChannelsAsync(string guild_id, ChannelType? type, ChannelSubtype? subtype, Sender? sender)
		=> (
			BotApis.GetChannelsInGuild is { Path: var path, Method: var method }
				&& path.ReplaceArgument(guild_id) is var replacedPath
				&& await HttpSendAsync(replacedPath, method, null, sender) is { Content: var responseContent }
				? await responseContent.ReadFromJsonAsync<List<Channel>?>()
				: null
		)?.Where(e => e.Type == type)?.Where(e => e.Subtype == subtype)?.ToList();

	/// <summary>
	/// Creates a channel. <b>The method can only be used for the private domain.</b>
	/// </summary>
	/// <param name="channel">Indicates the channel instance that is filled with the data you want to create.</param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>A task instance encapsulates the channel instance as the result.</returns>
	public async Task<Channel?> CreateChannelAsync(Channel channel, Sender? sender)
		=> BotApis.CreateChannel is { Path: var path, Method: var method }
		&& path.Replace("{guild_id}", channel.GuildId) is var replacedPath
		&& JsonContent.Create(channel) is var jsonContent
		&& await HttpSendAsync(replacedPath, method, jsonContent, sender) is { Content: var responseContent }
			? await responseContent.ReadFromJsonAsync<Channel?>()
			: null;

	/// <summary>
	/// Edits a channel. <b>The method can only be used for the private domain.</b>
	/// </summary>
	/// <param name="channel">The channel you want to edit.</param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>A task instance encapsulates the channel instance as the result.</returns>
	public async Task<Channel?> EditChannelAsync(Channel channel, Sender? sender)
		=> BotApis.ModifyChannel is { Path: var path, Method: var method }
		&& path.Replace("{channel_id}", channel.Id) is var replacedPath
		&& JsonContent.Create(channel) is var jsonContent
		&& await HttpSendAsync(replacedPath, method, jsonContent, sender) is { Content: var responseContent }
			? await responseContent.ReadFromJsonAsync<Channel?>()
			: null;

	/// <summary>
	/// Deletes a channel. <b>The method can only be used for the private domain.</b>
	/// </summary>
	/// <param name="channel_id">The channel you want to delete. <b>The argument cannot be renamed.</b></param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>
	/// A task instance encapsulates the <see cref="bool"/> value indicating
	/// whether the deletion operation is successful.
	/// </returns>
	public async Task<bool> DeleteChannelAsync(string channel_id, Sender? sender)
		=> BotApis.DeleteChannel is { Path: var path, Method: var method }
		&& path.ReplaceArgument(channel_id) is var replacedPath
		&& ((await HttpSendAsync(replacedPath, method, null, sender))?.IsSuccessStatusCode ?? false);
}
