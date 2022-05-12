partial class Program
{
	/// <summary>
	/// Indicates the assembly version.
	/// </summary>
	private static Version? AssemblyVersion => typeof(Program).Assembly.GetName().Version;


	/// <summary>
	/// Indicates callback method that will be invoked when the bot has successfully connected to the server,
	/// and successfully passed the authorization.
	/// </summary>
	private static void OnSuccessfullyConnected()
		=> Console.WriteLine(StringResource.Get("BotConnectedCallbackOutput_Success")!);

	/// <summary>
	/// Indicates callback method that will be invoked when the bot has failed connected to the server,
	/// or failed passed the authorization.
	/// </summary>
	private static void OnFailedConnected()
		=> Console.WriteLine(StringResource.Get("BotConnectedCallbackOutput_Fail")!);

	/// <summary>
	/// Exits the program fast.
	/// </summary>
	/// <param name="bot">The bot client which should be closed.</param>
	private static void FastExitOnBot(BotClient bot)
	{
		bot.Close();
		Environment.Exit(0);
	}

	/// <summary>
	/// Initializes the events.
	/// </summary>
	/// <param name="bot">The bot client.</param>
	private static void InitializeEvents(BotClient bot)
	{
		Console.CancelKeyPress += (_, _) => FastExitOnBot(bot);

		bot.AuthorizationPassed += static (_, e) =>
		{
			if (e is { User: { UserName: var userName, Id: var id } user }
				&& typeof(BotClient).Assembly.GetName() is { Name: var sdkName, Version: var sdkVersion })
			{
				string suffix = StringResource.Get("PrivateDomainBotSuffix")!;
				string sdkVersionName = StringResource.Get("SdkVersionName")!;
				string colon = StringResource.Get("Colon")!;

				Console.Title = $"{userName}{suffix} <{id}> - {sdkVersionName}{colon}{sdkName}_{sdkVersion}";
			}
		};
		bot.MessageCreated += static async (_, e) =>
		{
			if (
				e.Sender is
				{
					AtMe: true,
					Content: var content,
					Bot.Info.Tag: var botInfoTag,
					Author.Tag: var authorTag
				} sender
			)
			{
				await sender.ReplyAsync(content.Replace(botInfoTag, authorTag));
			}
		};
	}

	/// <summary>
	/// Registers the commands to be invoked.
	/// </summary>
	/// <param name="bot">The bot instance.</param>
	private static void RegisterCommands(BotClient bot)
	{
		// Clock in.
		// e.g. User sends the message:
		//     @bot /clockIn
		bot.RegisterCommand(new(StringResource.Get("Command_ClockIn")!, ClockInAsync));

		// Rank the experience point.
		// e.g. User sends the message:
		//     @bot /rank
		bot.RegisterCommand(new(StringResource.Get("Command_Rank")!, PrintRankResultAsync));

		// Prints the information about the author of the bot, and the bot itself.
		// e.g. User sends the message:
		//     @bot /about
		bot.RegisterCommand(new(StringResource.Get("Command_About")!, PrintAboutInfoAsync));

		// Repeats the message.
		// e.g. User sends the message:
		//     @bot /repeat 123
		// Then the bot will reply:
		//     @user 123
		// Empty content won't be repeated by bot.
		bot.RegisterCommand(new(StringResource.Get("Command_Repeat")!, RepeatAsync));
	}

	/// <summary>
	/// Loads the configuration file from local path.
	/// </summary>
	/// <param name="localFilePath">The local path storing the configuration of the bot.</param>
	/// <param name="testChannelId">Indicates the test channel ID used.</param>
	/// <returns>The <see cref="BotIdentity"/> instance.</returns>
	private static BotIdentity LoadBotConfiguration(string localFilePath, out string testChannelId)
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
				nameof(appid) => appid = realValue,
				nameof(token) => token = realValue,
				nameof(secret) => secret = realValue,
				nameof(testChannelId) => testChannelId = realValue,
				_ => default
			};
		}

		return new(appid, token, secret);
	}
}
