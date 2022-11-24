namespace Mirai.Net.Sessions;

/// <summary>
/// Provides with extenions methods on subscribing events for a <see cref="MiraiBot"/> instance.
/// </summary>
public static class MiraiBotSubscribingExtensions
{
	/// <summary>
	/// The object provided with methods to control <see cref="BotRunningContext.IsMuted"/> to be synchronized.
	/// </summary>
	private static readonly object MuteSyncRoot = new();


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
