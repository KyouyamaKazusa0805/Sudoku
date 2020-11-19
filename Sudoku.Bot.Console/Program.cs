#if AUTHOR_RESERVED

#pragma warning disable IDE1006

using System;
using System.IO;
using System.Text.Json;
using HuajiTech.Mirai;
using HuajiTech.Mirai.Events;
using Sudoku.Bot;
using Sudoku.Bot.Constants;

const string pluginConfigPath = @"P:\Bot\插件设置.json";
const long myNumber = (long)(222 * 1E7M) + (1 << 21);

// Enable the bot.
var sessionSettings = new SessionSettings("127.0.0.1", 8080, "1234567890");
await using var session = new Session(sessionSettings, myNumber);
await session.ConnectAsync();

var currentUserEventSource = new CurrentUserEventSource();
var handler = session.ApiEventHandler;
handler.Bind(currentUserEventSource);
await handler.ListenAsync();

var plugin = new SudokuPlugin(currentUserEventSource);

// Output the information.
Console.WriteLine("启动成功，可以愉快地使用啦！");
Console.WriteLine("（输入换行符退出程序）");
Console.ReadLine();

// Dispose resources.
string dir = Path.GetDirectoryName(pluginConfigPath)!;
DirectoryEx.CreateIfDoesNotExist(dir);

try
{
	string json = JsonSerializer.Serialize(SudokuPlugin.Settings, Processings.SerializerOption);
	File.WriteAllText(pluginConfigPath, json);
}
catch
{
}

#endif