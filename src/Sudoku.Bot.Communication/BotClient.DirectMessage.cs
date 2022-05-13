namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// Creates a direct message communication.
	/// </summary>
	/// <param name="recipientId">Indicates the recipient user ID.</param>
	/// <param name="sourceGuildId">The source GUILD ID.</param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>A task instance encapsulates the direct message source instance as the result value.</returns>
	public async Task<DirectMessageSource?> CreateDirectMessageAsync(string recipientId, string sourceGuildId, Sender sender)
		=> BotApis.CreateDirectMessageInGuild is { Path: var path, Method: var method }
		&& JsonContent.Create(new { recipient_id = recipientId, source_guild_id = sourceGuildId }) is var jsonContent
		&& await HttpSendAsync(path, method, jsonContent, sender) is { Content: var responseContent }
			? await responseContent.ReadFromJsonAsync<DirectMessageSource?>()
			: null;

	/// <summary>
	/// Sends the direct message. The method can only be called if the direct message communication
	/// has already been created.
	/// </summary>
	/// <param name="guild_id">The GUILD ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="message">The message to be sent.</param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>A task instance encapsulates the message instance as the result value.</returns>
	public async Task<Message?> SendPrivateMessageAsync(string guild_id, MessageToCreate message, Sender? sender)
		=> LastMessage(
			BotApis.SendDirectMessageInGuild is { Path: var path, Method: var method }
				&& path.ReplaceArgument(guild_id) is var replacedPath
				&& JsonContent.Create(message) is var jsonContent
				&& await HttpSendAsync(replacedPath, method, jsonContent, sender) is { Content: var responseContent }
				? await responseContent.ReadFromJsonAsync<Message?>()
				: null,
			true
		);
}
