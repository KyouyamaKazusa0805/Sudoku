#pragma warning disable IDE1006

using System;
using HuajiTech.Mirai;
using HuajiTech.Mirai.Events;
using Sudoku.Bot;

// See the link for more information.
// https://gitlab.huajitech.net/huajitech/mirai-http-dotnet-sdk/snippets/2

var sessionSettings = new SessionSettings(address: "127.0.0.1", port: 8080, authKey: "1234567890");
await using var session = new Session(sessionSettings, number: 2222097152L);
await session.ConnectAsync();

var currentUserEventSource = new CurrentUserEventSource();
var handler = session.ApiEventHandler;
handler.Bind(currentUserEventSource);
await handler.ListenAsync();

var plugin = new SudokuPlugin(currentUserEventSource);

Console.ReadLine();