namespace Sudoku.Bot.Oicq.Concepts;

/// <summary>
/// Provides with the event arguments for the messaging.
/// </summary>
public sealed class AmiableMessageEventArgs : AmiableEventArgs
{
	/// <summary>
	/// Indicates the sender that triggers the event.
	/// </summary>
	[JsonPropertyName("sender")]
	public object? Sender { get; set; }

	/// <summary>
	/// Indicates the message ID value.
	/// </summary>
	[JsonPropertyName("message_id")]
	public int MessageId { get; set; }

	/// <summary>
	/// Indicates the font of the message.
	/// </summary>
	[JsonPropertyName("font")]
	public int Font { get; set; }

	/// <summary>
	/// Indicates the QQ of the group.
	/// </summary>
	[JsonPropertyName("group_id")]
	public long GroupId { get; set; }

	/// <summary>
	/// Indicates the QQ of a user that triggers the event.
	/// </summary>
	[JsonPropertyName("user_id")]
	public long UserId { get; set; }

	/// <summary>
	/// Indicates the raw message.
	/// </summary>
	[JsonPropertyName("raw_message")]
	public string? RawMessage { get; set; }


	/// <summary>
	/// Indicates the message type.
	/// </summary>
	[JsonPropertyName("message_type")]
	[JsonConverter(typeof(EnumConverter<MessageEventType>))]
	public MessageEventType MessageType { get; set; }

	/// <summary>
	/// Indicates the sub type of the message.
	/// </summary>
	[JsonPropertyName("sub_type")]
	[JsonConverter(typeof(EnumConverter<MessageEventSubType>))]
	public MessageEventSubType SubType { get; set; }


	/// <summary>
	/// Send message to the source.
	/// </summary>
	/// <param name="contents">The contents of a message you want to send.</param>
	public void SendMessage(params object[] contents)
	{
		if (ApiWrapper is null)
		{
			return;
		}

		string content = string.Concat(contents);
		var sendGroupMessage = ApiWrapper.SendGroupMessage;
		var sendC2cMessage = ApiWrapper.SendC2cMessage;
		var (targetSendingMethod, qq) = MessageType switch
		{
			MessageEventType.Group => (sendGroupMessage, GroupId.ToString()),
			MessageEventType.C2C => (sendC2cMessage, UserId.ToString()),
			_ => default
		};

		targetSendingMethod?.Invoke(qq, content);
	}
}
