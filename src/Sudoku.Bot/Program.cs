#pragma warning disable IDE0011

var filePath = @"A:\QQ机器人\bot.json";
var json = File.ReadAllText(filePath);
var botInfo = JsonSerializer.Deserialize<BotInfo>(json, JsonOptions)!;
var accessInfo = new OpenApiAccessInfo { BotQQ = botInfo.Id, BotAppId = botInfo.AppId, BotToken = botInfo.Token, BotSecret = botInfo.Secret };
var apiProvider = new QQChannelApi(accessInfo);
apiProvider.UseBotIdentity();

var registeredCommands = (Command[])[
	new DisplayInfoCommand(),
	new ChangeInfoCommand(),
	new CheckInCommand(),
	new AnalysisCommand()
];

var bot = new ChannelBot(apiProvider);
bot.RegisterChatEvent();
bot.ReceivedChatGroupMessage += bot_ReceivedAtMessage;
bot.OnConnected += bot_OnConnected;
bot.AuthenticationSuccess += bot_AuthenticationSuccess;
bot.OnError += bot_OnError;
await bot.OnlineAsync();

Console.WriteLine("请按 Q 键退出机器人。");
while (Console.ReadLine() is not ("Q" or "q")) ;
Console.WriteLine("退出机器人。");

void bot_OnConnected()
{
	var commandNames = string.Join("、", from command in registeredCommands select command.CommandName);
	Console.WriteLine("连接机器人成功！");
	Console.ForegroundColor = ConsoleColor.Green;
	Console.WriteLine($"已注册的指令一共 {registeredCommands.Length} 个指令：{commandNames}");
	Console.ResetColor();
}

static void bot_AuthenticationSuccess() => Console.WriteLine("机器人鉴权成功！现在可以用机器人了。");

static void bot_OnError(Exception ex)
{
	Console.ForegroundColor = ConsoleColor.Red;
	Console.WriteLine($"机器人执行指令时出现错误：{ex.Message}");
	Console.ResetColor();
}

void bot_ReceivedAtMessage(ChatMessage message)
{
	var content = message.Content.Trim();
	content = content[content.IndexOf(' ') is var i and not -1 ? ..i : ..];
	var found = false;
	foreach (var command in registeredCommands)
	{
		if ($"/{command.CommandName}" == content)
		{
			command.GroupCallback(apiProvider.GetChatMessageApi(), message);
			found = true;
			break;
		}
	}

	if (found)
	{
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine($"接收消息：“{message.Content}”。");
	}
	else
	{
		Console.ForegroundColor = ConsoleColor.DarkYellow;
		Console.WriteLine($"接收消息：“{message.Content}”，但指令匹配失败。");
	}
	Console.ResetColor();
}


/// <summary>
/// 主方法包裹的程序类。
/// </summary>
file static partial class Program
{
	/// <summary>
	/// 在反序列化 JSON 期间使用到的解析控制选项。
	/// </summary>
	private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
}
