namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// Gets all permissions for the specified user in the specified channel.
	/// </summary>
	/// <param name="channel_id">The channel ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="user_id">The user ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>A task instance encapsulates the <see cref="ChannelPermissions"/> instance as the result value.</returns>
	public async Task<ChannelPermissions?> GetChannelPermissionsAsync(string channel_id, string user_id, Sender? sender)
		=> BotApis.GetUserPermissionInChannel is { Path: var path, Method: var method }
		&& path.ReplaceArgument(channel_id).ReplaceArgument(user_id) is var replacedPath
		&& await HttpSendAsync(replacedPath, method, null, sender) is { Content: var responseContent }
			? await responseContent.ReadFromJsonAsync<ChannelPermissions?>()
			: null;

	/// <summary>
	/// Modify (add or remove) the permission that is for the specified user in the specified channel.
	/// </summary>
	/// <param name="channel_id">The channel ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="user_id">The user ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="permissionsToBeAdded">
	/// Indicates the permissions you want to be added. The value is the string representation of an integer,
	/// as the eigenvalue for the enumeration type <see cref="PrivacyType"/>.
	/// </param>
	/// <param name="permissionsToBeRemoved">
	/// Indicates the permissions you want to be removed. The value is the string representation of an integer,
	/// as the eigenvalue for the enumeration type <see cref="PrivacyType"/>.
	/// </param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>
	/// A task instance encapsulates a <see cref="bool"/> value indicating whether the operation is successful.
	/// </returns>
	/// <seealso cref="PrivacyType"/>
	public async Task<bool> EditChannelPermissionsAsync(
		string channel_id, string user_id, string permissionsToBeAdded, string permissionsToBeRemoved, Sender? sender)
		=> BotApis.ModifyUserPermissionInChannel is { Path: var path, Method: var method }
		&& path.ReplaceArgument(channel_id).ReplaceArgument(user_id) is var replacedPath
		&& JsonContent.Create(new { add = permissionsToBeAdded, remove = permissionsToBeRemoved }) is var jsonContent
		&& ((await HttpSendAsync(replacedPath, method, jsonContent, sender))?.IsSuccessStatusCode ?? false);

	/// <summary>
	/// Gets all permissions being allowed for the specified role in the specified channel.
	/// </summary>
	/// <param name="channel_id">The channel ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="role_id">The role ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>A task instance encapsulates the <see cref="ChannelPermissions"/> instance as the result value.</returns>
	public async Task<ChannelPermissions?> GetRolePermissionsInChannelAsync(
		string channel_id, string role_id, Sender? sender)
		=> BotApis.GetRolePermissionInChannel is { Path: var path, Method: var method }
		&& path.ReplaceArgument(channel_id).ReplaceArgument(role_id) is var replacedPath
		&& await HttpSendAsync(replacedPath, method, null, sender) is { Content: var responseContent }
			? await responseContent.ReadFromJsonAsync<ChannelPermissions?>()
			: null;

	/// <summary>
	/// Modify (add or remove) the permission that is for the specified role in the specified channel.
	/// </summary>
	/// <param name="channel_id">The channel ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="role_id">The role ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="permissionsToBeAdded">
	/// Indicates the permissions you want to be added. The value is the string representation of an integer,
	/// as the eigenvalue for the enumeration type <see cref="PrivacyType"/>.
	/// </param>
	/// <param name="permissionsToBeRemoved">
	/// Indicates the permissions you want to be removed. The value is the string representation of an integer,
	/// as the eigenvalue for the enumeration type <see cref="PrivacyType"/>.
	/// </param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>
	/// A task instance encapsulates a <see cref="bool"/> value indicating whether the operation is successful.
	/// </returns>
	/// <seealso cref="PrivacyType"/>
	public async Task<bool> EditRolePermissionsInChannelAsync(
		string channel_id, string role_id, string permissionsToBeAdded, string permissionsToBeRemoved, Sender? sender)
		=> BotApis.ModifyRolePermissionInChannel is { Path: var path, Method: var method }
		&& path.ReplaceArgument(channel_id).ReplaceArgument(role_id) is var replacedPath
		&& JsonContent.Create(new { add = permissionsToBeAdded, remove = permissionsToBeRemoved }) is var jsonContent
		&& ((await HttpSendAsync(replacedPath, method, jsonContent, sender))?.IsSuccessStatusCode ?? false);
}
