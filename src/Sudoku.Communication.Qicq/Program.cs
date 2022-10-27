using static LocalResourceFetcher;

// Creates and initializes a bot.
using var bot = new MiraiBot { Address = GetString("HostPort"), QQ = GetString("BotQQ"), VerifyKey = GetString("VerifyKey") };
await bot.LaunchAsync();

// Registers some necessary events.
bot.MessageReceived.OfType<GroupMessageReceiver>().Subscribe(onGroupMessageReceiving);
bot.EventReceived.OfType<MemberJoinedEvent>().Subscribe(onMemberJoined);

// Blocks the main thread, in order to prevent the main thread exits too fast.
var signal = new ManualResetEvent(false);
signal.WaitOne();


static async void onMemberJoined(MemberJoinedEvent e)
{
	if (e.Member.Group is { Id: var id } group && id == GetString("SudokuGroupQQ"))
	{
		await group.SendGroupMessageAsync(GetString("SampleMemberJoinedMessage"));
	}
}

static async void onGroupMessageReceiving(GroupMessageReceiver e)
{
	// Test code.
	if (e.Sender.Id == GetString("AdminQQ"))
	{
		await e.SendMessageAsync(GetString("SampleReplyMessage"));
	}
}


/// <summary>
/// Defines a local common resource fetcher.
/// </summary>
file static class LocalResourceFetcher
{
	/// <summary>
	/// Fetches the resource via the key. This method simply calls <see cref="ResourceManager.GetString(string)"/>.
	/// </summary>
	/// <param name="key">The resource key.</param>
	/// <returns>The string value.</returns>
	/// <seealso cref="ResourceManager"/>
	public static string? GetString(string key) => Resources.ResourceManager.GetString(key);
}
