#pragma warning disable IDE0060

#nullable enable

namespace Sudoku.Bot.Oicq.Exporting;

/// <summary>
/// Provides a type that can converts the values into the Onebot standard object types.
/// </summary>
public static class MessageEvents
{
	/// <summary>
	/// Triggers the C2C event.
	/// </summary>
	public static int Event_PrivateMessage(
		long time, long selfId, string subtype, int msgId, long userId,
		string message, int font, object? senderInfo)
		=> Event_PrivateMessage(new AmiableMessageEventArgs
		{
			EventType = EventType.MESSAGE,
			MessageType = MessageEventType.PRIVATE,
			Timestamp = time,
			Bot = selfId,
			RawMessage = message,
			Font = font,
			MessageId = msgId,
			UserId = userId,
			Sender = senderInfo,
		});

	/// <summary>
	/// Triggers the C2C event.
	/// </summary>
	public static int Event_PrivateMessage(AmiableMessageEventArgs eventArgs)
	{
		var amiableEventType = AmiableEventType.Private;

		EventCore.InvokeEvents(amiableEventType, eventArgs);
		return (int)eventArgs.HandleResult;
	}

	/// <summary>
	/// Triggers the group event.
	/// </summary>
	public static int Event_GroupMessage(
		long time, long selfId, string subtype, int msg_id, long group_id, long userId,
		string message, int font, object? senderInfo)
		=> Event_GroupMessage(new AmiableMessageEventArgs
		{
			EventType = EventType.MESSAGE,
			MessageType = MessageEventType.GROUP,
			Timestamp = time,
			Bot = selfId,
			RawMessage = message,
			Font = font,
			GroupId = group_id,
			MessageId = msg_id,
			UserId = userId,
			Sender = senderInfo,
		});

	/// <summary>
	/// Triggers the group event.
	/// </summary>
	public static int Event_GroupMessage(AmiableMessageEventArgs eventArgs)
	{
		var amiableEventType = AmiableEventType.Group;

		EventCore.InvokeEvents(amiableEventType, eventArgs);
		return (int)eventArgs.HandleResult;
	}
}
