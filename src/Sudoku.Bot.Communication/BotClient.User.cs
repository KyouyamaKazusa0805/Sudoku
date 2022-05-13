namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// Gets the bot information.
	/// </summary>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>
	/// A task instance encapsulates the bot information that is encapsulated by a <see cref="User"/> instance.
	/// </returns>
	public async Task<User?> GetMeAsync(Sender? sender)
		=> BotApis.GetUserDetail is { Path: var path, Method: var method }
		&& await HttpSendAsync(path, method, null, sender) is { Content: var responseContent }
			? await responseContent.ReadFromJsonAsync<User?>()
			: null;

	/// <summary>
	/// Gets all GUILDs in which the current bot has joined.
	/// </summary>
	/// <param name="guild_id">The GUILD. <b>The argument cannot be renamed.</b></param>
	/// <param name="route">
	/// Indicates the direction indicating whether the data dragging is forwarding or backwarding.
	/// <see langword="true"/> is for forwarding and <see langword="false"/> is for backwarding.
	/// </param>
	/// <param name="limit">Indicates the number of records to be displayed. The general case is 100.</param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>A task instance encapsulates a list of <see cref="Guild"/>s as the result value.</returns>
	public async Task<List<Guild>?> GetMeGuildsAsync(string? guild_id, bool route, int limit, Sender? sender)
	{
		guild_id = string.IsNullOrWhiteSpace(guild_id) ? string.Empty : $"&{(route ? "before" : "after")}={guild_id}";
		return
			BotApis.GetUserJoinedGuilds is { Path: var path, Method: var method }
			&& $"{path}?limit={limit}{guild_id}" is var resultPath
			&& await HttpSendAsync(resultPath, method, null, sender) is { Content: var responseContent }
				? await responseContent.ReadFromJsonAsync<List<Guild>?>()
				: null;
	}
}
