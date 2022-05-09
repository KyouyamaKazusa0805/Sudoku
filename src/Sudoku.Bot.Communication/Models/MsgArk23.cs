namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the templated message whose ID is 23, which contains plain text with links.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/gosdk/api/message/message_template.html#%E6%A0%B7%E5%BC%8F-id-23">this link</see>.
/// </remarks>
public sealed class MsgArk23 : MessageToCreate
{
	/// <summary>
	/// Indciates the description of the ARK message.
	/// </summary>
	private readonly MessageArkKeyValuePair _arkDescription = new() { Key = "#DESC#", Value = null };

	/// <summary>
	/// Indicates the prompt of the ARK message.
	/// </summary>
	private readonly MessageArkKeyValuePair _arkPrompt = new() { Key = "#PROMPT#", Value = null };


	/// <summary>
	/// Initializes a <see cref="MsgArk23"/> instance via the specified info.
	/// </summary>
	/// <param name="desc">The description.</param>
	/// <param name="prompt">The prompt.</param>
	/// <param name="msgLines">The lines of the message.</param>
	/// <param name="replyMsgId">The message ID that the message is replied to.</param>
	public MsgArk23(
		string? desc = null, string? prompt = null, List<MessageArkObj>? msgLines = null, string? replyMsgId = null)
		=> (Id, Description, Prompt, MessageLines, Ark) = (
			replyMsgId, desc, prompt, msgLines ?? new(),
			new()
			{
				TemplateId = 23,
				Kv = new() { _arkDescription, _arkPrompt, new() { Key = "#LIST#", Obj = MessageLines } }
			}
		);


	/// <summary>
	/// Indicates the description of the message.
	/// </summary>
	public string? Description
	{
		get => _arkDescription.Value;

		set => _arkDescription.Value = value;
	}

	/// <summary>
	/// Indicates the propmt value of the message.
	/// </summary>
	public string? Prompt
	{
		get => _arkPrompt.Value;

		set => _arkPrompt.Value = value;
	}

	/// <summary>
	/// Indicates the lines of the message.
	/// </summary>
	public List<MessageArkObj> MessageLines { get; set; }


	/// <summary>
	/// Sets the reply message ID.
	/// </summary>
	/// <param name="msgId">The ID to be replied.</param>
	/// <returns>The current instance.</returns>
	public MsgArk23 WithReplyMessageId(string? msgId)
	{
		Id = msgId;

		return this;
	}

	/// <summary>
	/// Sets the description of the message.
	/// </summary>
	/// <param name="desc">The description of the message.</param>
	/// <returns>The current instance.</returns>
	public MsgArk23 WithDescription(string? desc)
	{
		Description = desc;

		return this;
	}

	/// <summary>
	/// Sets the prompt of the message.
	/// </summary>
	/// <param name="prompt">The prompt of the message.</param>
	/// <returns>The current instance.</returns>
	public MsgArk23 WithPrompt(string? prompt)
	{
		Prompt = prompt;
		
		return this;
	}

	/// <summary>
	/// Add a new line.
	/// </summary>
	/// <param name="content">The plain text to be displayed, as the introduction of the link.</param>
	/// <param name="link">The link. The URL link is available after passed the audit.</param>
	/// <returns>The current instance.</returns>
	public MsgArk23 AddLine(string? content, string? link = null)
	{
		var ojbk = new List<MessageArkObjKeyValuePair> { new() { Key = "desc", Value = content } };
		if (link is not null)
		{
			ojbk.Add(new() { Key = "link", Value = link });
		}

		MessageLines.Add(new() { ObjKv = ojbk });
		return this;
	}
}
