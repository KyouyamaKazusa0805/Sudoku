namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// Gets the specified message.
	/// </summary>
	/// <param name="channel_id">The channel. <b>The argument cannot be renamed.</b></param>
	/// <param name="message_id">The message. <b>The argument cannot be renamed.</b></param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>A task instance encapsulates the message instance as the result value.</returns>
	public async Task<Message?> GetMessageAsync(string channel_id, string message_id, Sender? sender)
		=> BotApis.GetMessageInChannel is { Path: var path, Method: var method }
		&& path.ReplaceArgument(channel_id).ReplaceArgument(message_id) is var replacedPath
		&& await HttpSendAsync(replacedPath, method, null, sender) is { Content: var responseContent }
			? await responseContent.ReadFromJsonAsync<Message?>()
			: null;

	/// <summary>
	/// Gets the message list.
	/// </summary>
	/// <param name="message">The message instance. This argument must require the GUILD and channel ID.</param>
	/// <param name="limit">The limit number of messages in a page. The value can only be between 1 and 20.</param>
	/// <param name="type">The type to get the message list.</param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>A task instance encapsulates the message list as the result value.</returns>
	public async Task<List<Message>?> GetMessagesAsync(Message message, int limit, GetMessageType? type, Sender? sender)
		=> BotApis.GetMessagesInChannel is { Path: var path, Method: var method }
		&& (type is null ? string.Empty : $"&type={type?.ToString().ToLower()}") is var typeStr
		&& $"{path.Replace("{channel_id}", message.ChannelId)}?limit={limit}&id={message.Id}{typeStr}" is var replacedPath
		&& await HttpSendAsync(replacedPath, method, null, sender) is { Content: var responseContent }
			? await responseContent.ReadFromJsonAsync<List<Message>?>()
			: null;

	/// <summary>
	/// Sends the message.
	/// </summary>
	/// <param name="channel_id">The channel. <b>The argument cannot be renamed.</b></param>
	/// <param name="message">The message instance.</param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>A task instance encapsulates the message instance as the result value.</returns>
	public async Task<Message?> SendMessageAsync(string channel_id, MessageToCreate message, Sender? sender)
		=> LastMessage(
			BotApis.SendMessageToChannel is { Path: var path, Method: var method }
			&& path.ReplaceArgument(channel_id) is var replacedPath
			&& JsonContent.Create(message) is var jsonContent
			&& await HttpSendAsync(replacedPath, method, jsonContent, sender) is var response
			&& response is { Content: var responseContent }
				? await responseContent.ReadFromJsonAsync<Message?>()
				: null,
			true
		);

	/// <summary>
	/// Recalls a message.
	/// </summary>
	/// <param name="channel_id">The channel. <b>The argument cannot be renamed.</b></param>
	/// <param name="message_id">The message. <b>The argument cannot be renamed.</b></param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>
	/// A task instance encapsulates the <see cref="bool"/> value indicating whether the operation is successful.
	/// </returns>
	public async Task<bool> DeleteMessageAsync(string channel_id, string message_id, Sender? sender)
		=> BotApis.RecallMessageInChannel is { Path: var path, Method: var method }
		&& path.ReplaceArgument(channel_id).ReplaceArgument(message_id) is var replacedPath
		&& ((await HttpSendAsync(replacedPath, method, null, sender))?.IsSuccessStatusCode ?? false);

	/// <summary>
	/// This method is core one used for handling the message.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="type">
	/// Indicates the type of the message. All possible types are:
	/// <list type="table">
	/// <listheader>
	/// <term>Type string</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term><see cref="RawMessageTypes.DirectMessageCreated"/> (i.e. <c>DIRECT_MESSAGE_CREATE</c>)</term>
	/// <description>Indicates the message is a direct message.</description>
	/// </item>
	/// <item>
	/// <term><see cref="RawMessageTypes.MentionMessageCreated"/> (i.e. <c>AT_MESSAGE_CREATE</c>)</term>
	/// <description>Indicates the message mentions the bot in a direct message.</description>
	/// </item>
	/// <item>
	/// <term><see cref="RawMessageTypes.NormalMessageCreated"/> (i.e. <c>MESSAGE_CREATE</c>)</term>
	/// <description>
	/// Indicates the message is in a channel. <b>The type is only used for <see cref="Intents.PrivateDomain"/>.
	/// </b></description>
	/// </item>
	/// </list>
	/// </param>
	/// <returns>A task instance handling the current operation.</returns>
	private async Task MessageCenterAsync(Message message, string type)
	{
		// Defines a sender instance.
		// Now checks the type of the message, to get the details to assign to the property 'sender.MessageType'.
		bool userPredicate(User u) => u.Id == Info.Id;
		var sender = new Sender(message, this)
		{
			MessageType = message switch
			{
				{ IsDirectMessage: true } => MessageType.Private,
				{ IsAllMentioned: true } => MessageType.MentionAll,
				{ Mentions: var list } when list?.Any(userPredicate) is true => MessageType.BotMentioned,
				_ => MessageType.Public // The default value.
			}
		};

		// Now gets the info for the bot in the specified GUILD.
		if (sender.MessageType != MessageType.Private && !Members.ContainsKey(message.GuildId))
		{
			Members[message.GuildId] = await GetMemberAsync(message.GuildId, Info.Id, null);
		}

		// If the message is received globally, the message won't trigger event AT_MESSAGES,
		// otherwise, duplicated triggering.
		if (Intents.Flags(Intent.MESSAGE_CREATE) && type[0] == 'A')
		{
			return;
		}

		// Checks whether the message is passed for the filtering property.
		// Please note that if the filter returns false (rather than not true), the message won't be triggered.
		// In other words, if the property is null, or the property holds a delegate instance, but returning true
		// after being invoked, the message will be passed.
		if (MessageFilter is not null && !MessageFilter(sender))
		{
			return;
		}

		// Pushes it into the stack.
		LastMessage(message, true);

		if (
#pragma warning disable IDE0055
			(message, sender) is not (
				{
					Content: var messageContent,
					Mentions: var messageMentions,
					MessageCreator: { Id: var messageCreatorId } messageCreator,
					Member.Roles: var memberRoles
				},
				{
					MessageType: var messageType,
					Bot: var bot,
					GuildId: var guildId,
					MessageCreator.UserName: var creatorUserName
				}
			)
#pragma warning restore IDE0055
		)
		{
			return;
		}

		// Gets the content of the message. Here we use a trick to get the normal content.
		// The mentioning message will contain the tag "<@!idvalue>", where the value 'idvalue'
		// means who you want to mention. We just remove it to get the last message content.
		string content = MessageContent.RemoveTag(messageContent, Info);

		// Try to recognize the command.
		bool hasCommand = content.StartsWith(CommandPrefix);
		content = content.ReplaceStart(CommandPrefix).TrimStart();
		if ((hasCommand || messageType is MessageType.BotMentioned or MessageType.Private) && content.Length > 0)
		{
			// Runs the logger to get the log message output the command line.
			_ = Task.Run(logHandler);

			// Use parallel iteration to get all possible commands to run.
			// Therefore, you should note that you had better not to reference different commands.
			var result = Parallel.ForEach(Commands.Values, forEachLoopHandler);
			if (!result.IsCompleted)
			{
				return;
			}
		}

		// Triggers the event 'MessageCreated'.
		MessageCreated?.Invoke(this, new(sender));


		void logHandler()
		{
			static bool predicate(User user, Match m) => user.Tag == m.Groups[0].Value;
			string f(Match m) => messageMentions?.Find(u => predicate(u, m))?.UserName.Insert(0, "@") ?? m.Value;
			string msgContent = Regex.Replace(messageContent, """<@!\d+>""", f);
			string senderMaster = (bot.Guilds.TryGetValue(guildId, out var guild) ? guild.Name : null) ?? creatorUserName;
			Log.Info($"[{senderMaster}][{messageCreator.UserName}] {msgContent.Replace("\xA0", " ")}");
		}

		void forEachLoopHandler(Command cmd, ParallelLoopState state, long i)
		{
			if (cmd.Rule.Match(content) is not { Success: true, Groups: [{ Value: var firstGroupValue }, ..] })
			{
				return;
			}

			bool rolePredicate(string r) => "24".Contains(r);

			content = content.ReplaceStart(firstGroupValue).TrimStart();
			if (cmd.RequiresAdministratorPermission && !(memberRoles.Any(rolePredicate) || messageCreatorId.Equals(GodId)))
			{
				if (messageType != MessageType.BotMentioned)
				{
					return;
				}

				string youCannotUseThisCommandText = StringResource.Get("YouCannotUseThisCommand")!;
				_ = sender.ReplyAsync($"{messageCreator.Tag} {youCannotUseThisCommandText}");
			}
			else
			{
				cmd.Callback(sender, content);
			}

			state.Break();
		}
	}
}
