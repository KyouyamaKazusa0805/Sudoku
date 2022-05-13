namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// Gets all roles in the specified GUILD.
	/// </summary>
	/// <param name="guild_id">The GUILD. <b>The argument cannot be renamed.</b></param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>A task instance encapsulates the list of roles as the result value.</returns>
	public async Task<List<Role>?> GetRolesAsync(string guild_id, Sender? sender)
		=> (
			BotApis.GetRolesInGuild is { Path: var path, Method: var method }
			&& path.ReplaceArgument(guild_id) is var replacedPath
			&& await HttpSendAsync(replacedPath, method, null, sender) is { Content: var responseContent }
				? await responseContent.ReadFromJsonAsync<GuildRoles?>()
				: null
		)?.Roles;

	/// <summary>
	/// Creates a role in the specified GUILD.
	/// </summary>
	/// <param name="guild_id">The GUILD. <b>The argument cannot be renamed.</b></param>
	/// <param name="info">Indicates the instance that is used for providing with extra data.</param>
	/// <param name="filter">
	/// Indicates a filter instance that ignores or allows which fields will be inferred automatically.
	/// </param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>A task instance encapsulates the role instance created.</returns>
	public async Task<Role?> CreateRoleAsync(string guild_id, Info info, Filter? filter, Sender? sender)
		=> (
			BotApis.CreateRoleInGuild is { Path: var path, Method: var method }
			&& path.ReplaceArgument(guild_id) is var replacedPath
			&& info is { Name: var name, Color: var color, Hoist: var hoist }
			&& (filter ?? new(!string.IsNullOrWhiteSpace(name), color is not null, hoist ?? false)) is var filterFinal
			&& new { filter = filterFinal, info } is var anon
			&& JsonContent.Create(anon) is var jsonContent
			&& await HttpSendAsync(replacedPath, method, jsonContent, sender) is { Content: var responseContent }
				? await responseContent.ReadFromJsonAsync<RoleCreatedResult?>()
				: null
		)?.Role;

	/// <summary>
	/// Modify the specified role in the specified GUILD.
	/// </summary>
	/// <param name="guild_id">The GUILD. <b>The argument cannot be renamed.</b></param>
	/// <param name="role_id">The role. <b>The argument cannot be renamed.</b></param>
	/// <param name="info">Indicates the instance that is used for providing with extra data.</param>
	/// <param name="filter">
	/// Indicates a filter instance that ignores or allows which fields will be inferred automatically.
	/// </param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>A task instance encapsulates the role instance modified.</returns>
	public async Task<Role?> EditRoleAsync(string guild_id, string role_id, Info info, Filter? filter, Sender? sender)
		=> (
			BotApis.ModifyRoleInGuild is { Path: var path, Method: var method }
			&& path.ReplaceArgument(guild_id).ReplaceArgument(role_id) is var replacedPath
			&& info is { Name: var name, Color: var color, Hoist: var hoist }
			&& (filter ?? new(!string.IsNullOrWhiteSpace(name), color is not null, hoist ?? false)) is var filterFinal
			&& new { filter = filterFinal, info } is var anon
			&& JsonContent.Create(anon) is var jsonContent
			&& await HttpSendAsync(replacedPath, method, jsonContent, sender) is { Content: var responseContent }
				? await responseContent.ReadFromJsonAsync<RoleModifiedResult?>()
				: null
		)?.Role;

	/// <summary>
	/// Deletes a role in the specified GUILD.
	/// </summary>
	/// <param name="guild_id">The GUILD. <b>The argument cannot be renamed.</b></param>
	/// <param name="role_id">The role. <b>The argument cannot be renamed.</b></param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>
	/// A task instance encapsulates a <see cref="bool"/> value indicating whether the operation is successful.
	/// </returns>
	public async Task<bool> DeleteRoleAsync(string guild_id, string role_id, Sender? sender)
		=> BotApis.DeleteRoleInGuild is { Path: var path, Method: var method }
		&& path.ReplaceArgument(guild_id).ReplaceArgument(role_id) is var replacedPath
		&& ((await HttpSendAsync(replacedPath, method, null, sender))?.IsSuccessStatusCode ?? false);

	/// <summary>
	/// Joins a user into the specified role in the specified channel of the specified GUILD.
	/// </summary>
	/// <param name="guild_id">The GUILD. <b>The argument cannot be renamed.</b></param>
	/// <param name="user_id">The user. <b>The argument cannot be renamed.</b></param>
	/// <param name="role_id">The role. <b>The argument cannot be renamed.</b></param>
	/// <param name="channelId">
	/// The channel. If the argument <paramref name="role_id"/> is <c>"5"</c>
	/// (i.e. a channel administrator), this argument shouldn't be <see langword="null"/>.
	/// </param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>
	/// A task instance encapsulates a <see cref="bool"/> value indicating whether the operation is successful.
	/// </returns>
	public async Task<bool> AddRoleMemberAsync(
		string guild_id, string user_id, string role_id, string? channelId, Sender? sender)
		=> BotApis.AddUserRoleInGuild is { Path: var path, Method: var method }
		&& channelId switch
		{
			not null when new { channel = new Channel { Id = channelId } } is var anon => JsonContent.Create(anon),
			_ => null
		} is var jsonContent
		&& path.ReplaceArgument(guild_id).ReplaceArgument(user_id).ReplaceArgument(role_id) is var replacedPath
		&& ((await HttpSendAsync(replacedPath, method, jsonContent, sender))?.IsSuccessStatusCode ?? false);

	/// <summary>
	/// Deletes a user from the specified role, in the specified channel in the specified GUILD.
	/// Due to the complexity of the handling, for more information please visit
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/guild/delete_guild_member_role.html">this link</see>.
	/// </summary>
	/// <param name="guild_id">The GUILD. <b>The argument cannot be renamed.</b></param>
	/// <param name="user_id">The user. <b>The argument cannot be renamed.</b></param>
	/// <param name="role_id">The role. <b>The argument cannot be renamed.</b></param>
	/// <param name="channelId">The channel ID.</param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>
	/// A task instance encapsulates a <see cref="bool"/> value indicating whether the operation is successful.
	/// </returns>
	public async Task<bool> DeleteRoleMemberAsync(
		string guild_id, string user_id, string role_id, string? channelId, Sender? sender)
		=> BotApis.DeleteUserRoleInGuild is { Path: var path, Method: var method }
		&& channelId switch
		{
			not null when new { channel = new Channel { Id = channelId } } is var anon => JsonContent.Create(anon),
			_ => null
		} is var jsonContent
		&& path.ReplaceArgument(guild_id).ReplaceArgument(user_id).ReplaceArgument(role_id) is var replacedPath
		&& ((await HttpSendAsync(replacedPath, method, jsonContent, sender))?.IsSuccessStatusCode ?? false);
}
