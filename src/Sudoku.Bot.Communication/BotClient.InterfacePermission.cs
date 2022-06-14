namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// Gets the permissions available in the current GUILD.
	/// </summary>
	/// <param name="guild_id">The GUILD. <b>The argument cannot be renamed.</b></param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>
	/// A task instance encapsulates a list of <see cref="ApiPermission"/> instances as the result value.
	/// </returns>
	public async Task<List<ApiPermission>?> GetGuildPermissionsAsync(string guild_id, Sender? sender)
		=> (
			BotApis.GetAvailablePermissionsInGuild is { Path: var path, Method: var method }
			&& path.ReplaceArgument(guild_id) is var replacedPath
			&& await HttpSendAsync(replacedPath, method, null, sender) is { Content: var responseContent }
				? await responseContent.ReadFromJsonAsync<ApiPermissions?>()
				: null
		)?.List;

	/// <summary>
	/// Creates the API permission authorization link.
	/// </summary>
	/// <param name="guild_id">The GUILD. <b>The argument cannot be renamed.</b></param>
	/// <param name="channel_id">The channel. <b>The argument cannot be renamed.</b></param>
	/// <param name="api_identify">The API identify. <b>The argument cannot be renamed.</b></param>
	/// <param name="description">The description for the API demand.</param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>
	/// A task instance that encapsulates the <see cref="ApiPermissionDemand"/> instance as the result value.
	/// </returns>
	public async Task<ApiPermissionDemand?> SendPermissionDemandAsync(
		string guild_id, string channel_id, ApiPermissionDemandIdentify api_identify,
		string description, Sender? sender)
		=> BotApis.CreatePermissionsAuthorizationLinkInGuild is { Path: var path, Method: var method }
		&& path.ReplaceArgument(guild_id) is var replacedPath
		&& JsonContent.Create(new { channel_id, api_identify, desc = description }) is var jsonContent
		&& await HttpSendAsync(replacedPath, method, jsonContent, sender) is { Content: var responseContent }
			? await responseContent.ReadFromJsonAsync<ApiPermissionDemand?>()
			: null;
}
