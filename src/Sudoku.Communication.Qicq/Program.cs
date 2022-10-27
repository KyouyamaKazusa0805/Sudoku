using static LocalResourceFetcher;

// Creates and initializes a bot.
using var bot = new MiraiBot { Address = X("HostPort"), QQ = X("BotQQ"), VerifyKey = X("VerifyKey") };
await bot.LaunchAsync();

// Registers some necessary events.
bot.MessageReceived.OfType<GroupMessageReceiver>().Subscribe(onGroupMessageReceiving);
bot.EventReceived.OfType<MemberJoinedEvent>().Subscribe(onMemberJoined);

// Blocks the main thread, in order to prevent the main thread exits too fast.
var signal = new ManualResetEvent(false);
signal.WaitOne();


static async void onMemberJoined(MemberJoinedEvent e)
{
	if (e.Member.Group is { Id: var id } group && isMyGroupId(id))
	{
		await group.SendGroupMessageAsync(X("SampleMemberJoinedMessage"));
	}
}

static async void onGroupMessageReceiving(GroupMessageReceiver e)
{
	if (e is not { Sender: { Id: var senderId, Permission: var permission } sender, GroupId: var groupId, MessageChain: var message })
	{
		return;
	}

	if (!isMyGroupId(groupId))
	{
		return;
	}

	var random = new Random();
	var plainMessage = message.GetPlainMessage()?.Trim();
	switch (plainMessage)
	{
		case ['!' or '\uff01', .. var slice]: // User commands.
		{
			//
			// Check-in
			//
			if (isCommand(slice, "_Command_CheckIn"))
			{
				var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
				if (!Directory.Exists(folder))
				{
					// Error. The computer does not contain "My Documents" folder.
					// This folder is special; if the computer does not contain the folder, we should return directly.
					return;
				}

				var botDataFolder = $"""{folder}\{X("BotSettingsFolderName")}""";
				if (!Directory.Exists(botDataFolder))
				{
					Directory.CreateDirectory(botDataFolder);
				}

				var botUsersDataFolder = $"""{botDataFolder}\{X("UserSettingsFolderName")}""";
				if (!Directory.Exists(botUsersDataFolder))
				{
					Directory.CreateDirectory(botUsersDataFolder);
				}

				var userDataPath = $"""{botUsersDataFolder}\{senderId}.json""";
				var userData = File.Exists(userDataPath)
					? JsonSerializer.Deserialize<UserData>(await File.ReadAllTextAsync(userDataPath))!
					: new() { QQ = senderId };

				if (userData.LastCheckIn == DateTime.Today)
				{
					// Disallow user checking in multiple times in a same day.
					await e.SendMessageAsync(X("_MessageFormat_CheckInFailedDueToMultipleInSameDay")!);
					return;
				}
				
				if ((DateTime.Today - userData.LastCheckIn).Days == 1)
				{
					// Continuous.
					userData.ComboCheckedIn++;

					var expEarned = generateCheckInExpContinuous(random, userData.ComboCheckedIn);
					userData.Score += expEarned;
					userData.LastCheckIn = DateTime.Today;

					await e.SendMessageAsync(string.Format(X("_MessageFormat_CheckInSuccessfulAndContinuous")!, userData.ComboCheckedIn, expEarned));
				}
				else
				{
					// Normal case.
					userData.ComboCheckedIn = 1;

					var expEarned = generateCheckInExp(random);
					userData.Score += expEarned;
					userData.LastCheckIn = DateTime.Today;

					await e.SendMessageAsync(string.Format(X("_MessageFormat_CheckInSuccessful")!, expEarned));
				}

				var json = JsonSerializer.Serialize(userData);
				await File.WriteAllTextAsync(userDataPath, json);

				return;
			}

			break;
		}
		case ['%' or '\uff05', ..] when isMe(sender) || permission is Permissions.Owner or Permissions.Administrator: // Manager commands.
		{
			break;
		}
		case [':' or '\uff1a', ..] when isMe(sender): // Admin commands.
		{
			break;
		}
		default: // Other unrecognized commands, or higher-permission commands visiting.
		{
			return;
		}
	}
}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
static bool isMyGroupId(string s) => s == X("SudokuGroupQQ");

[MethodImpl(MethodImplOptions.AggressiveInlining)]
static bool isMe(Member member) => member.Id == X("AdminQQ");

[MethodImpl(MethodImplOptions.AggressiveInlining)]
static bool isCommand([NotNullWhen(true)] string? slice, string commandKey) => slice == X(commandKey);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
static int generateCheckInExp(Random random)
	=> random.Next(0, 1000) switch
	{
		< 400 => 1, // 40%
		>= 400 and < 700 => 2, // 30%
		>= 700 and < 900 => 3, // 20%
		_ => 4 // 10%
	};

[MethodImpl(MethodImplOptions.AggressiveInlining)]
static int generateCheckInExpContinuous(Random random, int continuousDaysCount)
{
	var earned = generateCheckInExp(random);
	var level = continuousDaysCount / 7;
	return (int)Round(earned * (level * .2 + 1));
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
	public static string? X(string key) => Resources.ResourceManager.GetString(key);
}
