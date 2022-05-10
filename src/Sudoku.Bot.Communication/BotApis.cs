namespace Sudoku.Bot.Communication;

/// <summary>
/// Provides with a set of APIs.
/// </summary>
internal static class BotApis
{
	/// <summary>
	/// Gets the details of the bot.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/user/me.html">here</see>.</item>
	/// <item>Corresponding: <c>GET /users/@me</c></item>
	/// <item>Need authorization: false</item>
	/// </list>
	/// </summary>
	public static BotApi GetUserDetail => new(ApiType.Both, HttpMethod.Get, """/users/@me""");

	/// <summary>
	/// Gets the list of GUILDs where the current user has joined.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/user/guilds.html">here</see>.</item>
	/// <item>Corresponding: <c>GET /users/@me/guilds</c></item>
	/// <item>Need authorization: false</item>
	/// </list>
	/// </summary>
	public static BotApi GetUserJoinedGuilds => new(ApiType.Both, HttpMethod.Get, """/users/@me/guilds""");

	/// <summary>
	/// Gets the details of the specified GUILD.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/guild/get_guild.html">here</see>.</item>
	/// <item>Corresponding: <c>GET /guilds/{guild_id}</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi GetGuildDetail => new(ApiType.PublicDomain, HttpMethod.Get, """/guilds/{guild_id}""");

	/// <summary>
	/// Gets the list of channels in the specified GUILD.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel/get_channels.html">here</see>.</item>
	/// <item>Corresponding: <c>GET /guilds/{guild_id}/channels</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi GetChannelsInGuild
		=> new(ApiType.PublicDomain, HttpMethod.Get, """/guilds/{guild_id}/channels""");

	/// <summary>
	/// Gets the detail of a channel.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel/get_channel.html">here</see>.</item>
	/// <item>Corresponding: <c>GET /guilds/{guild_id}/channels</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi GetChannelDetail => new(ApiType.PublicDomain, HttpMethod.Get, """/channels/{channel_id}""");

	/// <summary>
	/// Creates a channel in the specified GUILD.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel/post_channels.html">here</see>.</item>
	/// <item>Corresponding: <c>POST /guilds/{guild_id}/channels</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PrivateDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi CreateChannel => new(ApiType.PrivateDomain, HttpMethod.Post, """/guilds/{guild_id}/channels""");

	/// <summary>
	/// Modify a channel.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel/patch_channel.html">here</see>.</item>
	/// <item>Corresponding: <c>PATCH /channels/{channel_id}</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PrivateDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi ModifyChannel => new(ApiType.PrivateDomain, HttpMethod.Patch, """/channels/{channel_id}""");

	/// <summary>
	/// Delete a channel.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel/delete_channel.html">here</see>.</item>
	/// <item>Corresponding: <c>DELETE /channels/{channel_id}</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PrivateDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi DeleteChannel => new(ApiType.PrivateDomain, HttpMethod.Delete, """/channels/{channel_id}""");

	/// <summary>
	/// Get members joined in the specified GUILD.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/member/get_members.html">here</see>.</item>
	/// <item>Corresponding: <c>GET /guilds/{guild_id}/members</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PrivateDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi GetMembersInGuild
		=> new(ApiType.PrivateDomain, HttpMethod.Get, """/guilds/{guild_id}/members""");

	/// <summary>
	/// Get detail of the specified member in the specified GUILD.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/member/get_member.html">here</see>.</item>
	/// <item>Corresponding: <c>GET /guilds/{guild_id}/members/{user_id}</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi GetMemberDetailInGuild
		=> new(ApiType.PublicDomain, HttpMethod.Get, """/guilds/{guild_id}/members/{user_id}""");

	/// <summary>
	/// Delete the specified member from the specified GUILD.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/member/delete_member.html">here</see>.</item>
	/// <item>Corresponding: <c>DELETE /guilds/{guild_id}/members/{user_id}</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PrivateDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi DeleteMemberInGuild
		=> new(ApiType.PrivateDomain, HttpMethod.Delete, """/guilds/{guild_id}/members/{user_id}""");

	/// <summary>
	/// Get the list of roles existed in the specified GUILD.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/guild/get_guild_roles.html">here</see>.</item>
	/// <item>Corresponding: <c>GET /guilds/{guild_id}/roles</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi GetRolesInGuild => new(ApiType.PublicDomain, HttpMethod.Get, """/guilds/{guild_id}/roles""");

	/// <summary>
	/// Create a role in the specified GUILD.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/guild/post_guild_role.html">here</see>.</item>
	/// <item>Corresponding: <c>POST /guilds/{guild_id}/roles</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi CreateRoleInGuild => new(ApiType.PublicDomain, HttpMethod.Post, """/guilds/{guild_id}/roles""");

	/// <summary>
	/// Modify the specified role in the specified GUILD.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/guild/patch_guild_role.html">here</see>.</item>
	/// <item>Corresponding: <c>PATCH /guilds/{guild_id}/roles/{role_id}</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi ModifyRoleInGuild
		=> new(ApiType.PublicDomain, HttpMethod.Patch, """/guilds/{guild_id}/roles/{role_id}""");

	/// <summary>
	/// Delete the specified role from the specified GUILD.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/guild/delete_guild_role.html">here</see>.</item>
	/// <item>Corresponding: <c>DELETE /guilds/{guild_id}/roles/{role_id}</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi DeleteRoleInGuild
		=> new(ApiType.PublicDomain, HttpMethod.Delete, """/guilds/{guild_id}/roles/{role_id}""");

	/// <summary>
	/// Adds the specified user into the specified role in the specified GUILD.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/guild/put_guild_member_role.html">here</see>.</item>
	/// <item>Corresponding: <c>PUT /guilds/{guild_id}/members/{user_id}/roles/{role_id}</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi AddUserRoleInGuild
		=> new(ApiType.PublicDomain, HttpMethod.Put, """/guilds/{guild_id}/members/{user_id}/roles/{role_id}""");

	/// <summary>
	/// Delete the specified user from the specified role in the specified GUILD.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/guild/delete_guild_member_role.html">here</see>.</item>
	/// <item>Corresponding: <c>DELETE /guilds/{guild_id}/members/{user_id}/roles/{role_id}</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi DeleteUserRoleInGuild
		=> new(ApiType.PublicDomain, HttpMethod.Delete, """/guilds/{guild_id}/members/{user_id}/roles/{role_id}""");

	/// <summary>
	/// Get permissions for the specified user in the specified channel.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel_permissions/get_channel_permissions.html">here</see>.</item>
	/// <item>Corresponding: <c>GET /channels/{channel_id}/members/{user_id}/permissions</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi GetUserPermissionInChannel
		=> new(ApiType.PublicDomain, HttpMethod.Get, """/channels/{channel_id}/members/{user_id}/permissions""");

	/// <summary>
	/// Modify permissions for the specified user in the specified channel.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel_permissions/put_channel_permissions.html">here</see>.</item>
	/// <item>Corresponding: <c>PUT /channels/{channel_id}/members/{user_id}/permissions</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi ModifyUserPermissionInChannel
		=> new(ApiType.PublicDomain, HttpMethod.Put, """/channels/{channel_id}/members/{user_id}/permissions""");

	/// <summary>
	/// Get permissions for the specified role in the specified channel.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel_permissions/get_channel_roles_permissions.html">here</see>.</item>
	/// <item>Corresponding: <c>GET /channels/{channel_id}/roles/{role_id}/permissions</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi GetRolePermissionInChannel
		=> new(ApiType.PublicDomain, HttpMethod.Get, """/channels/{channel_id}/roles/{role_id}/permissions""");

	/// <summary>
	/// Modify permissions for the specified role in the specified channel.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel_permissions/put_channel_roles_permissions.html">here</see>.</item>
	/// <item>Corresponding: <c>PUT /channels/{channel_id}/roles/{role_id}/permissions</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi ModifyRolePermissionInChannel
		=> new(ApiType.PublicDomain, HttpMethod.Put, """/channels/{channel_id}/roles/{role_id}/permissions""");

	/// <summary>
	/// Gets the specified message via its ID, in the specified channel.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/message/get_message_of_id.html">here</see>.</item>
	/// <item>Corresponding: <c>GET /channels/{channel_id}/messages/{message_id}</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi GetMessageInChannel
		=> new(ApiType.PublicDomain, HttpMethod.Get, """/channels/{channel_id}/messages/{message_id}""");

	/// <summary>
	/// Gets the message list in the specified channel.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/pythonsdk/api/message/get_messages.html">here</see>.</item>
	/// <item>Corresponding: <c>GET /channels/{channel_id}/messages</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PrivateDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi GetMessagesInChannel
		=> new(ApiType.PrivateDomain, HttpMethod.Get, """/channels/{channel_id}/messages""");

	/// <summary>
	/// Send the message to the specified channel.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/message/post_messages.html">here</see>.</item>
	/// <item>Corresponding: <c>POST /channels/{channel_id}/messages</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi SendMessageToChannel
		=> new(ApiType.PublicDomain, HttpMethod.Post, """/channels/{channel_id}/messages""");

	/// <summary>
	/// Recall the specified message in the specified channel.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/nodesdk/message/delete_message.html">here</see>.</item>
	/// <item>Corresponding: <c>DELETE /channels/{channel_id}/messages/{message_id}</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PrivateDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi RecallMessageInChannel
		=> new(ApiType.PrivateDomain, HttpMethod.Delete, """/channels/{channel_id}/messages/{message_id}""");

	/// <summary>
	/// Creates a direct message with a member in the same GUILD with the bot.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/dms/post_dms.html">here</see>.</item>
	/// <item>Corresponding: <c>POST /users/@me/dms</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi CreateDirectMessageInGuild => new(ApiType.PublicDomain, HttpMethod.Post, """/users/@me/dms""");

	/// <summary>
	/// Sends a direct message in the specified GUILD if the environment (direct message room) has been created.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/dms/post_dms_messages.html">here</see>.</item>
	/// <item>Corresponding: <c>POST /dms/{guild_id}/messages</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi SendDirectMessageInGuild =>
		new(ApiType.PublicDomain, HttpMethod.Post, """/dms/{guild_id}/messages""");

	/// <summary>
	/// Jinx all members in the specified GUILD.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/guild/patch_guild_mute.html">here</see>.</item>
	/// <item>Corresponding: <c>PATCH /guilds/{guild_id}/mute</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi JinxAllMembersInGuild
		=> new(ApiType.PublicDomain, HttpMethod.Patch, """/guilds/{guild_id}/mute""");

	/// <summary>
	/// Jinx the specified member in the specified GUILD.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/guild/patch_guild_member_mute.html">here</see>.</item>
	/// <item>Corresponding: <c>PATCH /guilds/{guild_id}/members/{user_id}/mute</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi JinxMemberInGuild
		=> new(ApiType.PublicDomain, HttpMethod.Patch, """/guilds/{guild_id}/members/{user_id}/mute""");

	/// <summary>
	/// Creates an announcement in the specified GUILD.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/announces/post_guild_announces.html">here</see>.</item>
	/// <item>Corresponding: <c>POST /guilds/{guild_id}/announces</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi CreateAnnouncementInGuild
		=> new(ApiType.PublicDomain, HttpMethod.Post, """/guilds/{guild_id}/announces""");

	/// <summary>
	/// Delete the specified announcement in the specified GUILD.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/announces/delete_guild_announces.html">here</see>.</item>
	/// <item>Corresponding: <c>DELETE /guilds/{guild_id}/announces/{message_id}</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi DeleteAnnouncementInGuild
		=> new(ApiType.PublicDomain, HttpMethod.Delete, """/guilds/{guild_id}/announces/{message_id}""");

	/// <summary>
	/// Creates an announcement in the specified channel.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/announces/post_channel_announces.html">here</see>.</item>
	/// <item>Corresponding: <c>POST /channels/{channel_id}/announces</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi CreateAnnouncementInChannel
		=> new(ApiType.PublicDomain, HttpMethod.Post, """/channels/{channel_id}/announces""");

	/// <summary>
	/// Delete the specified announcement in the specified channel.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/announces/delete_channel_announces.html">here</see>.</item>
	/// <item>Corresponding: <c>DELETE /channels/{channel_id}/announces/{message_id}</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi DeleteAnnouncementInChannel
		=> new(ApiType.PublicDomain, HttpMethod.Delete, """/channels/{channel_id}/announces/{message_id}""");

	/// <summary>
	/// Gets the schedule list in the specified channel.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/schedule/get_schedules.html">here</see>.</item>
	/// <item>Corresponding: <c>GET /channels/{channel_id}/schedules</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi GetSchedulesInChannel
		=> new(ApiType.PublicDomain, HttpMethod.Get, """/channels/{channel_id}/schedules""");

	/// <summary>
	/// Gte the details of the specified schedule in the specified channel.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/schedule/get_schedule.html">here</see>.</item>
	/// <item>Corresponding: <c>GET /channels/{channel_id}/schedules/{schedule_id}</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi GetScheduleInChannel
		=> new(ApiType.PublicDomain, HttpMethod.Get, """/channels/{channel_id}/schedules/{schedule_id}""");

	/// <summary>
	/// Creates a schedule instance in the specified channel.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/schedule/post_schedule.html">here</see>.</item>
	/// <item>Corresponding: <c>POST /channels/{channel_id}/schedules</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi CreateScheduleInChannel
		=> new(ApiType.PublicDomain, HttpMethod.Post, """/channels/{channel_id}/schedules""");

	/// <summary>
	/// Modify the specified schedule instance in the specified channel.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/schedule/patch_schedule.html">here</see>.</item>
	/// <item>Corresponding: <c>PATCH /channels/{channel_id}/schedules/{schedule_id}</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi ModifyScheduleInChannel
		=> new(ApiType.PublicDomain, HttpMethod.Patch, """/channels/{channel_id}/schedules/{schedule_id}""");

	/// <summary>
	/// Delete the specified schedule in the specified channel.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/schedule/delete_schedule.html">here</see>.</item>
	/// <item>Corresponding: <c>DELETE /channels/{channel_id}/schedules/{schedule_id}</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi DeleteScheduleInChannel
		=> new(ApiType.PublicDomain, HttpMethod.Delete, """/channels/{channel_id}/schedules/{schedule_id}""");

	/// <summary>
	/// 控制子频道 channel_id 下的音频
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/audio/audio_control.html">接口文档</see><br/>
	/// 公域鉴权<br/>
	/// POST /channels/{channel_id}/audio
	/// </para>
	/// </summary>
	public static BotApi 音频控制 => new(ApiType.PublicDomain, HttpMethod.Post, @"/channels/{channel_id}/audio");

	/// <summary>
	/// 获取机器人在频道 guild_id 内可以使用的权限列表
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/api_permissions/get_guild_api_permission.html">接口文档</see><br/>
	/// 无需鉴权<br/>
	/// GET /guilds/{guild_id}/api_permission
	/// </para>
	/// </summary>
	public static BotApi 获取频道可用权限列表 => new(ApiType.Both, HttpMethod.Get, @"/guilds/{guild_id}/api_permission");

	/// <summary>
	/// 创建 API 接口权限授权链接，该链接指向 guild_id 对应的频道
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/api_permissions/post_api_permission_demand.html">接口文档</see><br/>
	/// 无需鉴权<br/>
	/// POST /guilds/{guild_id}/api_permission/demand
	/// </para>
	/// </summary>
	public static BotApi 创建频道接口授权链接 => new(ApiType.Both, HttpMethod.Post, @"/guilds/{guild_id}/api_permission/demand");
}
