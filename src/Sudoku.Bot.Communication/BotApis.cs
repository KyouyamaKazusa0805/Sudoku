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
	public static BotApi GetChannelsInGuild => new(ApiType.PublicDomain, HttpMethod.Get, """/guilds/{guild_id}/channels""");

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
	public static BotApi GetMembersInGuild => new(ApiType.PrivateDomain, HttpMethod.Get, """/guilds/{guild_id}/members""");

	/// <summary>
	/// Get detail of the specified member in the specified GUILD.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/member/get_member.html">here</see>.</item>
	/// <item>Corresponding: <c>GET /guilds/{guild_id}/members/{user_id}</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi GetMemberDetailInGuild => new(ApiType.PublicDomain, HttpMethod.Get, """/guilds/{guild_id}/members/{user_id}""");

	/// <summary>
	/// Delete the specified member from the specified GUILD.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/member/delete_member.html">here</see>.</item>
	/// <item>Corresponding: <c>DELETE /guilds/{guild_id}/members/{user_id}</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PrivateDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi DeleteMemberInGuild => new(ApiType.PrivateDomain, HttpMethod.Delete, """/guilds/{guild_id}/members/{user_id}""");

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
	public static BotApi ModifyRoleInGuild => new(ApiType.PublicDomain, HttpMethod.Patch, """/guilds/{guild_id}/roles/{role_id}""");

	/// <summary>
	/// Delete the specified role from the specified GUILD.
	/// <list type="bullet">
	/// <item>Documentation: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/guild/delete_guild_role.html">here</see>.</item>
	/// <item>Corresponding: <c>DELETE /guilds/{guild_id}/roles/{role_id}</c></item>
	/// <item>Need authorization: true if <see cref="ApiType.PublicDomain"/>; otherwise false</item>
	/// </list>
	/// </summary>
	public static BotApi DeleteRoleInGuild => new(ApiType.PublicDomain, HttpMethod.Delete, """/guilds/{guild_id}/roles/{role_id}""");

	/// <summary>
	/// 将频道 guild_id 下的用户 user_id 添加到身份组 role_id
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/guild/put_guild_member_role.html">接口文档</see><br/>
	/// 公域鉴权<br/>
	/// PUT /guilds/{guild_id}/members/{user_id}/roles/{role_id}
	/// </para>
	/// </summary>
	public static BotApi 添加频道身份组成员 => new(ApiType.PublicDomain, HttpMethod.Put, @"/guilds/{guild_id}/members/{user_id}/roles/{role_id}");

	/// <summary>
	/// 将用户 user_id 从频道 guild_id 的 role_id 身份组中移除
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/guild/delete_guild_member_role.html">接口文档</see><br/>
	/// 公域鉴权<br/>
	/// DELETE /guilds/{guild_id}/members/{user_id}/roles/{role_id}
	/// </para>
	/// </summary>
	public static BotApi 删除频道身份组成员 => new(ApiType.PublicDomain, HttpMethod.Delete, @"/guilds/{guild_id}/members/{user_id}/roles/{role_id}");

	/// <summary>
	/// 获取子频道 channel_id 下用户 user_id 的权限
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel_permissions/get_channel_permissions.html">接口文档</see><br/>
	/// 公域鉴权<br/>
	/// GET /channels/{channel_id}/members/{user_id}/permissions
	/// </para>
	/// </summary>
	public static BotApi 获取子频道用户权限 => new(ApiType.PublicDomain, HttpMethod.Get, @"/channels/{channel_id}/members/{user_id}/permissions");

	/// <summary>
	/// 修改子频道 channel_id 下用户 user_id 的权限
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel_permissions/put_channel_permissions.html">接口文档</see><br/>
	/// 公域鉴权<br/>
	/// PUT /channels/{channel_id}/members/{user_id}/permissions
	/// </para>
	/// </summary>
	public static BotApi 修改子频道用户权限 => new(ApiType.PublicDomain, HttpMethod.Put, @"/channels/{channel_id}/members/{user_id}/permissions");

	/// <summary>
	/// 获取子频道 channel_id 下身份组 role_id 的权限
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel_permissions/get_channel_roles_permissions.html">接口文档</see><br/>
	/// 公域鉴权<br/>
	/// GET /channels/{channel_id}/roles/{role_id}/permissions
	/// </para>
	/// </summary>
	public static BotApi 获取子频道身份组权限 => new(ApiType.PublicDomain, HttpMethod.Get, @"/channels/{channel_id}/roles/{role_id}/permissions");

	/// <summary>
	/// 修改子频道 channel_id 下身份组 role_id 的权限
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel_permissions/put_channel_roles_permissions.html">接口文档</see><br/>
	/// 公域鉴权<br/>
	/// PUT /channels/{channel_id}/roles/{role_id}/permissions
	/// </para>
	/// </summary>
	public static BotApi 修改子频道身份组权限 => new(ApiType.PublicDomain, HttpMethod.Put, @"/channels/{channel_id}/roles/{role_id}/permissions");

	/// <summary>
	/// 获取子频道 channel_id 下的消息 message_id 的详情
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/message/get_message_of_id.html">接口文档</see><br/>
	/// 公域鉴权<br/>
	/// GET /channels/{channel_id}/messages/{message_id}
	/// </para>
	/// </summary>
	public static BotApi 获取指定消息 => new(ApiType.PublicDomain, HttpMethod.Get, @"/channels/{channel_id}/messages/{message_id}");

	/// <summary>
	/// 获取子频道 channel_id 下的消息列表
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/pythonsdk/api/message/get_messages.html">接口文档</see><br/>
	/// 私域鉴权<br/>
	/// GET /channels/{channel_id}/messages
	/// </para>
	/// </summary>
	public static BotApi 获取消息列表 => new(ApiType.PrivateDomain, HttpMethod.Get, @"/channels/{channel_id}/messages");

	/// <summary>
	/// 向 channel_id 指定的子频道发送消息
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/message/post_messages.html">接口文档</see><br/>
	/// 公域鉴权<br/>
	/// POST /channels/{channel_id}/messages
	/// </para>
	/// </summary>
	public static BotApi 发送消息 => new(ApiType.PublicDomain, HttpMethod.Post, @"/channels/{channel_id}/messages");

	/// <summary>
	/// 撤回 message_id 指定的消息
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/nodesdk/message/delete_message.html">接口文档</see><br/>
	/// 私域鉴权<br/>
	/// DELETE /channels/{channel_id}/messages/{message_id}
	/// </para>
	/// </summary>
	public static BotApi 撤回消息 => new(ApiType.PrivateDomain, HttpMethod.Delete, @"/channels/{channel_id}/messages/{message_id}");

	/// <summary>
	/// 机器人和在同一个频道内的成员创建私信会话
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/dms/post_dms.html">接口文档</see><br/>
	/// 公域鉴权<br/>
	/// POST /users/@me/dms
	/// </para>
	/// </summary>
	public static BotApi 创建私信会话 => new(ApiType.PublicDomain, HttpMethod.Post, @"/users/@me/dms");

	/// <summary>
	/// 发送私信消息（已经创建私信会话后）
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/dms/post_dms_messages.html">接口文档</see><br/>
	/// 公域鉴权<br/>
	/// POST /dms/{guild_id}/messages
	/// </para>
	/// </summary>
	public static BotApi 发送私信 => new(ApiType.PublicDomain, HttpMethod.Post, @"/dms/{guild_id}/messages");

	/// <summary>
	/// 将频道的全体成员（非管理员）禁言
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/guild/patch_guild_mute.html">接口文档</see><br/>
	/// 公域鉴权<br/>
	/// PATCH /guilds/{guild_id}/mute
	/// </para>
	/// </summary>
	public static BotApi 禁言全员 => new(ApiType.PublicDomain, HttpMethod.Patch, @"/guilds/{guild_id}/mute");

	/// <summary>
	/// 禁言频道 guild_id 下的成员 user_id
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/guild/patch_guild_member_mute.html">接口文档</see><br/>
	/// 公域鉴权<br/>
	/// PATCH /guilds/{guild_id}/members/{user_id}/mute
	/// </para>
	/// </summary>
	public static BotApi 禁言指定成员 => new(ApiType.PublicDomain, HttpMethod.Patch, @"/guilds/{guild_id}/members/{user_id}/mute");

	/// <summary>
	/// 将频道 guild_id 内的某条消息设置为频道全局公告
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/announces/post_guild_announces.html">接口文档</see><br/>
	/// 公域鉴权<br/>
	/// POST /guilds/{guild_id}/announces
	/// </para>
	/// </summary>
	public static BotApi 创建频道公告 => new(ApiType.PublicDomain, HttpMethod.Post, @"/guilds/{guild_id}/announces");

	/// <summary>
	/// 删除频道 guild_id 下 message_id 指定的全局公告
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/announces/delete_guild_announces.html">接口文档</see><br/>
	/// 公域鉴权<br/>
	/// DELETE /guilds/{guild_id}/announces/{message_id}
	/// </para>
	/// </summary>
	public static BotApi 删除频道公告 => new(ApiType.PublicDomain, HttpMethod.Delete, @"/guilds/{guild_id}/announces/{message_id}");

	/// <summary>
	/// 将子频道 channel_id 内的某条消息设置为子频道公告
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/announces/post_channel_announces.html">接口文档</see><br/>
	/// 公域鉴权<br/>
	/// POST /channels/{channel_id}/announces
	/// </para>
	/// </summary>
	public static BotApi 创建子频道公告 => new(ApiType.PublicDomain, HttpMethod.Post, @"/channels/{channel_id}/announces");

	/// <summary>
	/// 删除子频道 channel_id 下 message_id 指定的子频道公告
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/announces/delete_channel_announces.html">接口文档</see><br/>
	/// 公域鉴权<br/>
	/// DELETE /channels/{channel_id}/announces/{message_id}
	/// </para>
	/// </summary>
	public static BotApi 删除子频道公告 => new(ApiType.PublicDomain, HttpMethod.Delete, @"/channels/{channel_id}/announces/{message_id}");

	/// <summary>
	/// 获取 channel_id 指定的子频道中当天的日程列表
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/schedule/get_schedules.html">接口文档</see><br/>
	/// 公域鉴权<br/>
	/// GET /channels/{channel_id}/schedules
	/// </para>
	/// </summary>
	public static BotApi 获取频道日程列表 => new(ApiType.PublicDomain, HttpMethod.Get, @"/channels/{channel_id}/schedules");

	/// <summary>
	/// 获取日程子频道 channel_id 下 schedule_id 指定的的日程的详情
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/schedule/get_schedule.html">接口文档</see><br/>
	/// 公域鉴权<br/>
	/// GET /channels/{channel_id}/schedules/{schedule_id}
	/// </para>
	/// </summary>
	public static BotApi 获取日程详情 => new(ApiType.PublicDomain, HttpMethod.Get, @"/channels/{channel_id}/schedules/{schedule_id}");

	/// <summary>
	/// 在 channel_id 指定的日程子频道下创建一个日程
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/schedule/post_schedule.html">接口文档</see><br/>
	/// 公域鉴权<br/>
	/// POST /channels/{channel_id}/schedules
	/// </para>
	/// </summary>
	public static BotApi 创建日程 => new(ApiType.PublicDomain, HttpMethod.Post, @"/channels/{channel_id}/schedules");

	/// <summary>
	/// 修改日程子频道 channel_id 下 schedule_id 指定的日程的详情
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/schedule/patch_schedule.html">接口文档</see><br/>
	/// 公域鉴权<br/>
	/// PATCH /channels/{channel_id}/schedules/{schedule_id}
	/// </para>
	/// </summary>
	public static BotApi 修改日程 => new(ApiType.PublicDomain, HttpMethod.Patch, @"/channels/{channel_id}/schedules/{schedule_id}");

	/// <summary>
	/// 删除日程子频道 channel_id 下 schedule_id 指定的日程
	/// <para>
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/schedule/delete_schedule.html">接口文档</see><br/>
	/// 公域鉴权<br/>
	/// DELETE /channels/{channel_id}/schedules/{schedule_id}
	/// </para>
	/// </summary>
	public static BotApi 删除日程 => new(ApiType.PublicDomain, HttpMethod.Delete, @"/channels/{channel_id}/schedules/{schedule_id}");

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
