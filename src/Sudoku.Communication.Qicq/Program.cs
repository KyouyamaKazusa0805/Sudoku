// Add resource router.
var currentAssembly = typeof(Command).Assembly;
R.AddExternalResourceFetecher(currentAssembly, Resources.ResourceManager.GetString);

// Creates and initializes a bot.
using var bot = new MiraiBot { Address = R["HostPort"], QQ = R["BotQQ"]!, VerifyKey = R["VerifyKey"] };

try
{
	await bot.LaunchAsync();

	// Registers some necessary events.
	bot.MessageReceived.OfType<GroupMessageReceiver>().Subscribe(
		async e =>
		{
			if (e is { Sender.Permission: var permission, MessageChain: (_, { } messageTrimmed, _) })
			{
				foreach (var type in currentAssembly.GetDerivedTypes<Command>())
				{
					if (type.GetConstructor(Array.Empty<Type>()) is null)
					{
						continue;
					}

					if (type.GetCustomAttribute<CommandAttribute>() is not { AllowedPermissions: var allowPermissions })
					{
						continue;
					}

					if (Array.IndexOf(allowPermissions, permission) == -1)
					{
						await e.SendMessageAsync(R["_MessageFormat_PermissionRequired"]!);
						continue;
					}

					if (await ((Command)Activator.CreateInstance(type)!).ExecuteAsync(messageTrimmed, e))
					{
						return;
					}
				}
			}
		}
	);
	bot.EventReceived.OfType<MemberJoinedEvent>().Subscribe(
		static async e =>
		{
			if (e.Member.Group is { Id: var groupId } group && groupId == R["SudokuGroupQQ"])
			{
				await group.SendGroupMessageAsync(R["SampleMemberJoinedMessage"]);
			}
		}
	);
	bot.EventReceived.OfType<NewMemberRequestedEvent>().Subscribe(
		async e =>
		{
			if (e is { GroupId: var groupId, Message: var message } && groupId == R["SudokuGroupQQ"]
				&& await bot.CanHandleInvitationOrJoinRequestAsync(groupId))
			{
				await (
					message.Trim().IsMatch("""((\u54d4\u54e9)\2|[Bb]\s{0,3}\u7ad9|[Bb]i(li)bi\3)""")
						? e.ApproveAsync()
						: e.RejectAsync(R["_MessageFormat_RejectJoiningGroup"]!)
				);
			}
		}
	);
	bot.EventReceived.OfType<NewInvitationRequestedEvent>().Subscribe(
		async e =>
		{
			if (e is { GroupId: var groupId } && groupId == R["SudokuGroupQQ"] && await bot.CanHandleInvitationOrJoinRequestAsync(groupId))
			{
				await e.ApproveAsync();
			}
		}
	);

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
