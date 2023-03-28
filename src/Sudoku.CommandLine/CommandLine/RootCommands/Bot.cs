#define AUTO_SEND_MESSAGE_AFTER_MEMBER_JOINED
#define ALLOW_MEMBER_REQUEST
#undef ALLOW_INVITATION
#define BASIC_LOG_INFO_OUTPUT
#define ALLOW_PERIODIC_OPERATION
namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Represents a bot command.
/// </summary>
[RootCommand("bot", DescriptionResourceKey = "_Description_Bot")]
[SupportedArguments("bot")]
[Usage("bot -a <address> -q <number> -k <key>", IsPattern = true)]
[Usage("bot -a localhost:8080 -q 1357924680 -k HelloWorld", DescriptionResourceKey = "_Usage_Bot_1")]
[SupportedOSPlatform(OperatingSystemNames.Windows)]
file sealed class Bot : IExecutable
{
	/// <summary>
	/// The address.
	/// </summary>
	[DoubleArgumentsCommand('a', "address", DescriptionResourceKey = "_Description_Address_Bot")]
	public string Address { get; set; } = "localhost:8080";

	/// <summary>
	/// The number of the bot.
	/// </summary>
	[DoubleArgumentsCommand('q', "qq", DescriptionResourceKey = "_Description_BotNumber_Bot", IsRequired = true)]
	public string BotNumber { get; set; } = string.Empty;

	/// <summary>
	/// The verify key.
	/// </summary>
	[DoubleArgumentsCommand('k', "key", DescriptionResourceKey = "_Description_VerifyKey_Bot", IsRequired = true)]
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
			bot.SubscribeGroupMessage(CommandCollection.BuiltIn);

#if AUTO_SEND_MESSAGE_AFTER_MEMBER_JOINED
			bot.SubscribeMemberJoined(OnMemberJoinedAsync);
#endif
#if ALLOW_MEMBER_REQUEST
			bot.SubscribeNewMemberRequested(OnNewMemberRequestedAsync);
#endif
#if ALLOW_INVITATION
			bot.SubscribeNewInvitationRequested(OnNewInvitationRequestedAsync);
#endif
			bot.SubscribeMuted();
			bot.SubscribeUnmuted();

			(await AccountManager.GetGroupsAsync()).ForEach(static group => RunningContexts.TryAdd(group.Id, new()));

#if BASIC_LOG_INFO_OUTPUT
			await Terminal.WriteLineAsync(R["_Message_BootSuccess"]!, ConsoleColor.DarkGreen);
#endif
		}
		catch (Exception ex)
		{
#if BASIC_LOG_INFO_OUTPUT
			await Terminal.WriteLineAsync(
				ex switch
				{
					FlurlHttpException => R["_Message_BootFailed_Mirai"]!,
					InvalidResponseException => R["_Message_BootFailed_Connection"]!,
					_ => throw ex
				},
				ConsoleColor.DarkRed
			);
#endif
		}

#if ALLOW_PERIODIC_OPERATION
		_ = PeriodicCommandCollection.BuiltIn;
#endif

		Terminal.Pause();
	}

	/// <summary>
	/// Triggers when bot has already left the group.
	/// </summary>
	/// <param name="e">The event handler.</param>
	private void OnBotLeft(LeftEvent e) => RunningContexts.TryRemove(e.Group.Id, out _);

	/// <summary>
	/// Triggers when bot has already been kicked by group owner.
	/// </summary>
	/// <param name="e">The event handler.</param>
	private void OnBotKicked(KickedEvent e) => RunningContexts.TryRemove(e.Group.Id, out _);

	/// <summary>
	/// Triggers when bot has already joined in the target group.
	/// </summary>
	/// <param name="e">The event handler.</param>
	private void OnBotJoined(JoinedEvent e) => RunningContexts.TryAdd(e.Group.Id, new());

#if ALLOW_INVITATION
	/// <summary>
	/// Triggers when someone has been invited by another one to join in this group.
	/// </summary>
	/// <param name="e">The event handler.</param>
	private async void OnNewInvitationRequestedAsync(NewInvitationRequestedEvent e)
	{
		if (e is { GroupId: SudokuGroupNumber })
		{
			await e.ApproveAsync();
		}
	}
#endif

#if ALLOW_MEMBER_REQUEST
	/// <summary>
	/// Triggers when someone has requested that he wants to join in this group.
	/// </summary>
	/// <param name="e">The event handler.</param>
	private async void OnNewMemberRequestedAsync(NewMemberRequestedEvent e)
	{
		const string answerLocatorStr = "\u7b54\u6848\uff1a";

		if (e is { GroupId: SudokuGroupNumber, Message: var message }
			&& message.IndexOf(answerLocatorStr) is var answerLocatorStrIndex and not -1
			&& answerLocatorStrIndex + answerLocatorStr.Length is var finalIndex && finalIndex < message.Length
			&& message[finalIndex..] is var finalMessage)
		{
			if (Patterns.BilibiliPattern().IsMatch(finalMessage.Trim()))
			{
				await e.ApproveAsync();
			}
			else
			{
				await e.RejectAsync(R["_MessageFormat_MemberJoinedRejected"]!);
			}
		}
	}
#endif

#if AUTO_SEND_MESSAGE_AFTER_MEMBER_JOINED
	/// <summary>
	/// Triggers when someone has already joined in this group.
	/// </summary>
	/// <param name="e">The event handler.</param>
	private async void OnMemberJoinedAsync(MemberJoinedEvent e)
	{
		if (e.Member.Group is { Id: SudokuGroupNumber } group)
		{
			await group.SendGroupMessageAsync(R["_MessageFormat_SampleMemberJoined"]!);
		}
	}
#endif
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
[SupportedOSPlatform(OperatingSystemNames.Windows)]
file static class Extensions
{
	/// <summary>
	/// The object provided with methods to control <see cref="BotRunningContext.IsMuted"/> to be synchronized.
	/// </summary>
	private static readonly object MuteSyncRoot = new();


	/// <summary>
	/// Iterates all elements in this enumerable collection.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The collection.</param>
	/// <param name="visitor">The visitor method to handle and operate with each element.</param>
	public static void ForEach<T>(this IEnumerable<T> @this, Action<T> visitor)
	{
		switch (@this)
		{
			case T[] array:
			{
				Array.ForEach(array, visitor);
				break;
			}
			case List<T> list:
			{
				list.ForEach(visitor);
				break;
			}
			default:
			{
				foreach (var element in @this)
				{
					visitor(element);
				}

				break;
			}
		}
	}

#if false
	/// <summary>
	/// Subscribes for event <see cref="OnlineEvent"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SubscribeBotOnlined(this MiraiBot @this, Action<OnlineEvent> action)
		=> @this.EventReceived.OfType<OnlineEvent>().Subscribe(action);

	/// <summary>
	/// Subscribes for event <see cref="OfflineEvent"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SubscribeBotOfflined(this MiraiBot @this, Action<OfflineEvent> action)
		=> @this.EventReceived.OfType<OfflineEvent>().Subscribe(action);
#endif

	/// <summary>
	/// Subscribes for event <see cref="JoinedEvent"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SubscribeJoined(this MiraiBot @this, Action<JoinedEvent> action)
		=> @this.EventReceived.OfType<JoinedEvent>().Subscribe(action);

	/// <summary>
	/// Subscribes for event <see cref="LeftEvent"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SubscribeLeft(this MiraiBot @this, Action<LeftEvent> action)
		=> @this.EventReceived.OfType<LeftEvent>().Subscribe(action);

	/// <summary>
	/// Subscribes for event <see cref="KickedEvent"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SubscribeKicked(this MiraiBot @this, Action<KickedEvent> action)
		=> @this.EventReceived.OfType<KickedEvent>().Subscribe(action);

	/// <summary>
	/// Subscribes for modules.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SubscribeGroupMessage(this MiraiBot @this, CommandCollection modules)
		=> @this.MessageReceived.SubscribeGroupMessage(modules.Raise);

	/// <summary>
	/// Subscribes for event <see cref="GroupMessageReceiver"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SubscribeGroupMessageReceived(this MiraiBot @this, Action<GroupMessageReceiver> action)
		=> @this.MessageReceived.OfType<GroupMessageReceiver>().Subscribe(
			async gmr =>
			{
				switch (@this, gmr)
				{
					case ({ QQ: var botNumber }, { GroupId: var groupId }):
					{
						if (await GroupManager.GetMemberAsync(botNumber, groupId) is { MuteTimeRemaining: not "0" })
						{
							return;
						}

						lock (MuteSyncRoot)
						{
							if (RunningContexts[groupId].IsMuted)
							{
								return;
							}
						}

						action(gmr);

						break;
					}
				}
			}
		);

	/// <summary>
	/// Subscribes for event <see cref="MemberJoinedEvent"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SubscribeMemberJoined(this MiraiBot @this, Action<MemberJoinedEvent> action)
		=> @this.EventReceived.OfType<MemberJoinedEvent>().Subscribe(action);

	/// <summary>
	/// Subscribes for event <see cref="NewMemberRequestedEvent"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SubscribeNewMemberRequested(this MiraiBot @this, Action<NewMemberRequestedEvent> action)
		=> @this.EventReceived.OfType<NewMemberRequestedEvent>().Subscribe(action);

	/// <summary>
	/// Subscribes for event <see cref="NewInvitationRequestedEvent"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SubscribeNewInvitationRequested(this MiraiBot @this, Action<NewInvitationRequestedEvent> action)
		=> @this.EventReceived.OfType<NewInvitationRequestedEvent>().Subscribe(action);

	/// <summary>
	/// Subscribes for event <see cref="MutedEvent"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SubscribeMuted(this MiraiBot @this, Action<MutedEvent>? action = null)
		=> @this.EventReceived.OfType<MutedEvent>().Subscribe(
			me =>
			{
				lock (MuteSyncRoot)
				{
					RunningContexts[me.Operator.Group.Id].IsMuted = true;
				}

				action?.Invoke(me);
			}
		);

	/// <summary>
	/// Subscribes for event <see cref="UnmutedEvent"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SubscribeUnmuted(this MiraiBot @this, Action<UnmutedEvent>? action = null)
		=> @this.EventReceived.OfType<UnmutedEvent>().Subscribe(
			ue =>
			{
				lock (MuteSyncRoot)
				{
					RunningContexts[ue.Operator.Group.Id].IsMuted = false;
				}

				action?.Invoke(ue);
			}
		);
}
