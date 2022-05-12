namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// Gets the details for a GUILD.
	/// </summary>
	/// <param name="guild_id">The GUILD ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>A task encapsulates a <see cref="Guild"/> instance as the result value.</returns>
	public async Task<Guild?> GetGuildAsync(string guild_id, Sender? sender)
		=> BotApis.GetGuildDetail is { Path: var path, Method: var method }
		&& path.ReplaceArgument(guild_id) is var replacedPath
		&& await HttpSendAsync(replacedPath, method, null, sender) is { Content: var responseContent }
			? await responseContent.ReadFromJsonAsync<Guild?>()
			: null;
}
