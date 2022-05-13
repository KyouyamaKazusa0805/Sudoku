// Configures the log level.
Log.LogLevel = LogLevel.Info;

// Initializes an identity instance.
// Please note that the token and secret code corresponds to the info for the bot.
// For more information please visit the link https://q.qq.com/bot/#/home
var identity = LoadBotConfiguration(StringResource.Get("__LocalBotConfigPath")!, out string testChannelId);

// Initializes a bot client.
var bot = new BotClient(identity, sandBoxApi: true, reportApiError: true)
{
	Intents = Intents.PrivateDomain,
	CommandPrefix = "/",
	MessageFilter = sender => sender.ChannelId != testChannelId
};

// Sets the elementary event handlers.
InitializeEvents(bot);

// Registers commands.
RegisterCommands(bot);

// Starts the bot with the specified number of trial times and callback functions.
bot.StartAsync(3, OnSuccessfullyConnected, OnFailedConnected);

// Here we should use an infinite loop to make the console don't exit fast.
// Issue: https://github.com/Antecer/QQChannelBot/issues/1
while (true)
{
	await Task.Delay(1000);
}


/// <summary>
/// Indicates the entry point of the program.
/// </summary>
internal static partial class Program
{
	private static async void RepeatAsync(Sender sender, string message)
	{
		if (!string.IsNullOrWhiteSpace(message) && sender.AtMe)
		{
			await sender.ReplyAsync($"{sender.Author.Tag} {message}");
		}
	}

	private static async void PrintAboutInfoAsync(Sender sender, string message)
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

	private static async void ClockInAsync(Sender sender, string message)
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
			await sender.ReplyAsync($"{a} {extraExp} {b}", true);
		}
	}

	private static async void PrintRankResultAsync(Sender sender, string message)
	{
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

		string[] filePaths = (
			from fileInfo in new DirectoryInfo(path).GetFiles()
			where fileInfo.Extension == ".json"
			select fileInfo.FullName
		).ToArray();
		if (filePaths.Length == 0)
		{
			// No configuration files found.
			await sender.ReplyAsync(StringResource.Get("RankExpFailed_NoConfigFileFound")!);
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
}
