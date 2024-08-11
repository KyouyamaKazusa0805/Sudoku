#pragma warning disable CA1418

[assembly: SupportedOSPlatform("windows")]
[assembly: SupportedOSPlatform("Tencent-QQ")] // 故意的

var json = File.ReadAllText(BotConfigPath);
var botAccessInfo = (OpenApiAccessInfo?)JsonSerializer.Deserialize<BotInfo>(json, JsonOptions)!;
var apiProvider = new QQChannelApi(botAccessInfo);
apiProvider.UseBotIdentity();

var bot = new ChannelBot(apiProvider);
bot.RegisterChatEvent();
bot.ReceivedChatGroupMessage += onChatMessageReceived;
bot.OnConnected += onConnected;
bot.AuthenticationSuccess += onAuthenticationSuccessful;
bot.OnError += onErrorEncountered;
await bot.OnlineAsync();

WriteLog("请按 Q 键退出机器人。");
BlockConsole('Q', 'q');
WriteLog("退出机器人。");


void onChatMessageReceived(ChatMessage chatMessage)
{
	// 要判断下长指令的运行环境。如果指令执行时间较长（如游戏），就需要判断是否当前 QQ 群是否存有运行游戏的上下文。
	if (RunningContexts.ContainsKey(chatMessage.GroupOpenId))
	{
		return;
	}

	var content = chatMessage.GetCommandFullName();
	var found = false;
	foreach (var command in RegisteredCommands)
	{
		if (command.CommandFullName == content)
		{
			// 指令触发成功。
			command.GroupCallback(apiProvider.GetChatMessageApi(), chatMessage);
			found = true;
			break;
		}
	}

	var (severity, message) = found
		? (LogSeverity.Info, $"接收消息：“{chatMessage.Content}”。")
		: (LogSeverity.Warning, $"接收消息：“{chatMessage.Content}”，但指令匹配失败。");
	WriteLog(severity, message);
}

void onConnected()
{
	if (_isFirstLaunch)
	{
		var commandNames = string.Join(ChineseComma, from command in RegisteredCommands select command.CommandName);
		WriteLog("连接机器人成功。");
		WriteLog(LogSeverity.Info, $"已注册的指令一共 {RegisteredCommands.Length} 个指令：{commandNames}。");
	}
	else
	{
		WriteLog("机器人重连成功。");
	}
}

static void onAuthenticationSuccessful()
{
	if (_isFirstLaunch)
	{
		WriteLog("机器人鉴权成功！现在可以用机器人了。");
		_isFirstLaunch = false;
	}
}

static void onErrorEncountered(Exception ex) => WriteLog(LogSeverity.Error, $"机器人执行指令时出现错误：{ex.Message}");
