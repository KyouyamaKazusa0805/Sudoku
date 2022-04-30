#pragma warning disable IDE0060

#nullable enable

namespace Sudoku.Bot.Oicq.Exporting;

/// <summary>
/// Provides a triggerer that can converts the values into the Onebot standard object types.
/// </summary>
public static class EventTriggerer
{
	/// <summary>
	/// Triggers C2C event.
	/// </summary>
	public static int TriggerC2CEvent(
		long time, long selfId, string subtype, int msgId, long userId, string message, int font, object? senderInfo)
		=> TriggerC2CEvent(
			new AmiableMessageEventArgs
			{
				EventType = EventType.Message,
				MessageType = MessageEventType.C2C,
				Timestamp = time,
				Bot = selfId,
				RawMessage = message,
				Font = font,
				MessageId = msgId,
				UserId = userId,
				Sender = senderInfo,
			}
		);

	/// <summary>
	/// Triggers C2C event.
	/// </summary>
	public static int TriggerC2CEvent(AmiableMessageEventArgs eventArgs)
	{
		var amiableEventType = AmiableEventType.C2C;

		BackingEventHandler.InvokeEvent(amiableEventType, eventArgs);
		return (int)eventArgs.HandleResult;
	}

	/// <summary>
	/// Triggers group event.
	/// </summary>
	public static int TriggerGroupEvent(
		long time, long selfId, string subtype, int msg_id, long group_id, long userId,
		string message, int font, object? senderInfo)
		=> TriggerGroupEvent(
			new AmiableMessageEventArgs
			{
				EventType = EventType.Message,
				MessageType = MessageEventType.Group,
				Timestamp = time,
				Bot = selfId,
				RawMessage = message,
				Font = font,
				GroupId = group_id,
				MessageId = msg_id,
				UserId = userId,
				Sender = senderInfo,
			}
		);

	/// <summary>
	/// Triggers group event.
	/// </summary>
	public static int TriggerGroupEvent(AmiableMessageEventArgs eventArgs)
	{
		var amiableEventType = AmiableEventType.Group;

		BackingEventHandler.InvokeEvent(amiableEventType, eventArgs);
		return (int)eventArgs.HandleResult;
	}
}
