namespace Mirai.Net.Sessions;

/// <summary>
/// Provides with extenions methods on subscribing events for a <see cref="MiraiBot"/> instance.
/// </summary>
internal static class MiraiBotSubscribingExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SubscribeGroupMessageReceived(this MiraiBot @this, Action<GroupMessageReceiver> action)
		=> @this.MessageReceived.OfType<GroupMessageReceiver>().Subscribe(action);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SubscribeMemberJoined(this MiraiBot @this, Action<MemberJoinedEvent> action)
		=> @this.EventReceived.OfType<MemberJoinedEvent>().Subscribe(action);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SubscribeNewMemberRequested(this MiraiBot @this, Action<NewMemberRequestedEvent> action)
		=> @this.EventReceived.OfType<NewMemberRequestedEvent>().Subscribe(action);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SubscribeNewInvitationRequested(this MiraiBot @this, Action<NewInvitationRequestedEvent> action)
		 => @this.EventReceived.OfType<NewInvitationRequestedEvent>().Subscribe(action);
}
