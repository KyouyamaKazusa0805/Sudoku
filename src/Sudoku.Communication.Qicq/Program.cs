// Add resource router.
var currentAssembly = typeof(Command).Assembly;
R.AddExternalResourceFetecher(currentAssembly, Resources.ResourceManager.GetString);

// Creates and initializes a bot.
using var bot = new MiraiBot { Address = R["HostPort"], QQ = R["BotQQ"]!, VerifyKey = R["VerifyKey"] };

try
{
	await bot.LaunchAsync();

	// Registers some necessary events.
	bot.SubscribeGroupMessageReceived(onGroupMessageReceived);
	bot.SubscribeMemberJoined(onMemberJoined);
	bot.SubscribeNewMemberRequested(onNewMemberRequested);
	bot.SubscribeNewInvitationRequested(onNewInvitationRequested);

	// Blocks the main thread, in order to prevent the main thread exits too fast.
	Terminal.WriteLine(R["BootingSuccessMessage"]!, ConsoleColor.DarkGreen);
}
catch (FlurlHttpException)
{
	Terminal.WriteLine(R["BootingFailedDueToMirai"]!, ConsoleColor.DarkRed);
}
catch (InvalidResponseException)
{
	Terminal.WriteLine(R["BootingFailedDueToHttp"]!, ConsoleColor.DarkRed);
}

Terminal.Pause();


async void onGroupMessageReceived(GroupMessageReceiver e)
{
	if (e is { Sender.Permission: var permission, MessageChain: (_, { } messageTrimmed, _) })
	{
		foreach (var type in currentAssembly.GetDerivedTypes<Command>())
		{
			if (type.GetConstructor(Array.Empty<Type>()) is not null
				&& type.GetCustomAttribute<CommandAttribute>() is { AllowedPermissions: var allowPermissions }
				&& Array.IndexOf(allowPermissions, permission) != -1
				&& await ((Command)Activator.CreateInstance(type)!).ExecuteAsync(messageTrimmed, e))
			{
				return;
			}
		}
	}
}

static async void onMemberJoined(MemberJoinedEvent e)
{
	if (e.Member.Group is { Id: var groupId } group && groupId == R["SudokuGroupQQ"])
	{
		await group.SendGroupMessageAsync(R["SampleMemberJoinedMessage"]);
	}
}

async void onNewMemberRequested(NewMemberRequestedEvent e)
{
	const string answerLocatorStr = "\u7b54\u6848\uff1a";

	if (e is { GroupId: var groupId, Message: var message }
		&& message.IndexOf(answerLocatorStr) is var answerLocatorStrIndex and not -1
		&& answerLocatorStrIndex + answerLocatorStr.Length is var finalIndex && finalIndex < message.Length
		&& message[finalIndex..] is var finalMessage
		&& groupId == R["SudokuGroupQQ"])
	{
		await (BilibiliPattern().IsMatch(finalMessage.Trim()) ? e.ApproveAsync() : e.RejectAsync(R["_MessageFormat_RejectJoiningGroup"]!));
	}
}

async void onNewInvitationRequested(NewInvitationRequestedEvent e)
{
	if (e is { GroupId: var groupId } && groupId == R["SudokuGroupQQ"])
	{
		await e.ApproveAsync();
	}
}


/// <summary>
/// The program type.
/// </summary>
internal static partial class Program
{
	[GeneratedRegex("""((\u54d4\u54e9)\2|[Bb]\s{0,3}\u7ad9|[Bb]i(li)bi\3)""", RegexOptions.Compiled, 5000, "zh-CN")]
	private static partial Regex BilibiliPattern();
}
