// Configures the log level.
Log.LogLevel = LogLevel.Info;

// Outputs the copyright information.
Log.Info(
	$"""
	{StringResource.Get("__ProjectCopyrightStatement")!}
	
	
	"""
);

// Initializes an identity instance.
// Please note that the token and secret code corresponds to the info for the bot.
// For more information please visit the link https://q.qq.com/bot/#/home
var identity = LoadBotConfiguration(StringResource.Get("__LocalBotConfigPath")!, out string testChannelId);

// Initializes a bot client.
var bot = new BotClient(identity, sandBoxApi: true, reportApiError: true)
{
	Intents = Intents.PrivateDomain,
	MessageFilter = sender => sender.ChannelId == testChannelId && sender.IsMentioned
};

// Sets the elementary event handlers.
InitializeEvents(bot);

// Registers commands.
registerCommands(bot);

// Starts the bot with the specified number of trial times and callback functions.
bot.StartAsync(3, OnSuccessfullyConnected, OnFailedConnected);

// Here we should use an infinite loop to make the console don't exit fast.
// Issue: https://github.com/Antecer/QQChannelBot/issues/1
while (true)
{
	await Task.Delay(1000);
}


static void registerCommands(BotClient bot)
{
	bot.RegisterCommand(new(StringResource.Get("Command_Sudoku")!, PlaySudokuAsync));
	bot.RegisterCommand(new(StringResource.Get("Command_ClockIn")!, ClockInAsync));
	bot.RegisterCommand(new(StringResource.Get("Command_Rank")!, PrintRankResultAsync));
	bot.RegisterCommand(new(StringResource.Get("Command_About")!, PrintAboutInfoAsync));
}

static async void PlaySudokuAsync(Sender sender, string message)
{
	if ((sender, message) is not ({ IsMentioned: true }, (false, false, false)))
	{
		return;
	}

	if (
		!new[] { StringResource.Get("Command_Sudoku_PkMode1")!, StringResource.Get("Command_Sudoku_PkMode2")! }
			.Contains(message, new IgnoreCasingStringEqualityComparer())
	)
	{
		return;
	}

	// Versus mode.
	// Now we leave about 40 seconds to ensure at least 2 players can join in the arena.
	_currentLockCommand = StringResource.Get("Command_Sudoku")!;
	_timeLast = 40;

	await sender.ReplyAsync(StringResource.Get("CommandStartWaitingPlayers")!);

	// Defines a timer to wait for 40 seconds.
	var timer = new Timer(1000);
	timer.Elapsed += (_, _) => _timeLast--;
	timer.Start();

	// Wait for players joining in the game.
	while (_timeLast > 0 && _playersJoined.Count < 2)
	{
	}

	// Stop the timer and release the memory.
	timer.Stop();
	timer.Close();

	// Checks the result.
	switch ((_timeLast, _playersJoined))
	{
		case ( <= 0, { Count: < 2 }):
		{
			// Players not enough.
			await sender.ReplyAsync(StringResource.Get("CommandGamePlayFailed")!);
			goto default;
		}
		case (_, { Count: >= 2 }):
		{
			// Game start.
			break;
		}
		default:
		{
			// Reset the values.
			_currentLockCommand = null;
			_playersJoined.Clear();

			return;
		}
	}

	// Create game.
	var generator = new HardPatternPuzzleGenerator();

	// TODO: Implement the game. Here we should implement the drawing items.
}

static async void ClockInAsync(Sender sender, string message)
{
	// Checks the user data.
	// If the user is the new, we should create a file to store it;
	// otherwise, check the user data file, and get the latest time that he/she clocked in.
	// If the 'yyyyMMdd' is same, we should disallow the user re-clocking in.
	if (sender is not { MessageCreator.Id: var userId, IsMentioned: true })
	{
		return;
	}

	if (!string.IsNullOrWhiteSpace(message))
	{
		await sender.ReplyAsync(StringResource.Get("CommandParserError_CommandNotRequireParameter")!);
		return;
	}

	if (_currentLockCommand is not null)
	{
		await sender.ReplyAsync(StringResource.Get("CommandExecutionFailed_LockedCommandIsNotEmpty")!);
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
	static bool isWeekend() => DateTime.Today.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
	static int getRandomScore() => Random.Shared.Next() % (isWeekend() ? 20 : 10) + 1;
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
					int extraExp = getRandomScore();
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
					string d = isWeekend() ? $" {StringResource.Get("HopeYouHappyWeekend")!}" : string.Empty;
					await sender.ReplyAsync($"{a} {extraExp} {b} {realFinalValue} {c}{d}", true);
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
				int extraExp = getRandomScore();

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
				string d = isWeekend() ? $" {StringResource.Get("HopeYouHappyWeekend")!}" : string.Empty;
				await sender.ReplyAsync($"{a} {extraExp} {b} {realFinalValue} {c}{d}", true);
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
		int extraExp = getRandomScore();

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
		string c = isWeekend() ? $" {StringResource.Get("HopeYouHappyWeekend")!}" : string.Empty;
		await sender.ReplyAsync($"{a} {extraExp} {b}{c}", true);
	}
}

static async void PrintRankResultAsync(Sender sender, string message)
{
	if (sender is not { MessageCreator.Id: var userId, IsMentioned: true })
	{
		return;
	}

	if (!string.IsNullOrWhiteSpace(message))
	{
		await sender.ReplyAsync(StringResource.Get("CommandParserError_CommandNotRequireParameter")!);
		return;
	}

	if (_currentLockCommand is not null)
	{
		await sender.ReplyAsync(StringResource.Get("CommandExecutionFailed_LockedCommandIsNotEmpty")!);
		return;
	}

	if (StringResource.Get("__LocalPlayerConfigPath") is not { } path)
	{
		// The configuration path is not found.
		await sender.ReplyAsync(StringResource.Get("ClockInError_ResourceNotFound")!);
		return;
	}

	string[] filePaths = (
		from fileInfo in new DirectoryInfo(path).GetFiles()
		where fileInfo.Extension == ".json"
		select fileInfo.FullName
	).ToArray();
	if (filePaths.Length == 0)
	{
		// No configuration files found.
		await sender.ReplyAsync(StringResource.Get("RankExpFailed_NoConfigFileFound")!);
		return;
	}

	var rankingPairs = (
		from filePath in filePaths
		let jsonString = File.ReadAllText(filePath)
		let rootElement = JsonSerializer.Deserialize<JsonElement>(jsonString)
		let exp = (
			from jsonElement in rootElement.EnumerateObject()
			where jsonElement.Name == ConfigurationPaths.ExperiencePoint
			let expStr = jsonElement.Value.ToString()
			where int.TryParse(expStr, out _)
			select int.Parse(expStr)
		).FirstOrDefault()
		let id = Path.GetFileNameWithoutExtension(filePath)
		orderby exp descending, id
		select (Id: id, ExperiencePoint: exp)
	).ToArray();

	(string Id, int ExperiencePoint)? pair = null;
	var sb = new StringBuilder();
	for (int rankingOrder = 0, i = 0, rankingPairsLength = rankingPairs.Length; i < rankingPairsLength; i++)
	{
		var (id, exp) = rankingPairs[i];

		var member = await sender.GetMemberAsync(new() { Id = id });
		string nickname = member?.Nickname ?? $"<{id}>";
		string expText = StringResource.Get("ExperiencePointText")!;
		string comma = StringResource.Get("Comma")!;
		string colon = StringResource.Get("Colon")!;
		string di = StringResource.Get("Di")!;
		string ming = StringResource.Get("Ming")!;

		// Special case: if the current user has a same experience point value with the former one,
		// we should treat him/her as same ranking value as the former one.
		int finalRankingOrder = pair is (_, var formerExp) && formerExp == exp ? rankingOrder : ++rankingOrder;
		sb.AppendLine($"{di} {finalRankingOrder} {ming}{colon}{nickname}{comma}{exp} {expText}");

		pair = (id, exp);
	}

	await sender.ReplyAsync(
		$"""
		{StringResource.Get("RankExpSuccessful_Segment1")!}
		---
		{sb}
		"""
	);
}

static async void PrintAboutInfoAsync(Sender sender, string message)
{
	if (!sender.IsMentioned)
	{
		return;
	}

	if (!string.IsNullOrWhiteSpace(message))
	{
		await sender.ReplyAsync(StringResource.Get("CommandParserError_CommandNotRequireParameter")!);
		return;
	}

	if (_currentLockCommand is not null)
	{
		await sender.ReplyAsync(StringResource.Get("CommandExecutionFailed_LockedCommandIsNotEmpty")!);
		return;
	}

	await sender.ReplyAsync(
		$"""
		{StringResource.Get("AboutInfo_Segment1")!}{AssemblyVersion}
		---
		{StringResource.Get("AboutInfo_Segment2")!}
		"""
	);
}
