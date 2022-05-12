namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// Creates a global announcement.
	/// </summary>
	/// <param name="guild_id">The GUILD ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="channel_id">The channel ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="message_id">The message ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>
	/// The task encapsulates the <see cref="Announces"/> instance as the result value
	/// after being <see langword="await"/>ed.
	/// </returns>
	public async Task<Announces?> CreateAnnouncesGlobalAsync(
		string guild_id, string channel_id, string message_id, Sender? sender)
		=> BotApis.CreateAnnouncementInGuild is { Path: var path, Method: var method }
		&& path.ReplaceArgument(guild_id) is var replacedPath
		&& JsonContent.Create(new { channel_id, message_id }) is var jsonContent
		&& await HttpSendAsync(replacedPath, method, jsonContent, sender) is { Content: var responseContent }
			? await responseContent.ReadFromJsonAsync<Announces?>()
			: null;

	/// <summary>
	/// Deletes a global announcement.
	/// </summary>
	/// <param name="guild_id">The GUILD ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="message_id">The message ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>
	/// The task encapsulates the <see cref="bool"/> value, indicating whether the operation is successful,
	/// after being <see langword="await"/>ed.
	/// </returns>
	public async Task<bool> DeleteAnnouncesGlobalAsync(string guild_id, string message_id, Sender? sender)
		=> BotApis.DeleteAnnouncementInGuild is { Path: var path, Method: var method }
		&& path.ReplaceArgument(guild_id).ReplaceArgument(message_id) is var replacedPath
		&& ((await HttpSendAsync(replacedPath, method, null, sender))?.IsSuccessStatusCode ?? false);

	/// <summary>
	/// Creates an announcement in a channel.
	/// </summary>
	/// <param name="channel_id">The channel ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="message_id">The message ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>
	/// The task encapsulates the <see cref="Announces"/> instance as the result value
	/// after being <see langword="await"/>ed.
	/// </returns>
	public async Task<Announces?> CreateAnnouncesAsync(string channel_id, string message_id, Sender? sender)
		=> BotApis.CreateAnnouncementInChannel is { Path: var path, Method: var method }
		&& path.ReplaceArgument(channel_id) is var replacedPath
		&& channel_id is null ? null : JsonContent.Create(new { message_id }) is var jsonContent
		&& await HttpSendAsync(replacedPath, method, jsonContent, sender) is { Content: var responseContent }
			? await responseContent.ReadFromJsonAsync<Announces?>()
			: null;

	/// <summary>
	/// Deletes an announcement in a channel.
	/// </summary>
	/// <param name="channel_id">The channel ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="message_id">The message ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>
	/// The task encapsulates the <see cref="bool"/> value, indicating whether the operation is successful,
	/// after being <see langword="await"/>ed.
	/// </returns>
	public async Task<bool> DeleteAnnouncesAsync(string channel_id, string message_id, Sender? sender)
		=> BotApis.DeleteAnnouncementInChannel is { Path: var path, Method: var method }
		&& path.ReplaceArgument(channel_id).ReplaceArgument(message_id) is var replacedPath
		&& ((await HttpSendAsync(replacedPath, method, null, sender))?.IsSuccessStatusCode ?? false);
}
