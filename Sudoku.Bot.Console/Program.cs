#if AUTHOR_RESERVED

#pragma warning disable IDE1006

using System;
using HuajiTech.Mirai;
using HuajiTech.Mirai.Events;
using Sudoku.Bot;

const long myQQ = (long)(222 * 1E7M) + (1 << 21);

var sessionSettings = new SessionSettings("127.0.0.1", 8080, "1234567890");
await using var session = new Session(sessionSettings, myQQ);
await session.ConnectAsync();

var currentUserEventSource = new CurrentUserEventSource();
var handler = session.ApiEventHandler;
handler.Bind(currentUserEventSource);
await handler.ListenAsync();

var plugin = new SudokuPlugin(currentUserEventSource);

Console.WriteLine("启动成功，可以愉快地使用啦！");
Console.WriteLine("（输入换行符退出程序）");
Console.ReadLine();

#endif