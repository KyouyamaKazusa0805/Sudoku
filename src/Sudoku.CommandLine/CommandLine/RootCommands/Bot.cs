namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Represents a bot command.
/// </summary>
[RootCommand("bot", "To launch bot procedure.")]
[SupportedArguments("bot")]
[SupportedOSPlatform("windows")]
public sealed partial class Bot : IExecutable
{
	/// <summary>
	/// The address.
	/// </summary>
	[DoubleArgumentsCommand('a', "address", "Indicates the address of the bot to be connected to. Generally the value is 'localhost:8080'.")]
	public string Address { get; set; } = "localhost:8080";

	/// <summary>
	/// The number of the bot.
	/// </summary>
	[DoubleArgumentsCommand('q', "qq", "Indicates the real number of the bot.", IsRequired = true)]
	public string BotNumber { get; set; } = string.Empty;

	/// <summary>
	/// The verify key.
	/// </summary>
	[DoubleArgumentsCommand('k', "key", "Indicates the verify key used for making communication of web socket.", IsRequired = true)]
	public string VerifyKey { get; set; } = string.Empty;


	/// <inheritdoc/>
	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		using var bot = new MiraiBot { Address = Address, QQ = BotNumber, VerifyKey = VerifyKey };

		try
		{
			await bot.LaunchAsync();

			bot.SubscribeJoined(OnBotJoined);
			bot.SubscribeLeft(OnBotLeft);
			bot.SubscribeKicked(OnBotKicked);
			bot.SubscribeGroupMessageReceived(OnGroupMessageReceivedAsync);
			bot.SubscribeMemberJoined(OnMemberJoinedAsync);
			bot.SubscribeNewMemberRequested(OnNewMemberRequestedAsync);
			bot.SubscribeNewInvitationRequested(OnNewInvitationRequestedAsync);

			var groups = await AccountManager.GetGroupsAsync();
			foreach (var group in groups)
			{
				RunningContexts.TryAdd(group.Id, new());
			}

			await Terminal.WriteLineAsync(R["BootingSuccessMessage"]!, ConsoleColor.DarkGreen);
		}
		catch (FlurlHttpException)
		{
			await Terminal.WriteLineAsync(R["BootingFailedDueToMirai"]!, ConsoleColor.DarkRed);
		}
		catch (InvalidResponseException)
		{
			await Terminal.WriteLineAsync(R["BootingFailedDueToHttp"]!, ConsoleColor.DarkRed);
		}

		Terminal.Pause();
	}


	partial void OnBotLeft(LeftEvent e);
	partial void OnBotJoined(JoinedEvent e);
	partial void OnBotKicked(KickedEvent e);
	partial void OnGroupMessageReceivedAsync(GroupMessageReceiver e);
	partial void OnMemberJoinedAsync(MemberJoinedEvent e);
	partial void OnNewMemberRequestedAsync(NewMemberRequestedEvent e);
	partial void OnNewInvitationRequestedAsync(NewInvitationRequestedEvent e);
}

partial class Bot
{
	partial void OnBotLeft(LeftEvent e) => RunningContexts.TryRemove(e.Group.Id, out _);

	partial void OnBotJoined(JoinedEvent e) => RunningContexts.TryAdd(e.Group.Id, new());

	partial void OnBotKicked(KickedEvent e) => RunningContexts.TryRemove(e.Group.Id, out _);

	async partial void OnGroupMessageReceivedAsync(GroupMessageReceiver e)
	{
		switch (e)
		{
			// At message: special use. Gaming will rely on this case.
			// I think this way to handle messages is too complex and ugly. I may change the design on commands later.
			case { Sender: var sender, MessageChain: [SourceMessage, AtMessage { Target: var qq }, PlainMessage { Text: var message }] }
			when qq == BotNumber
				&& RunningContexts.TryGetValue(e.GroupId, out var context)
				&& context is { AnsweringContext.CurrentRoundAnsweredValues: { } answeredValues }
				&& message.Trim() is var trimmed:
			{
				if (int.TryParse(trimmed, out var validInteger))
				{
					tryParseDigits(validInteger, sender, answeredValues);
				}
				else if (trimmed.Reserve(@"\d") is var extraCharactersRemoved && int.TryParse(extraCharactersRemoved, out validInteger))
				{
					tryParseDigits(validInteger, sender, answeredValues);
				}

				break;


				static bool tryParseDigits(int validInteger, Member sender, ConcurrentBag<UserPuzzleAnswerData> answeredValues)
				{
					if (validInteger < 0)
					{
						return false;
					}

					// Split by digit bits.
					List<int> numberList;
					for (numberList = new(); validInteger != 0; validInteger /= 10)
					{
						numberList.Add(validInteger % 10 - 1);
					}

					// Reverse the collection.
					numberList.Reverse();

					answeredValues.Add(new(sender, numberList.ToArray()));
					return true;
				}
			}

			// Normal command message.
			case { Sender.Permission: var permission, MessageChain: (_, { } messageTrimmed, _) }:
			{
				foreach (var type in GetType().Assembly.GetDerivedTypes<Command>())
				{
					if (type.GetConstructor(Array.Empty<Type>()) is not null
						&& type.GetCustomAttribute<CommandAttribute>() is { AllowedPermissions: var allowPermissions }
						&& Array.IndexOf(allowPermissions, permission) != -1
						&& await ((Command)Activator.CreateInstance(type)!).ExecuteAsync(messageTrimmed, e))
					{
						return;
					}
				}

				break;
			}
		}
	}

	async partial void OnMemberJoinedAsync(MemberJoinedEvent e)
	{
		if (e.Member.Group is { Id: var groupId } group && groupId == R["SudokuGroupQQ"])
		{
			await group.SendGroupMessageAsync(R["SampleMemberJoinedMessage"]);
		}
	}

	async partial void OnNewMemberRequestedAsync(NewMemberRequestedEvent e)
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

	async partial void OnNewInvitationRequestedAsync(NewInvitationRequestedEvent e)
	{
		if (e is { GroupId: var groupId } && groupId == R["SudokuGroupQQ"])
		{
			await e.ApproveAsync();
		}
	}
}
