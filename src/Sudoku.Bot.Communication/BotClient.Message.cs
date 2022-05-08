namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// 获取指定消息
	/// </summary>
	/// <param name="channel_id">子频道Id</param>
	/// <param name="message_id">消息Id</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<Message?> GetMessageAsync(string channel_id, string message_id, Sender? sender = null)
	{
		var api = BotApis.获取指定消息;
		var response = await HttpSendAsync(
			api.Path.Replace("{channel_id}", channel_id).Replace("{message_id}", message_id),
			api.Method,
			null,
			sender
		);

		return response is null ? null : await response.Content.ReadFromJsonAsync<Message?>();
	}

	/// <summary>
	/// 获取消息列表（2022年1月29日暂未开通）
	/// </summary>
	/// <param name="message">作为坐标的消息（需要消息Id和子频道Id）</param>
	/// <param name="limit">分页大小（1-20）</param>
	/// <param name="typesEnum">拉取类型（默认拉取最新消息）</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<List<Message>?> GetMessagesAsync(
		Message message, int limit = 20, GetMsgTypesEnum? typesEnum = GetMsgTypesEnum.latest, Sender? sender = null)
	{
		var api = BotApis.获取消息列表;
		string type = typesEnum is null ? string.Empty : $"&type={typesEnum}";
		var response = await HttpSendAsync(
			$"{api.Path.Replace("{channel_id}", message.ChannelId)}?limit={limit}&id={message.Id}{type}",
			api.Method,
			null,
			sender
		);

		return response is null ? null : await response.Content.ReadFromJsonAsync<List<Message>?>();
	}

	/// <summary>
	/// 发送消息
	/// <para>
	/// 要求操作人在该子频道具有"发送消息"的权限 <br/>
	/// 发送成功之后，会触发一个创建消息的事件 <br/>
	/// 被动回复消息有效期为 5 分钟 <br/>
	/// 主动推送消息每个子频道限 2 条/天 <br/>
	/// 发送消息接口要求机器人接口需要链接到websocket gateway 上保持在线状态
	/// </para>
	/// </summary>
	/// <param name="channel_id">子频道Id</param>
	/// <param name="message">消息对象</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<Message?> SendMessageAsync(string channel_id, MessageToCreate message, Sender? sender = null)
	{
		var api = BotApis.发送消息;
		var response = await HttpSendAsync(
			api.Path.Replace("{channel_id}", channel_id),
			api.Method,
			JsonContent.Create(message),
			sender
		);
		var result = response is null ? null : await response.Content.ReadFromJsonAsync<Message?>();
		return LastMessage(result, true);
	}

	/// <summary>
	/// 撤回消息
	/// </summary>
	/// <param name="channel_id">子频道Id</param>
	/// <param name="message_id">消息Id</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<bool> DeleteMessageAsync(string channel_id, string message_id, Sender? sender = null)
	{
		var api = BotApis.撤回消息;
		var response = await HttpSendAsync(
			api.Path.Replace("{channel_id}", channel_id).Replace("{message_id}", message_id),
			api.Method,
			null,
			sender
		);
		return response?.IsSuccessStatusCode ?? false;
	}

	/// <summary>
	/// 撤回目标用户在当前子频道发出的最后一条消息
	/// <para>
	/// 需要传入指令发出者的消息对象<br/>
	/// 用于检索指令发出者所在频道信息
	/// </para>
	/// </summary>
	/// <param name="masterMessage">
	/// 被撤回消息的目标用户信息<br/>
	/// 需要：message.GuildId、message.ChannelId、message.Author.Id
	/// </param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<bool?> DeleteLastMessageAsync(Message? masterMessage, Sender? sender = null)
	{
		var lastMessage = LastMessage(masterMessage);
		return lastMessage is null ? null : await DeleteMessageAsync(lastMessage.ChannelId, lastMessage.Id, sender);
	}

	/// <summary>
	/// 集中处理聊天消息
	/// </summary>
	/// <param name="message">消息对象</param>
	/// <param name="type">消息类型
	/// <para>
	/// DIRECT_MESSAGE_CREATE - 私信<br/>
	/// AT_MESSAGE_CREATE - 频道内 @机器人<br/>
	/// MESSAGE_CREATE - 频道内任意消息(仅私域支持)<br/>
	/// </para></param>
	/// <returns></returns>
	private async Task MessageCenterAsync(Message message, string type)
	{
		// 记录Sender信息
		Sender sender = new(message, this);

		// 识别消息类型（私聊，AT全员，AT机器人）
		_ = message switch
		{
			{ DirectMessage: true } => sender.MessageType = MessageType.Private,
			{ MentionEveryone: true } => sender.MessageType = MessageType.AtAll,
			{ Mentions: var list } when list?.Any(u => u.Id == Info.Id) is true => sender.MessageType = MessageType.AtMe,
			_ => default
		};

		// 记录机器人在当前频道下的身份组信息
		if (sender.MessageType != MessageType.Private && !Members.ContainsKey(message.GuildId))
		{
			Members[message.GuildId] = await GetMemberAsync(message.GuildId, Info.Id);
		}

		// 若已经启用全局消息接收，将不单独响应 AT_MESSAGES 事件，否则会造成重复响应。
		if (Intents.HasFlag(Intent.MESSAGE_CREATE) && type.StartsWith("A"))
		{
			return;
		}

		// 调用消息拦截器
		if (MessageFilter?.Invoke(sender) is true)
		{
			return;
		}

		// 记录收到的消息
		LastMessage(message, true);

		// 预判收到的消息
		string content = message.Content.Trim().ReplaceStart(Info.Tag).TrimStart();

		// 识别指令
		bool hasCommand = content.StartsWith(CommandPrefix);
		content = content.ReplaceStart(CommandPrefix).TrimStart();
		if ((hasCommand || sender.MessageType == MessageType.AtMe || sender.MessageType == MessageType.Private)
			&& content.Length > 0)
		{
			// 在新的线程上输出日志信息
			_ = Task.Run(logHandler);

			// 并行遍历指令列表，提升效率
			var result = Parallel.ForEach(Commands.Values, forEachLoopHandler);

			if (!result.IsCompleted)
			{
				return;
			}
		}

		// 触发Message到达事件
		OnMsgCreate?.Invoke(sender);


		void logHandler()
		{
			static bool predicate(User user, Match m) => user.Tag == m.Groups[0].Value;
			string f(Match m) => message.Mentions?.Find(u => predicate(u, m))?.UserName.Insert(0, "@") ?? m.Value;
			string msgContent = Regex.Replace(message.Content, """<@!\d+>""", f);
			string senderMaster = (sender.Bot.Guilds.TryGetValue(sender.GuildId, out var guild) ? guild.Name : null)
				?? sender.Author.UserName;
			Log.Info($"[{senderMaster}][{message.Author.UserName}] {msgContent.Replace("\xA0", " ")}");
		}

		void forEachLoopHandler(Command cmd, ParallelLoopState state, long i)
		{
			var cmdMatch = cmd.Rule.Match(content);
			if (!cmdMatch.Success)
			{
				return;
			}

			content = content.ReplaceStart(cmdMatch.Groups[0].Value).TrimStart();
			if (
				cmd.NeedAdmin && !(
					message.Member.Roles.Any(static r => "24".Contains(r)) || message.Author.Id.Equals(GodId)
				)
			)
			{
				if (sender.MessageType == MessageType.AtMe)
				{
					_ = sender.ReplyAsync($"{message.Author.Tag} 你无权使用该命令！");
				}
				else
				{
					return;
				}
			}
			else
			{
				cmd.CallBack?.Invoke(sender, content);
			}

			state.Break();
		}
	}
}
