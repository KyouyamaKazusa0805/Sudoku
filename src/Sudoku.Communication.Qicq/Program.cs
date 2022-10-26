using var bot = new MiraiBot { Address = "localhost:8080", QQ = "979329690", VerifyKey = "helloworld" };
await bot.LaunchAsync();

bot.MessageReceived.OfType<GroupMessageReceiver>().Subscribe(onGroupMessageReceiving);

// Blocks the main thread, in order to avoid main thread returning too fast.
var signal = new ManualResetEvent(false);
signal.WaitOne();


static async void onGroupMessageReceiving(GroupMessageReceiver e)
{
	if (e.Sender.Id == "747507738")
	{
		await e.SendMessageAsync("Hello, world!");
	}
}
