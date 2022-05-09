#pragma warning disable CS1591

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
	private readonly MessageArkKeyValuePair _arkDescription = new() { Key = "#DESC#", Value = null };

	private readonly MessageArkKeyValuePair _arkPrompt = new() { Key = "#PROMPT#", Value = null };


	public MsgArk23(
		string? description = null, string? prompt = null,
		List<MessageArkObj>? messageLines = null, string? replyMessageId = null)
		=> (Id, Description, Prompt, MessageLines, Ark) = (
			replyMessageId,
			description,
			prompt,
			messageLines ?? new(),
			new()
			{
				TemplateId = 23,
				Kv = new() { _arkDescription, _arkPrompt, new() { Key = "#LIST#", Obj = MessageLines } }
			}
		);


	public string? Description
	{
		get => _arkDescription.Value;

		set => _arkDescription.Value = value;
	}

	public string? Prompt
	{
		get => _arkPrompt.Value;

		set => _arkPrompt.Value = value;
	}

	public List<MessageArkObj> MessageLines { get; set; }


	public MsgArk23 WithReplyMessageId(string? msgId)
	{
		Id = msgId;

		return this;
	}

	public MsgArk23 WithDescription(string? desc)
	{
		Description = desc;

		return this;
	}

	public MsgArk23 WithPrompt(string? prompt)
	{
		Prompt = prompt;
		
		return this;
	}

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
