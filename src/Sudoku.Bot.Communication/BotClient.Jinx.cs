namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// Jinxes the specified GUILD, to make all members in this GUILD be mute.
	/// </summary>
	/// <param name="guild_id">The GUILD ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="muteTimeSpan">Indicates the time span that describes the time duration of the jinx operation.</param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>
	/// A task instance encapsulates the <see cref="bool"/> value indicating whether the operation is successful,
	/// as the result value.
	/// </returns>
	public async Task<bool> JinxGuildAsync(string guild_id, JinxTimeSpan muteTimeSpan, Sender? sender)
		=> BotApis.JinxAllMembersInGuild is { Path: var path, Method: var method }
		&& path.ReplaceArgument(guild_id) is var replacedPath
		&& JsonContent.Create(muteTimeSpan) is var jsonContent
		&& ((await HttpSendAsync(replacedPath, method, jsonContent, sender))?.IsSuccessStatusCode ?? false);

	/// <summary>
	/// Jinxes the specified member in the specified GUILD.
	/// </summary>
	/// <param name="guild_id">The GUILD ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="user_id">The user ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="muteTimeSpan">Indicates the time span that describes the time duration of the jinx operation.</param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>
	/// A task instance encapsulates the <see cref="bool"/> value indicating whether the operation is successful,
	/// as the result value.
	/// </returns>
	public async Task<bool> JinxMemberAsync(string guild_id, string user_id, JinxTimeSpan muteTimeSpan, Sender? sender)
		=> BotApis.JinxMemberInGuild is { Path: var path, Method: var method }
		&& path.ReplaceArgument(guild_id).ReplaceArgument(user_id) is var replacedPath
		&& JsonContent.Create(muteTimeSpan) is var jsonContent
		&& ((await HttpSendAsync(replacedPath, method, jsonContent, sender))?.IsSuccessStatusCode ?? false);
}
