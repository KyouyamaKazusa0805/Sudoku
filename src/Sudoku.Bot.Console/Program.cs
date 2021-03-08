using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using HuajiTech.Mirai.Http;
using HuajiTech.Mirai.Http.Events;
using Sudoku.Bot;
using Sudoku.Bot.Console.CommandLines;
using Sudoku.Bot.Resources;
using Sudoku.Bot.Serialization;
using Sudoku.Drawing;

//
// Constant declarations.
//
const int port = 8080;
const string ipAddress = "127.0.0.1";
const string authKey = "1234567890";
const string dateTimeFormat = "yyyy-MM-dd HH:mm:ss";
string banner = "-".PadRight(40, '-'); /*readonly*/

//
// Check the path and the bot number.
//
if (!ArgChecker.TryGetPath(args, out string? path))
{
	return;
}

string pathSettingsFile = $@"{path}\Settings.json";
if (!File.Exists(pathSettingsFile))
{
	return;
}

string pathNumberFile = $@"{path}\Number.txt";
if (!File.Exists(pathNumberFile) || !long.TryParse(File.ReadAllText(pathNumberFile), out long number))
{
	return;
}

long numberAllowedInConfig = -1;
string pathAllowInConfigModeFile = $@"{path}\AllowInConfigMode.txt";
if (File.Exists(pathAllowInConfigModeFile)
	&& !long.TryParse(File.ReadAllText(pathAllowInConfigModeFile), out numberAllowedInConfig))
{
	numberAllowedInConfig = -1;
}

//
// De-serialize the configuration file.
//
SudokuPlugin.Settings = JsonSerializer.Deserialize<Settings>(
	File.ReadAllText(pathSettingsFile),
	SerializationOptions.Default
) ?? new();

try
{
	//
	// Open the bot.
	//
	var sessionSettings = new SessionSettings(ipAddress, port, authKey);
	await using var session = new Session(sessionSettings, number);
	await session.ConnectAsync();

	//
	// Instantiate the event sources, and enable the listening operation.
	//
	var groupReceivedSource = new GroupMessageReceivedEventSource();
	var handler = session.ApiEventHandler;
	handler.Bind(groupReceivedSource);
	await handler.ListenAsync();

	//
	// Say hello to groups when possible.
	//
	bool configMode = ArgChecker.IsConfigMode(args);
	if (ArgChecker.EnableGreeting(args) && !configMode)
	{
		var user = session.CurrentUser;

		string pathBlackList = $@"{path}\BlackList.txt";
		string[] groupStrs = File.Exists(pathBlackList) ? File.ReadAllLines(pathBlackList) : Array.Empty<string>();

		var groups = await user.GetGroupsAsync();
		foreach (var g in from g in groups where !groupStrs.Contains(g.Number.ToString()) select g)
		{
			string info = new StringBuilder()
				.Append((string)TextResources.Current.Evenybody)
				.Append(
					(string)(DateTime.Now.Hour switch
					{
						>= 0 and < 4 => TextResources.Current.TimeModifierBehindMidnight,
						>= 4 and < 7 => TextResources.Current.TimeModifierDawn,
						>= 7 and < 11 => TextResources.Current.TimeModifierMorning,
						>= 11 and < 13 => TextResources.Current.TimeModifierNoon,
						>= 13 and < 17 => TextResources.Current.TimeModifierAfternoon,
						>= 17 and < 19 => TextResources.Current.TimeModifierNightfall,
						>= 19 and < 23 => TextResources.Current.TimeModifierEvening,
						23 => TextResources.Current.TimeModifierForeMidnight,
					})
				)
				.Append((string)TextResources.Current.Greeting)
				.ToString();

			await g.SendAsync(info);
		}
	}

	//
	// Instantiate a new plugin.
	//
	ArgChecker.TryGetSize(args, out int size);
	SudokuPlugin.Start(groupReceivedSource, path, configMode, size, numberAllowedInConfig);

	//
	// Output the console information.
	//
	Console.WriteLine(banner);
	Console.Write(TextResources.Current.ProgramName);
	Console.WriteLine(configMode ? TextResources.Current.ConfigMode : string.Empty);
	Console.WriteLine(banner);
	Console.WriteLine();
	Console.Write(TextResources.Current.NowTime);
	Console.WriteLine(DateTime.Now.ToString(dateTimeFormat, null));
	Console.WriteLine();
	Console.WriteLine(TextResources.Current.OpenSuccessful);
	Console.WriteLine(TextResources.Current.PleaseInputEnterKeyToQuit);
	Console.ReadLine();

	//
	// User has closed the program. Now save the configuration.
	//
	if (!Directory.Exists(path))
	{
		Directory.CreateDirectory(path);
	}

	File.WriteAllText(
		pathSettingsFile,
		JsonSerializer.Serialize(SudokuPlugin.Settings, SerializationOptions.Default)
	);
}
catch (Exception ex)
{
	Console.WriteLine((string)TextResources.Current.ErrorWhileOpening);

	Console.ForegroundColor = ConsoleColor.Red;
	Console.WriteLine(ex.Message);
	Console.ResetColor();
}
