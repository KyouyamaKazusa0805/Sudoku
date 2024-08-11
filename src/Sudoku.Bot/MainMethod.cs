var json = File.ReadAllText(BotConfigPath);
var botAccessInfo = (OpenApiAccessInfo?)JsonSerializer.Deserialize<BotInfo>(json, JsonOptions)!;
var apiProvider = new QQChannelApi(botAccessInfo);
apiProvider.UseBotIdentity();

var registeredCommands = Command.AssemblyCommands();
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
	var content = chatMessage.GetCommandFullName();
	var found = false;
	foreach (var command in registeredCommands)
	{
		if (command.CommandFullName == content)
		{
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
		var commandNames = string.Join(ChineseComma, from command in registeredCommands select command.CommandName);
		WriteLog("连接机器人成功。");
		WriteLog(LogSeverity.Info, $"已注册的指令一共 {registeredCommands.Length} 个指令：{commandNames}。");
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
