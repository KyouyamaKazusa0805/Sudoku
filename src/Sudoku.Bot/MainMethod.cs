// Configures the log level.
Log.LogLevel = LogLevel.Info;

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

// Starts the bot with the specified number of trial times and callback functions.
bot.StartAsync(3, onSuccessfullyConnected, onFailedConnected);

// Here we should use an infinite loop to make the console don't exit fast.
// Issue: https://github.com/Antecer/QQChannelBot/issues/1
while (true)
{
	await Task.Delay(1000);
}


static void onSuccessfullyConnected() => Console.WriteLine(StringResource.Get("BotConnectedCallbackOutput_Success")!);

static void onFailedConnected() => Console.WriteLine(StringResource.Get("BotConnectedCallbackOutput_Fail")!);

static void fastExitOnBot(BotClient bot)
{
	bot.Close();
	Environment.Exit(0);
}

static void initializeEvents(BotClient bot)
{
	Console.CancelKeyPress += (_, _) => fastExitOnBot(bot);

	bot.AuthorizationPassed += onAuthorizationPassed;
	bot.MessageCreated += onMessageCreated;
}

static void onAuthorizationPassed(BotClient? _, AuthoizationPassedEventArgs e)
{
	if (e is { User: { UserName: var userName, Id: var id } user }
		&& typeof(BotClient).Assembly.GetName() is { Name: var sdkName, Version: var sdkVersion })
	{
		string suffix = StringResource.Get("PrivateDomainBotSuffix")!;
		string sdkVersionName = StringResource.Get("SdkVersionName")!;
		string colon = StringResource.Get("Colon")!;

		Console.Title = $"{userName}{suffix} <{id}> - {sdkVersionName}{colon}{sdkName}_{sdkVersion}";
	}
}

static async void onMessageCreated(BotClient? _, MessageCreatedEventArgs e)
{
	if (e.Sender is { AtMe: true, Content: var content, Bot.Info.Tag: var botInfoTag, Author.Tag: var authorTag } sender)
	{
		await sender.ReplyAsync(content.Replace(botInfoTag, authorTag));
	}
}

static void initializeCommands(BotClient bot)
{
	// Repeats the message.
	// e.g. User sends the message:
	//     @bot /repeat 123
	// Then the bot will reply:
	//     @user 123
	// Empty content won't be repeated by bot.
	bot.AddCommand(new(StringResource.Get("Command_Repeat")!, repeatAsync));

	// Prints the information about the author of the bot, and the bot itself.
	// e.g. User sends the message:
	//     @bot /about
	bot.AddCommand(new(StringResource.Get("Command_About")!, printAboutInfoAsync));

	// Clock in.
	// e.g. User sends the message:
	//     @bot /clockIn
	bot.AddCommand(new(StringResource.Get("Command_ClockIn")!, clockInAsync));
}

static async void repeatAsync(Sender sender, string message)
{
	if (!string.IsNullOrWhiteSpace(message) && sender.AtMe)
	{
		await sender.ReplyAsync($"{sender.Author.Tag} {message}");
	}
}

static async void printAboutInfoAsync(Sender sender, string message)
{
	if (sender.AtMe)
	{
		await sender.ReplyAsync(
			$"""
			{StringResource.Get("AboutInfo_Segment1")!}{AssemblyVersion}
			---
			{StringResource.Get("AboutInfo_Segment2")!}
			"""
		);
	}
}

static async void clockInAsync(Sender sender, string message)
{
	// Checks the user data.
	// If the user is the new, we should create a file to store it;
	// otherwise, check the user data file, and get the latest time that he/she clocked in.
	// If the 'yyyyMMdd' is same, we should disallow the user re-clocking in.
	if (sender is not { Author.Id: var userId, AtMe: true })
	{
		return;
	}

	if (StringResource.Get("__LocalPlayerConfigPath") is not { } path)
	{
		// The configuration path is not found.
		await sender.ReplyAsync(StringResource.Get("ClockInError_ResourceNotFound")!);
		return;
	}

	if (!Directory.Exists(path))
	{
		// If the local path doesn't exist, create it.
		Directory.CreateDirectory(path);
	}

	string userPath = $"""{path}\{userId}.json""";
	var generalSerializerOptions = new JsonSerializerOptions { WriteIndented = true }.AddConverter<DictionaryConverter>();
	if (File.Exists(userPath))
	{
		string originalJson = await File.ReadAllTextAsync(userPath);
		var rootElement = JsonSerializer.Deserialize<JsonElement>(originalJson);
		var foundPairs = rootElement.TryGetPropertyValues(ConfigurationPaths.LastTimeCheckedIn, ConfigurationPaths.ExperiencePoint);

		static bool checkInPredicate((string Name, string?) e) => e.Name == ConfigurationPaths.LastTimeCheckedIn;
		static bool expPredicate((string Name, string?) e) => e.Name == ConfigurationPaths.ExperiencePoint;
		if (foundPairs is not null)
		{
			if (Array.FindIndex(foundPairs, checkInPredicate) is var checkInIndex and not -1)
			{
				try
				{
					var date = DateTime.Parse(foundPairs[checkInIndex].CorrespondingValue!).Date;
					if (date == DateTime.Today)
					{
						// A same day. Notice the user that he/she cannot clock in again.
						await sender.ReplyAsync(StringResource.Get("ClockInWarning_CannotClockInInSameDay")!);
						return;
					}

					// Valid. Now clock in and update the value.
					int extraExp = Random.Shared.Next() % 10 + 1;
					var tempDic = rootElement
						.ToDictionaryOfStringElements()
						.AppendOrCover(ConfigurationPaths.LastTimeCheckedIn, DateTime.Today.ToString());
					var (realFinalValue, dictionary) = expPropertyExists(extraExp, out int finalValue) switch
					{
						true => (
							finalValue,
							tempDic.AppendOrCover(ConfigurationPaths.ExperiencePoint, finalValue.ToString())
						),
						_ => (
							extraExp,
							tempDic.AppendOrCover(ConfigurationPaths.ExperiencePoint, extraExp.ToString())
						)
					};
					string updatedJson = JsonSerializer.Serialize(dictionary, generalSerializerOptions);

					await File.WriteAllTextAsync(userPath, updatedJson);

					string a = StringResource.Get("ClockInSuccess_ValueUpdatedSegment1")!;
					string b = StringResource.Get("ClockInSuccess_ValueUpdatedSegment2")!;
					string c = StringResource.Get("ClockInSuccess_ValueUpdatedSegment3")!;
					await sender.ReplyAsync($"{a} {extraExp} {b} {realFinalValue} {c}");
				}
				catch (ArgumentNullException)
				{
					await sender.ReplyAsync(StringResource.Get("ClockInError_DateStringIsNull")!);
				}
				catch (Exception ex) when (ex is FormatException or InvalidOperationException)
				{
					// Invalid date value parsed.
					await sender.ReplyAsync(StringResource.Get("ClockInError_InvalidDateValue")!, true);
				}
			}
			else
			{
				// The file exists but the current property doesn't.
				// We should create the property with the current value and update the file.
				int extraExp = Random.Shared.Next() % 10 + 1;

				var tempDic = rootElement
					.ToDictionaryOfStringElements()
					.AppendOrCover(ConfigurationPaths.LastTimeCheckedIn, DateTime.Today.ToString());
				var (realFinalValue, dictionary) = expPropertyExists(extraExp, out int finalValue) switch
				{
					true => (
						finalValue,
						tempDic.AppendOrCover(ConfigurationPaths.ExperiencePoint, finalValue.ToString())
					),
					_ => (
						extraExp,
						tempDic.AppendOrCover(ConfigurationPaths.ExperiencePoint, extraExp.ToString())
					)
				};
				string updatedJson = JsonSerializer.Serialize(dictionary, generalSerializerOptions);

				await File.WriteAllTextAsync(userPath, updatedJson);

				string a = StringResource.Get("ClockInSuccess_ValueCreatedSegment1")!;
				string b = StringResource.Get("ClockInSuccess_ValueCreatedSegment2")!;
				string c = StringResource.Get("ClockInSuccess_ValueCreatedSegment3")!;
				await sender.ReplyAsync($"{a} {extraExp} {b} {realFinalValue} {c}", true);
			}
		}


		bool expPropertyExists(int extraExp, out int finalValue)
		{
			Unsafe.SkipInit(out finalValue);

			if (Array.FindIndex(foundPairs, expPredicate) is var expIndex and not -1
				&& int.Parse(foundPairs[expIndex].CorrespondingValue!) + extraExp is var f)
			{
				finalValue = f;
				return true;
			}
			else
			{
				return false;
			}
		}
	}
	else
	{
		// The file doesn't exist. Just create a file that contains the data.
		int extraExp = Random.Shared.Next() % 10 + 1;

		string indenting = new(' ', 2);
		await File.WriteAllTextAsync(
			userPath,
			$$"""
			{
			{{indenting}}"{{ConfigurationPaths.LastTimeCheckedIn}}": "{{DateTime.Today}}",
			{{indenting}}"{{ConfigurationPaths.ExperiencePoint}}": "{{extraExp}}"
			}
			"""
		);

		string a = StringResource.Get("ClockInSuccess_FileCreatedSegment1")!;
		string b = StringResource.Get("ClockInSuccess_FileCreatedSegment2")!;
		await sender.ReplyAsync($"{a} {extraExp} {b}", true);
	}
}

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
