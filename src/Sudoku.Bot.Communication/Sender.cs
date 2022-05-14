namespace Sudoku.Bot.Communication;

/// <summary>
/// Defines a sender instance being used for an encapsulation on sending messages,
/// introducing who sends the message. This type is generally used for a bot instance.
/// </summary>
/// <param name="Bot">Indicates the bot client instance.</param>
/// <param name="Message">Indicates the message instance. The value can be <see langword="null"/>.</param>
public sealed record class Sender(Message Message, BotClient Bot)
{
	/// <summary>
	/// Indicates the message type. The default value is <see cref="MessageType.Public"/>.
	/// </summary>
	public MessageType MessageType { get; set; } = MessageType.Public;

	/// <summary>
	/// Indicates whether the message mentions the bot.
	/// </summary>
	public bool IsMentioned => MessageType == MessageType.BotMentioned;

	/// <summary>
	/// Indicates whether an error message will be reported. The default value is <see cref="BotClient.ReportApiError"/>.
	/// </summary>
	public bool ReportError { get; set; }

	/// <summary>
	/// Indicates the content of the message.
	/// </summary>
	public string Content => Message.Content;

	/// <summary>
	/// Indicates the GUILD.
	/// </summary>
	public string GuildId => Message.GuildId;

	/// <summary>
	/// Indicates the channel.
	/// </summary>
	public string ChannelId => Message.ChannelId;

	/// <summary>
	/// Indicates the message creator.
	/// </summary>
	public User MessageCreator => Message.MessageCreator;

	/// <summary>
	/// Indicates the member who have joined the same GUILD as the current sender.
	/// </summary>
	public Member Member => Message.Member;

	/// <summary>
	/// Indicates the users who the message has mentioned.
	/// </summary>
	public List<User>? Mentions => Message.Mentions;


	/// <summary>
	/// Replies a message to the message creator.
	/// </summary>
	/// <param name="message">Indicates a message.</param>
	/// <param name="isQuote">Indicates whether the message use a quote that references to the message creator.</param>
	/// <returns>A task instance encapsulates the message sent.</returns>
	public async Task<Message?> ReplyAsync(MessageToCreate message, bool isQuote = false)
	{
		message.Id = Message.Id;
		if (isQuote)
		{
			message.Reference = new() { MessageId = Message.Id, IgnoreGetMessageError = true };
		}

		return await (
			MessageType == MessageType.Private
				? Bot.SendPrivateMessageAsync(GuildId, message, null)
				: Bot.SendMessageAsync(ChannelId, message, this)
		);
	}

	/// <summary>
	/// Replies a message as the string representation to the message creator.
	/// </summary>
	/// <param name="message">Indicates a message.</param>
	/// <param name="isQuote">Indicates whether the message use a quote that references to the message creator.</param>
	/// <returns>A task instance encapsulates the message sent.</returns>
	public async Task<Message?> ReplyAsync(string message, bool isQuote = false)
		=> await ReplyAsync(new MsgText(message), isQuote);

	/// <inheritdoc cref="BotClient.GetInfoAsync(Sender?)"/>
	public async Task<User?> GetSenderInfoAsync() => await Bot.GetInfoAsync(this);

	/// <inheritdoc cref="BotClient.GetMeGuildsAsync(string?, bool, int, Sender?)"/>
	public async Task<List<Guild>?> GetMeGuildsAsync(string? guild_id = null, bool route = false, int limit = 100)
		=> await Bot.GetMeGuildsAsync(guild_id, route, limit, this);

	/// <inheritdoc cref="BotClient.GetGuildAsync(string, Sender?)"/>
	public async Task<Guild?> GetGuildAsync() => await Bot.GetGuildAsync(Message.GuildId, this);

	/// <inheritdoc cref="BotClient.GetChannelsAsync(string, ChannelType?, ChannelSubtype?, Sender?)"/>
	public async Task<List<Channel>?> GetChannelsAsync(ChannelType? type = null, ChannelSubtype? subtype = null)
		=> await Bot.GetChannelsAsync(GuildId, type, subtype, this);

	/// <inheritdoc cref="BotClient.GetChannelAsync(string, Sender?)"/>
	public async Task<Channel?> GetChannelAsync(string? channelId = null)
		=> await Bot.GetChannelAsync(channelId ?? ChannelId, this);

	/// <inheritdoc cref="BotClient.CreateChannelAsync(Channel, Sender?)"/>
	public async Task<Channel?> CreateChannelAsync(Channel channel)
	{
		if (string.IsNullOrWhiteSpace(channel.GuildId))
		{
			channel.GuildId = GuildId;
		}

		return await Bot.CreateChannelAsync(channel, this);
	}

	/// <inheritdoc cref="BotClient.EditChannelAsync(Channel, Sender?)"/>
	public async Task<Channel?> EditChannelAsync(Channel channel) => await Bot.EditChannelAsync(channel, this);

	/// <inheritdoc cref="BotClient.DeleteChannelAsync(string, Sender?)"/>
	public async Task<bool> DeleteChannelAsync(Channel channel) => await Bot.DeleteChannelAsync(channel.Id, this);

	/// <inheritdoc cref="BotClient.GetGuildMembersAsync(string, int, string?, Sender?)"/>
	public async Task<List<Member>?> GetGuildMembersAsync(int limit = 100, string? after = null)
		=> await Bot.GetGuildMembersAsync(GuildId, limit, after, this);

	/// <inheritdoc cref="BotClient.GetMemberAsync(string, string, Sender?)"/>
	public async Task<Member?> GetMemberAsync(User user) => await Bot.GetMemberAsync(GuildId, user.Id, this);

	/// <inheritdoc cref="BotClient.DeleteGuildMemberAsync(string, string, Sender?)"/>
	public async Task<bool> DeleteGuildMemberAsync(User user) => await Bot.DeleteGuildMemberAsync(GuildId, user.Id, this);

	/// <inheritdoc cref="BotClient.GetRolesAsync(string, Sender?)"/>
	public async Task<List<Role>?> GetRolesAsync() => await Bot.GetRolesAsync(Message.GuildId, this);

	/// <inheritdoc cref="BotClient.CreateRoleAsync(string, Info, Filter?, Sender?)"/>
	public async Task<Role?> CreateRoleAsync(Info info) => await Bot.CreateRoleAsync(GuildId, info, null, this);

	/// <inheritdoc cref="BotClient.EditRoleAsync(string, string, Info, Filter?, Sender?)"/>
	public async Task<Role?> EditRoleAsync(string role_id, Info info)
		=> await Bot.EditRoleAsync(GuildId, role_id, info, null, this);

	/// <inheritdoc cref="BotClient.DeleteRoleAsync(string, string, Sender?)"/>
	public async Task<bool> DeleteRoleAsync(string role_id) => await Bot.DeleteRoleAsync(GuildId, role_id, this);

	/// <inheritdoc cref="BotClient.AddRoleMemberAsync(string, string, string, string?, Sender?)"/>
	public async Task<bool> AddRoleMemberAsync(User user, string role_id, string? channel_id = null)
		=> await Bot.AddRoleMemberAsync(GuildId, user.Id, role_id, channel_id, this);

	/// <inheritdoc cref="BotClient.DeleteRoleMemberAsync(string, string, string, string?, Sender?)"/>
	public async Task<bool> DeleteRoleMemberAsync(User user, string role_id, string? channel_id = null)
		=> await Bot.DeleteRoleMemberAsync(GuildId, user.Id, role_id, channel_id, this);

	/// <inheritdoc cref="BotClient.GetChannelPermissionsAsync(string, string, Sender?)"/>
	public async Task<ChannelPermissions?> GetChannelPermissionsAsync(User? user = null, string? channel_id = null)
		=> await Bot.GetChannelPermissionsAsync(channel_id ?? ChannelId, user?.Id ?? MessageCreator.Id, this);

	/// <inheritdoc cref="BotClient.EditChannelPermissionsAsync(string, string, string, string, Sender?)"/>
	public async Task<bool> EditChannelPermissionsAsync(PrivacyType permission, User user, string? channel_id = null)
	{
		string permissionToBeAdded = ((int)permission).ToString();
		string permissionToBeRemoved = (0x7 ^ (int)permission).ToString();
		return await Bot.EditChannelPermissionsAsync(channel_id ?? ChannelId, user.Id, permissionToBeAdded, permissionToBeRemoved, this);
	}

	/// <inheritdoc cref="BotClient.GetRolePermissionsInChannelAsync(string, string, Sender?)"/>
	public async Task<ChannelPermissions?> GetMemberChannelPermissionsAsync(string role_id, string? channel_id = null)
		=> await Bot.GetRolePermissionsInChannelAsync(channel_id ?? ChannelId, role_id, this);

	/// <inheritdoc cref="BotClient.EditRolePermissionsInChannelAsync(string, string, string, string, Sender?)"/>
	public async Task<bool> EditMemberChannelPermissionsAsync(PrivacyType permission, string role_id, string? channel_id = null)
	{
		string permissionsToBeAdded = ((int)permission).ToString();
		string permissionsToBeRemoved = (0x07 ^ (int)permission).ToString();
		return await Bot.EditRolePermissionsInChannelAsync(channel_id ?? ChannelId, role_id, permissionsToBeAdded, permissionsToBeRemoved, this);
	}

	/// <inheritdoc cref="BotClient.GetMessagesAsync(Message, int, GetMessageType?, Sender?)"/>
	public async Task<List<Message>?> GetMessagesAsync(
		Message? message = null, int limit = 20, GetMessageType? type = null)
		=> await Bot.GetMessagesAsync(message ?? Message, limit, type, this);

	/// <inheritdoc cref="BotClient.GetMessageAsync(string, string, Sender?)"/>
	public async Task<Message?> GetMessageAsync(Message message)
		=> await Bot.GetMessageAsync(message.ChannelId, message.Id, this);

	/// <inheritdoc cref="BotClient.SendMessageAsync(string, MessageToCreate, Sender?)"/>
	public async Task<Message?> SendMessageAsync(MessageToCreate message, string? channel_id = null)
		=> await Bot.SendMessageAsync(channel_id ?? ChannelId, message, this);

	/// <inheritdoc cref="BotClient.DeleteMessageAsync(string, string, Sender?)"/>
	public async Task<bool> DeleteMessageAsync(Message message)
		=> await Bot.DeleteMessageAsync(message.ChannelId, message.Id, this);

	/// <inheritdoc cref="BotClient.CreateDirectMessageAsync(string, string, Sender)"/>
	public async Task<DirectMessageSource?> CreateDirectMessageCommunicationAsync(User user)
		=> await Bot.CreateDirectMessageAsync(user.Id, GuildId, this);

	/// <inheritdoc cref="BotClient.SendPrivateMessageAsync(string, MessageToCreate, Sender?)"/>
	public async Task<Message?> SendPrivateMessageAsync(MessageToCreate message, string? guild_id = null)
		=> await Bot.SendPrivateMessageAsync(guild_id ?? GuildId, message, this);

	/// <inheritdoc cref="BotClient.JinxGuildAsync(string, JinxTimeSpan, Sender?)"/>
	public async Task<bool> JinxGuildAsync(JinxTimeSpan muteTime) => await Bot.JinxGuildAsync(GuildId, muteTime, this);

	/// <inheritdoc cref="BotClient.JinxMemberAsync(string, string, JinxTimeSpan, Sender?)"/>
	public async Task<bool> JinxMemberAsync(User user, JinxTimeSpan muteTime)
		=> await Bot.JinxMemberAsync(GuildId, user.Id, muteTime, this);

	/// <inheritdoc cref="BotClient.CreateAnnouncesGlobalAsync(string, string, string, Sender?)"/>
	public async Task<Announces?> CreateAnnouncesGlobalAsync(Message message)
		=> await Bot.CreateAnnouncesGlobalAsync(GuildId, ChannelId, message.Id, this);

	/// <inheritdoc cref="BotClient.DeleteAnnouncesGlobalAsync(string, string, Sender?)"/>
	public async Task<bool> DeleteAnnouncesGlobalAsync()
		=> await Bot.DeleteAnnouncesGlobalAsync(GuildId, "all", this);

	/// <inheritdoc cref="BotClient.CreateAnnouncesAsync(string, string, Sender?)"/>
	public async Task<Announces?> CreateAnnouncesAsync(Message message, string? channel_id = null)
		=> await Bot.CreateAnnouncesAsync(channel_id ?? message.ChannelId, message.Id, this);

	/// <inheritdoc cref="BotClient.DeleteAnnouncesAsync(string, string, Sender?)"/>
	public async Task<bool> DeleteAnnouncesAsync(string? channel_id = null)
		=> await Bot.DeleteAnnouncesAsync(channel_id ?? ChannelId, "all", this);

	/// <inheritdoc cref="BotClient.GetSchedulesAsync(string, DateTime?, Sender?)"/>
	public async Task<List<Schedule>?> GetSchedulesAsync(string channel_id, DateTime? since = null)
		=> await Bot.GetSchedulesAsync(channel_id, since, this);

	/// <inheritdoc cref="BotClient.GetScheduleAsync(string, string, Sender?)"/>
	public async Task<Schedule?> GetScheduleAsync(string channel_id, string schedule_id)
		=> await Bot.GetScheduleAsync(channel_id, schedule_id, this);

	/// <inheritdoc cref="BotClient.CreateScheduleAsync(string, Schedule, Sender?)"/>
	public async Task<Schedule?> CreateScheduleAsync(string channel_id, Schedule schedule)
		=> await Bot.CreateScheduleAsync(channel_id, schedule, this);

	/// <inheritdoc cref="BotClient.EditScheduleAsync(string, Schedule, Sender?)"/>
	public async Task<Schedule?> EditScheduleAsync(string channel_id, Schedule schedule)
		=> await Bot.EditScheduleAsync(channel_id, schedule, this);

	/// <inheritdoc cref="BotClient.DeleteScheduleAsync(string, Schedule, Sender?)"/>
	public async Task<bool> DeleteScheduleAsync(string channel_id, Schedule schedule)
		=> await Bot.DeleteScheduleAsync(channel_id, schedule, this);

	/// <inheritdoc cref="BotClient.AudioControlAsync(string, AudioControl, Sender?)"/>
	public async Task<Message?> AudioControlAsync(AudioControl audioControl, string? channel_id = null)
		=> await Bot.AudioControlAsync(channel_id ?? ChannelId, audioControl, this);

	/// <inheritdoc cref="BotClient.GetGuildPermissionsAsync(string, Sender?)"/>
	public async Task<List<ApiPermission>?> GetGuildPermissionsAsync()
		=> await Bot.GetGuildPermissionsAsync(GuildId, this);

	/// <inheritdoc cref="BotClient.SendPermissionDemandAsync(string, string, ApiPermissionDemandIdentify, string, Sender?)"/>
	/// <remarks>
	/// This API can only be called 3 times per day per GUILD.
	/// </remarks>
	public async Task<ApiPermissionDemand?> SendPermissionDemandAsync(
		ApiPermissionDemandIdentify api_identify, string description = "")
		=> await Bot.SendPermissionDemandAsync(GuildId, ChannelId, api_identify, description, this);
}
