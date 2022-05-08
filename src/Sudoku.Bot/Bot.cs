// Configures the log level.
Log.LogLevel = LogLevel.Debug;

// Initializes an identity instance.
// Please note that the token and secret code corresponds to the info for the bot.
// For more information please visit the link https://q.qq.com/bot/#/home
const string localPath = """P:\Bot\bot.json""";
var identity = loadBotFileFromLocal(localPath, out string testChannelId);

// Initializes a bot client.
var bot = new BotClient(identity, sandBoxApi: true, reportApiError: true)
{
	Intents = Intents.PrivateDomain,
	CommandPrefix = "/",
	MessageFilter = sender => sender.ChannelId != testChannelId
};

// Sets the elementary event handlers.
bot.OnReady += ready;
bot.OnMsgCreate += messageCreatedAsync;

// Registers the customized command. For example, repeating.
// e.g. User sends the message:
//     @bot /repeat 123
// Then the bot will reply:
//     @user 123
var repeatCommand = new Command("repeat", repeatAsync);
bot.AddCommand(repeatCommand);

// Starts the bot.
bot.Start();

// Here we should use an infinite loop to make the console don't exit fast.
// Issue: https://github.com/Antecer/QQChannelBot/issues/1
while (true)
{
	await Task.Delay(1000);
}


static void ready(User user)
{
	var sdk = typeof(BotClient).Assembly.GetName();
	Console.Title = $"{user.UserName}_Private <{user.Id}> - SDK Version: {sdk.Name}_{sdk.Version}; connetion state:";
}

static async void messageCreatedAsync(Sender sender)
{
	if (sender.AtMe)
	{
		string replyMsg = sender.Content.Replace(sender.Bot.Info.Tag, sender.Author.Tag);
		await sender.ReplyAsync(replyMsg);
	}
}

static async void repeatAsync(Sender sender, string message) => await sender.ReplyAsync($"{sender.Author.Tag} {message}");

static Identity loadBotFileFromLocal(string path, out string testChannelId)
{
	Unsafe.SkipInit(out testChannelId);

	string text = File.ReadAllText(path);
	var options = new JsonSerializerOptions { AllowTrailingCommas = true };
	var jsonElement = JsonSerializer.Deserialize<JsonElement>(text, options);
	Unsafe.SkipInit(out string appid);
	Unsafe.SkipInit(out string token);
	Unsafe.SkipInit(out string secret);
	foreach (var element in jsonElement.EnumerateObject())
	{
		string realValue = element.Value.ToString();
		_ = element.Name switch
		{
			"appid" => appid = realValue,
			"token" => token = realValue,
			"secret" => secret = realValue,
			"testChannelId" => testChannelId = realValue,
			_ => default
		};
	}

	return new() { BotAppId = appid, BotToken = token, BotSecret = secret };
}
