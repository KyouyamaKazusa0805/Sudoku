// Configures the log level.
Log.LogLevel = LogLevel.Debug;

// Initializes an identity instance.
// Please note that the token and secret code corresponds to the info for the bot.
// For more information please visit the link https://q.qq.com/bot/#/home
var identity = loadBotConfiguration(StringResource.Get("__LocalBotConfigPath")!, out string testChannelId);

// Initializes a bot client.
var bot = new BotClient(identity, sandBoxApi: true, reportApiError: true)
{
	Intents = Intents.PrivateDomain,
	CommandPrefix = "/",
	MessageFilter = sender => sender.ChannelId != testChannelId
};

// Sets the elementary event handlers.
initializeEvents(bot);

// Add commands.
initializeCommands(bot);

// Starts the bot.
bot.Start();

// Here we should use an infinite loop to make the console don't exit fast.
// Issue: https://github.com/Antecer/QQChannelBot/issues/1
while (true)
{
	await Task.Delay(1000);
}


static void initializeEvents(BotClient bot)
{
	bot.OnAuthorizationPassed += static user =>
	{
		var sdk = typeof(BotClient).Assembly.GetName();
		Console.Title = $"{user.UserName}_Private <{user.Id}> - SDK Version: {sdk.Name}_{sdk.Version}; connetion state:";
	};
	bot.OnMsgCreate += static async sender =>
	{
		if (sender is not { AtMe: true, Content: var content, Bot.Info.Tag: var botInfoTag, Author.Tag: var authorTag })
		{
			return;
		}

		await sender.ReplyAsync(content.Replace(botInfoTag, authorTag));
	};
}

static void initializeCommands(BotClient bot)
{
	// Repeats the message.
	// e.g. User sends the message:
	//     @bot /repeat 123
	// Then the bot will reply:
	//     @user 123
	var repeatCommand = new Command(StringResource.Get("Command_Repeat")!, repeatAsync);
	bot.AddCommand(repeatCommand);

	// Prints the information about the author of the bot, and the bot itself.
	var aboutCommand = new Command(StringResource.Get("Command_About")!, printAboutInfoAsync);
	bot.AddCommand(aboutCommand);
}

static async void repeatAsync(Sender sender, string message)
{
	if (string.IsNullOrWhiteSpace(message))
	{
		return;
	}

	await sender.ReplyAsync($"{sender.Author.Tag} {message}");
}

static async void printAboutInfoAsync(Sender sender, string message)
	=> await sender.ReplyAsync(
		$"""
		{StringResource.Get("AboutInfo_Segment1")!}{AssemblyVersion}
		---
		{StringResource.Get("AboutInfo_Segment2")!}
		"""
	);

static Identity loadBotConfiguration(string localFilePath, out string testChannelId)
{
	Unsafe.SkipInit(out testChannelId);

	string text = File.ReadAllText(localFilePath);
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
