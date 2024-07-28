#pragma warning disable IDE0011

using System;
using System.IO;
using System.Text.Json;
using MyBot.Api;
using MyBot.Datas;
using MyBot.Expansions.Bot;
using Sudoku.Bot.Localization;

var filePath = @"A:\QQ机器人\bot.json";
var json = File.ReadAllText(filePath);
var botInfo = JsonSerializer.Deserialize<BotInfo>(json, JsonOptions)!;
var accessInfo = new OpenApiAccessInfo { BotQQ = botInfo.Id, BotAppId = botInfo.AppId, BotToken = botInfo.Token, BotSecret = botInfo.Secret };
var channelApi = new QQChannelApi(accessInfo);
channelApi.UseBotIdentity();
channelApi.UseSandBoxMode();

var bot = new ChannelBot(channelApi);
bot.UsePrivateBot();
bot.RegisterAtMessageEvent();
bot.OnConnected += bot_OnConnected;
bot.AuthenticationSuccess += bot_AuthenticationSuccess;
bot.OnError += bot_OnError;

bot.RegisterCommand(
	"test",
	async commandInfo =>
	{
		var message = channelApi.GetMessageApi();
		await message.SendTextMessageAsync(commandInfo.ChannelId, "成功", commandInfo.MessageId);
	}
);

await bot.OnlineAsync();

Console.WriteLine("请按 Q 键退出机器人。");

while (Console.ReadLine() is not ("Q" or "q")) ;

Console.WriteLine("退出机器人。");


static void bot_OnConnected() => Console.WriteLine("连接机器人成功。");

static void bot_AuthenticationSuccess() => Console.WriteLine("机器人鉴权成功。");

static void bot_OnError(Exception ex)
{
	Console.ForegroundColor = ConsoleColor.Red;
	Console.WriteLine($"机器人执行指令时出现错误：{ex.Message}");
	Console.ResetColor();
}


file static partial class Program
{
	private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
}
