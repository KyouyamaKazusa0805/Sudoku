// Add resource router.
R.AddExternalResourceFetecher(typeof(Program).Assembly, Resources.ResourceManager.GetString);

// Creates and initializes a bot.
using var bot = new MiraiBot { Address = R["HostPort"], QQ = R["BotQQ"]!, VerifyKey = R["VerifyKey"] };

try
{
	await bot.LaunchAsync();

	// Registers some necessary events.
	bot.MessageReceived.OfType<GroupMessageReceiver>().Subscribe(onGroupMessageReceiving);
	bot.EventReceived.OfType<MemberJoinedEvent>().Subscribe(onMemberJoined);
	bot.EventReceived.OfType<NewMemberRequestedEvent>().Subscribe(onNewMemberRequested);
	bot.EventReceived.OfType<NewInvitationRequestedEvent>().Subscribe(onInvitationRequested);

	// Blocks the main thread, in order to prevent the main thread exits too fast.
	Console.WriteLine(R["BootingSuccessMessage"]);
	Console.ReadKey();
}
catch (FlurlHttpException)
{
	Console.WriteLine(R["BootingFailedDueToMirai"]);
}
catch (InvalidResponseException)
{
	Console.WriteLine(R["BootingFailedDueToHttp"]);
}


async void onNewMemberRequested(NewMemberRequestedEvent e)
{
	if (e is not { GroupId: var groupId, Message: var message })
	{
		return;
	}

	if (!isMyGroupId(groupId))
	{
		return;
	}

	if (!await bot.CanHandleInvitationOrJoinRequestAsync(groupId))
	{
		return;
	}

	var bilibiliPattern = R["BilibiliNameRegexPattern"]!;
	if (!message.Trim().IsMatch(bilibiliPattern))
	{
		await e.RejectAsync(R["_MessageFormat_RejectJoiningGroup"]!);
		return;
	}

	await e.ApproveAsync();
}

async void onInvitationRequested(NewInvitationRequestedEvent e)
{
	if (e is not { GroupId: var groupId })
	{
		return;
	}

	if (!isMyGroupId(groupId))
	{
		return;
	}

	if (!await bot.CanHandleInvitationOrJoinRequestAsync(groupId))
	{
		return;
	}

	await e.ApproveAsync();
}

static async void onMemberJoined(MemberJoinedEvent e)
{
	if (e.Member.Group is { Id: var id } group && isMyGroupId(id))
	{
		await group.SendGroupMessageAsync(R["SampleMemberJoinedMessage"]);
	}
}

static async void onGroupMessageReceiving(GroupMessageReceiver e)
{
	if (e is not
		{
			Sender:
			{
				Id: var senderId,
				Name: var senderName,
				Permission: var permission,
				MmeberProfile.NickName: var senderOriginalName,
				Group: var group
			} sender,
			MessageChain: var message
		})
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
			// Help
			//
			if (isCommand(slice, "_Command_Help"))
			{
				await e.SendMessageAsync(R["_HelpMessage"]);
				return;
			}

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

				var botDataFolder = $"""{folder}\{R["BotSettingsFolderName"]}""";
				if (!Directory.Exists(botDataFolder))
				{
					Directory.CreateDirectory(botDataFolder);
				}

				var botUsersDataFolder = $"""{botDataFolder}\{R["UserSettingsFolderName"]}""";
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
					await e.SendMessageAsync(R["_MessageFormat_CheckInFailedDueToMultipleInSameDay"]!);
					return;
				}

				if ((DateTime.Today - userData.LastCheckIn).Days == 1)
				{
					// Continuous.
					userData.ComboCheckedIn++;

					var expEarned = generateCheckInExpContinuous(random, userData.ComboCheckedIn);
					userData.Score += expEarned;
					userData.LastCheckIn = DateTime.Today;

					await e.SendMessageAsync(string.Format(R["_MessageFormat_CheckInSuccessfulAndContinuous"]!, userData.ComboCheckedIn, expEarned));
				}
				else
				{
					// Normal case.
					userData.ComboCheckedIn = 1;

					var expEarned = generateCheckInExp(random);
					userData.Score += expEarned;
					userData.LastCheckIn = DateTime.Today;

					await e.SendMessageAsync(string.Format(R["_MessageFormat_CheckInSuccessful"]!, expEarned));
				}

				var json = JsonSerializer.Serialize(userData);
				await File.WriteAllTextAsync(userDataPath, json);

				return;
			}

			//
			// Check-in manual
			//
			if (isCommand(slice, "_Command_CheckInIntro"))
			{
				await e.SendMessageAsync(R["_MessageFormat_CheckInIntro"]);
				return;
			}

			//
			// Lookup score
			//
			if (isCommand(slice, "_Command_LookupScore"))
			{
				var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
				if (!Directory.Exists(folder))
				{
					// Error. The computer does not contain "My Documents" folder.
					// This folder is special; if the computer does not contain the folder, we should return directly.
					goto DirectlyReturn;
				}

				var botDataFolder = $"""{folder}\{R["BotSettingsFolderName"]}""";
				if (!Directory.Exists(botDataFolder))
				{
					goto SpecialCase_UserDataFileNotFound;
				}

				var botUsersDataFolder = $"""{botDataFolder}\{R["UserSettingsFolderName"]}""";
				if (!Directory.Exists(botUsersDataFolder))
				{
					goto SpecialCase_UserDataFileNotFound;
				}

				var userDataPath = $"""{botUsersDataFolder}\{senderId}.json""";
				if (!File.Exists(userDataPath))
				{
					goto SpecialCase_UserDataFileNotFound;
				}

				var userData = JsonSerializer.Deserialize<UserData>(await File.ReadAllTextAsync(userDataPath))!;
				await e.SendMessageAsync(string.Format(R["_MessageFormat_UserScoreIs"]!, senderName, userData.Score, senderOriginalName));

				goto DirectlyReturn;

			SpecialCase_UserDataFileNotFound:
				await e.SendMessageAsync(string.Format(R["_MessageFormat_UserScoreNotFound"]!, senderName, senderOriginalName));

			DirectlyReturn:
				return;
			}

			break;
		}
		case ['%' or '\uff05', .. var slice] when isMe(sender) || permission is Permissions.Owner or Permissions.Administrator: // Manager commands.
		{
			//
			// Lookup score
			//
			if (isComplexCommand(slice, "_Command_ComplexLookupScore", out var lookupArguments))
			{
				if (lookupArguments is not [var nameOrId])
				{
					return;
				}

				var satisfiedMembers = (
					from member in await @group.GetGroupMembersAsync()
					where member.Id == nameOrId || member.Name == nameOrId
					select member
				).ToArray();
				switch (satisfiedMembers)
				{
					case []:
					{
						await e.SendMessageAsync(R["_MessageFormat_LookupNameOrIdInvalid"]);
						break;
					}
					case { Length: >= 2 }:
					{
						await e.SendMessageAsync(R["_MessageFormat_LookupNameOrIdAmbiguous"]);
						break;
					}
					case [{ Id: var foundMemberId }]:
					{
						var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
						if (!Directory.Exists(folder))
						{
							// Error. The computer does not contain "My Documents" folder.
							// This folder is special; if the computer does not contain the folder, we should return directly.
							goto DirectlyReturn;
						}

						var botDataFolder = $"""{folder}\{R["BotSettingsFolderName"]}""";
						if (!Directory.Exists(botDataFolder))
						{
							goto SpecialCase_UserDataFileNotFound;
						}

						var botUsersDataFolder = $"""{botDataFolder}\{R["UserSettingsFolderName"]}""";
						if (!Directory.Exists(botUsersDataFolder))
						{
							goto SpecialCase_UserDataFileNotFound;
						}

						var userDataPath = $"""{botUsersDataFolder}\{foundMemberId}.json""";
						if (!File.Exists(userDataPath))
						{
							goto SpecialCase_UserDataFileNotFound;
						}

						var userData = JsonSerializer.Deserialize<UserData>(await File.ReadAllTextAsync(userDataPath))!;
						await e.SendMessageAsync(string.Format(R["_MessageFormat_UserScoreIs"]!, senderName, userData.Score, senderOriginalName));

						goto DirectlyReturn;

					SpecialCase_UserDataFileNotFound:
						await e.SendMessageAsync(string.Format(R["_MessageFormat_UserScoreNotFound"]!, senderName, senderOriginalName));

					DirectlyReturn:
						return;
					}
				}

				return;
			}

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
static bool isMyGroupId(string s) => s == R["SudokuGroupQQ"];

[MethodImpl(MethodImplOptions.AggressiveInlining)]
static bool isMe(Member member) => member.Id == R["AdminQQ"];

[MethodImpl(MethodImplOptions.AggressiveInlining)]
static bool isCommand([NotNullWhen(true)] string? slice, string commandKey) => slice == R[commandKey];

[MethodImpl(MethodImplOptions.AggressiveInlining)]
static bool isComplexCommand([NotNullWhen(true)] string? slice, string commandKey, [NotNullWhen(true)] out string[]? arguments)
{
	if (slice is null)
	{
		goto InvalidReturn;
	}

	if (!slice.Contains(',') && !slice.Contains('\uff0c'))
	{
		goto InvalidReturn;
	}

	var baseArguments = slice.Split(new[] { ',', '\uff0c' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
	if (baseArguments.Any(static a => a is []))
	{
		goto InvalidReturn;
	}

	if (baseArguments is not [var commandName, .. var otherArguments])
	{
		goto InvalidReturn;
	}

	if (R[commandKey] != commandName)
	{
		goto InvalidReturn;
	}

	arguments = otherArguments;
	return true;

InvalidReturn:
	arguments = null;
	return false;
}

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
