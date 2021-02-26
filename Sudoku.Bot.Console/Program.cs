using System;
using System.IO;
using System.Text.Json;
using HuajiTech.Mirai.Http;
using HuajiTech.Mirai.Http.Events;
using Sudoku.Bot;
using Sudoku.Bot.Console.CommandLines;
using Sudoku.Bot.Resources;
using Sudoku.Bot.Serialization;
using Sudoku.Drawing;

#region Constant declarations
const int port = 8080;
const string ipAddress = "127.0.0.1";
const string authKey = "1234567890";
const string dateTimeFormat = "yyyy-MM-dd HH:mm:ss";
string banner = "-".PadRight(40, '-'); /*readonly*/
#endregion

#region Get the bot number
if (getNumber() is not { } number)
{
	return;
}
#endregion

#region Check whether the mode is the config mode
bool configMode = getConfigModeFromArgs(args);
string? pathSettings = getPathSettingsFromArgs();
#endregion

#region De-serialize the configuration file
if (pathSettings is not null && File.Exists(pathSettings))
{
	// Read the configuration.
	string json = File.ReadAllText(pathSettings);
	SudokuPlugin.Settings = JsonSerializer.Deserialize<Settings>(json, SerializationOptions.Default) ?? new();
}
#endregion

try
{
	#region Open the bot
	var sessionSettings = new SessionSettings(ipAddress, port, authKey);
	await using var session = new Session(sessionSettings, number);
	await session.ConnectAsync();
	#endregion

	#region Instantiate the event sources, and enable the listening operation
	var currentUserEventSource = new GroupMessageReceivedEventSource();
	var handler = session.ApiEventHandler;
	handler.Bind(currentUserEventSource);
	await handler.ListenAsync();
	#endregion

	#region Global greeting
	// If you want to add a greeting plugin, put it to here.
	#endregion

	#region Instantiate a new plugin
	var sudokuPlugin = new SudokuPlugin(currentUserEventSource);
	#endregion

	#region Output the console information
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
	#endregion

	try
	{
		#region Check whether the configuration file path is valid
		if (pathSettings is null)
		{
			outputInfo(TextResources.Current.PathDoesNotExist);
			return;
		}
		#endregion

		#region User has closed the program. Now save the configuration
		string dir = Path.GetDirectoryName(pathSettings)!;
		if (!Directory.Exists(dir))
		{
			Directory.CreateDirectory(dir);
		}

		string json = JsonSerializer.Serialize(SudokuPlugin.Settings, SerializationOptions.Default);
		File.WriteAllText(pathSettings, json);
		#endregion
	}
	catch (Exception ex)
	{
		outputException(ex, TextResources.Current.ErrorWhileSavingSettings);
	}
}
catch (Exception ex)
{
	outputException(ex, TextResources.Current.ErrorWhileOpening);
}


#region Local functions
static void outputInfo(string info)
{
	Console.ForegroundColor = ConsoleColor.Yellow;
	Console.WriteLine(info);
	Console.ResetColor();
}

static void outputException(Exception ex, string? header = null)
{
	if (header is not null)
	{
		Console.WriteLine(header);
	}

	Console.ForegroundColor = ConsoleColor.Red;
	Console.WriteLine(ex.Message);
	Console.ResetColor();
}

long? getNumber()
{
	if (Array.IndexOf(args, Commands.NumberPath) is var index and not -1
		&& args.Length >= index + 1
		&& args[index + 1] is var p
		&& File.Exists(p)
		&& File.ReadAllText(p) is var numberStr
		&& long.TryParse(numberStr, out long result))
	{
		return result;
	}
	else
	{
		return null;
	}
}

bool getConfigModeFromArgs() => args.Length != 0 && Array.IndexOf(args, Commands.ConfigMode) != -1;

string? getPathSettingsFromArgs()
{
	if (Array.IndexOf(args, Commands.SettingsPath) is var index and not -1
		&& index + 1 < args.Length
		&& args[index + 1] is var path
		&& Directory.Exists(path))
	{
		return $@"{path}\Settings.json";
	}
	else
	{
		return null;
	}
}
#endregion
