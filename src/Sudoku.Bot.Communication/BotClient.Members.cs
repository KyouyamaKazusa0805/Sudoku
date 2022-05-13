namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// Gets all members joined in the specified GUILD.
	/// </summary>
	/// <param name="guild_id">The GUILD. <b>The argument cannot be renamed.</b></param>
	/// <param name="limit">
	/// Indicates the number of elements being displayed in this group. General value is 10.
	/// The supported range is [0, 1000].
	/// </param>
	/// <param name="after">
	/// Indicates the last member ID. If the first calling, pass the argument value with <c>"0"</c>.
	/// If <see langword="null"/>, <c>"0"</c> will be replaced with the original value.
	/// </param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>A task instance encapsulates a list of <see cref="Member"/>s as the result value.</returns>
	public async Task<List<Member>?> GetGuildMembersAsync(string guild_id, int limit, string? after, Sender? sender)
		=> BotApis.GetMembersInGuild is { Path: var path, Method: var method }
		&& $"{path.ReplaceArgument(guild_id)}?limit={limit}&after={after ?? "0"}" is var resultPath
		&& await HttpSendAsync(resultPath, method, null, sender) is { Content: var responseContent }
			? await responseContent.ReadFromJsonAsync<List<Member>?>()
			: null;

	/// <summary>
	/// Gets the user information joined in the specified GUILD.
	/// </summary>
	/// <param name="guild_id">The GUILD. <b>The argument cannot be renamed.</b></param>
	/// <param name="user_id">The user. <b>The argument cannot be renamed.</b></param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>A task instance encapsulates the member instance as the result.</returns>
	public async Task<Member?> GetMemberAsync(string guild_id, string user_id, Sender? sender)
		=> BotApis.GetMemberDetailInGuild is { Path: var path, Method: var method }
		&& path.ReplaceArgument(guild_id).ReplaceArgument(user_id) is var replacedPath
		&& await HttpSendAsync(replacedPath, method, null, sender) is { Content: var responseContent }
			? await responseContent.ReadFromJsonAsync<Member?>()
			: null;

	/// <summary>
	/// Deletes the specified member in the specified GUILD.
	/// </summary>
	/// <param name="guild_id">The GUILD. <b>The argument cannot be renamed.</b></param>
	/// <param name="user_id">The user. <b>The argument cannot be renamed.</b></param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>
	/// A task instance encapsulates a <see cref="bool"/> value indicating whether the operation is successful.
	/// </returns>
	public async Task<bool> DeleteGuildMemberAsync(string guild_id, string user_id, Sender? sender)
		=> BotApis.DeleteMemberInGuild is { Path: var path, Method: var method }
		&& path.ReplaceArgument(guild_id).ReplaceArgument(user_id) is var replacedPath
		&& ((await HttpSendAsync(replacedPath, method, null, sender))?.IsSuccessStatusCode ?? false);
}
