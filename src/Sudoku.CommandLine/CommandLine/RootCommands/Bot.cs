namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Represents a bot command.
/// </summary>
[RootCommand("bot", "To launch bot procedure.")]
[SupportedArguments("bot")]
[SupportedOSPlatform("windows")]
public sealed class Bot : IExecutable
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
		// Creates and initializes a bot.
		using var bot = new MiraiBot { Address = Address, QQ = BotNumber, VerifyKey = VerifyKey };

		try
		{
			await bot.LaunchAsync();

			// Registers some necessary events.
			bot.SubscribeGroupMessageReceived(onGroupMessageReceived);
			bot.SubscribeMemberJoined(onMemberJoined);
			bot.SubscribeNewMemberRequested(onNewMemberRequested);
			bot.SubscribeNewInvitationRequested(onNewInvitationRequested);

			// Blocks the main thread, in order to prevent the main thread exits too fast.
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


		async void onGroupMessageReceived(GroupMessageReceiver e)
		{
			switch (e)
			{
				// At message: special use. Gaming will rely on this case.
				case
				{
					Sender.Id: var sender,
					MessageChain: [SourceMessage, AtMessage { Target: var possibleBotId }, PlainMessage { Text: var plainMessage }]
				}
				when possibleBotId == BotNumber:
				{
					AnswerData.Add(
						new(
							sender,
							int.TryParse(plainMessage.Trim(), out var resultDigit) && resultDigit is >= 1 and <= 9 ? resultDigit - 1 : -1
						)
					);

					break;
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

		static async void onMemberJoined(MemberJoinedEvent e)
		{
			if (e.Member.Group is { Id: var groupId } group && groupId == R["SudokuGroupQQ"])
			{
				await group.SendGroupMessageAsync(R["SampleMemberJoinedMessage"]);
			}
		}

		static async void onNewMemberRequested(NewMemberRequestedEvent e)
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

		static async void onNewInvitationRequested(NewInvitationRequestedEvent e)
		{
			if (e is { GroupId: var groupId } && groupId == R["SudokuGroupQQ"])
			{
				await e.ApproveAsync();
			}
		}
	}
}
