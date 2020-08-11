#if AUTHOR_RESERVED
#pragma warning disable IDE1006 // Global main method cannot fix the name suffix 'Async'.

using System;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Sudoku.Bot;

// TODO: Compile Mirai console and then fill the values.
int port = default;
var options = new MiraiHttpSessionOptions("ip", port, "token");
await using var session = new MiraiHttpSession();
var plugin = new ExamplePlugin();
session.AddPlugin(plugin);
await session.ConnectAsync(options, 979329690L);
while (true)
{
	if (await Console.In.ReadLineAsync() is "exit" or "quit")
	{
		return;
	}
}

#endif