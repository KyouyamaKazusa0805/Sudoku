#pragma warning disable CS1591

namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the templated message whose ID is 37, which contains a line of plain text, a subtitle
/// and a big-sized picture.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/gosdk/api/message/message_template.html#%E6%A0%B7%E5%BC%8F-id-37">this link</see>.
/// </remarks>
public sealed class MsgArk37 : MessageToCreate
{
	private readonly MessageArkKeyValuePair _arkPrompt = new() { Key = "#PROMPT#", Value = null };

	private readonly MessageArkKeyValuePair _arkMetaTitle = new() { Key = "#METATITLE#", Value = null };

	private readonly MessageArkKeyValuePair _arkMetaSubTitle = new() { Key = "#METASUBTITLE#", Value = null };

	private readonly MessageArkKeyValuePair _arkMetaCover = new() { Key = "#METACOVER#", Value = null };

	private readonly MessageArkKeyValuePair _arkMetaUrl = new() { Key = "#METAURL#", Value = null };


	public MsgArk37(
		string? prompt = null, string? metaTitle = null, string? metaSubTitle = null,
		string? metaCover = null, string? metaUrl = null, string? replyMsgId = null)
		=> (Id, Prompt, MetaTitle, MetaSubTitle, MetaCover, MetaUrl, Ark) = (
			replyMsgId,
			prompt,
			metaTitle,
			metaSubTitle,
			metaCover,
			metaUrl,
			new()
			{
				TemplateId = 37,
				Kv = new()
				{
					_arkPrompt,
					_arkMetaTitle,
					_arkMetaSubTitle,
					_arkMetaCover,
					_arkMetaUrl
				}
			}
		);


	public string? Prompt
	{
		get => _arkPrompt.Value;

		set => _arkPrompt.Value = value;
	}

	public string? MetaTitle
	{
		get => _arkMetaTitle.Value;

		set => _arkMetaTitle.Value = value;
	}

	public string? MetaCover
	{
		get => _arkMetaCover.Value;

		set => _arkMetaCover.Value = value;
	}

	public string? MetaUrl
	{
		get => _arkMetaUrl.Value;

		set => _arkMetaUrl.Value = value;
	}

	public string? MetaSubTitle
	{
		get => _arkMetaSubTitle.Value;

		set => _arkMetaSubTitle.Value = value;
	}


	public MsgArk37 WithRepliedMessageId(string? msgId)
	{
		Id = msgId;

		return this;
	}

	public MsgArk37 WithPrompt(string? prompt)
	{
		Prompt = prompt;

		return this;
	}

	public MsgArk37 WithMetaTitle(string? metaTitle)
	{
		MetaTitle = metaTitle;

		return this;
	}

	public MsgArk37 WithMetaSubtitle(string? subTitle)
	{
		MetaSubTitle = subTitle;

		return this;
	}

	public MsgArk37 WithMetaCover(string? cover)
	{
		MetaCover = cover;

		return this;
	}

	public MsgArk37 WithMetaUrl(string? metaUrl)
	{
		MetaUrl = metaUrl;

		return this;
	}
}
