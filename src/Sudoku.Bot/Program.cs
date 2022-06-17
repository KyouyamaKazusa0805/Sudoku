/// <summary>
/// Indicates the entry point of the program.
/// </summary>
internal static partial class Program
{
#pragma warning disable IDE0044
	/// <summary>
	/// Indicates the current locked command.
	/// This field is reserved by author to define the long-term command. If the field is not <see langword="null"/>,
	/// what the method executed, what name the field will store.
	/// </summary>
	private static volatile string? _currentLockCommand;

	/// <summary>
	/// Indicates the time last.
	/// This field is used by controlling the time span that group-mates can join in the game.
	/// </summary>
	private static volatile int _timeLast;

	/// <summary>
	/// Indicates the players having been joined into the game.
	/// </summary>
	private static volatile List<string> _playersJoined = new();
#pragma warning restore IDE0044


	/// <summary>
	/// Indicates the assembly version.
	/// </summary>
	private static Version? AssemblyVersion => typeof(Program).Assembly.GetName().Version;


	/// <summary>
	/// Indicates callback method that will be invoked when the bot has successfully connected to the server,
	/// and successfully passed the authorization.
	/// </summary>
	private static void OnSuccessfullyConnected() => Logging.Info(R["BotConnectedCallbackOutput_Success"]!);

	/// <summary>
	/// Indicates callback method that will be invoked when the bot has failed connected to the server,
	/// or failed passed the authorization.
	/// </summary>
	private static void OnFailedConnected() => Logging.Warn(R["BotConnectedCallbackOutput_Fail"]!);

	/// <summary>
	/// Exits the program fast.
	/// </summary>
	/// <param name="bot">The bot client which should be closed.</param>
	private static void FastExitOnBot(BotClient bot)
	{
		bot.Close();
		BotHttpClient.HttpClient.Dispose();

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
				string suffix = R["PrivateDomainBotSuffix"]!;
				string sdkVersionName = R["SdkVersionName"]!;
				string colon = R["Colon"]!;

				Console.Title = $"{userName}{suffix} <{id}> - {sdkVersionName}{colon}{sdkName}_{sdkVersion}";
			}
		};
		bot.MessageCreated += static async (_, e) =>
		{
			if (_currentLockCommand != R["Command_Sudoku"])
			{
				return;
			}

			if (e.Sender is not { Bot.Info: var botInfo, IsMentioned: true, Content: var m, MessageCreator.Id: var creatorId } sender)
			{
				return;
			}

			// Filters the message to remove the mentioning message part.
			if (!string.IsNullOrWhiteSpace(MessageContent.RemoveTag(m, botInfo)))
			{
				return;
			}

			// Here we should register the joining operation.
			_playersJoined.Add(creatorId);
			await sender.ReplyAsync(R["CommandJoinGameSuccess"]!);
		};
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
