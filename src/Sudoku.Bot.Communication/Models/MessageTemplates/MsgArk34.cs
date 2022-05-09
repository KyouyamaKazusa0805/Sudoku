#pragma warning disable CS1591

namespace Sudoku.Bot.Communication.Models.MessageTemplates;

/// <summary>
/// Indicates the templated message whose ID is 34, which contains a line of plain text, a big-sized picture.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/gosdk/api/message/message_template.html#%E6%A0%B7%E5%BC%8F-id-34">this link</see>.
/// </remarks>
public class MsgArk34 : MessageToCreate
{
	private readonly MessageArkKeyValuePair _arkDescription = new() { Key = "#DESC#", Value = null };

	private readonly MessageArkKeyValuePair _arkPrompt = new() { Key = "#PROMPT#", Value = null };

	private readonly MessageArkKeyValuePair _arkMetaTitle = new() { Key = "#METATITLE#", Value = null };

	private readonly MessageArkKeyValuePair _arkMetaDescription = new() { Key = "#METADESC#", Value = null };

	private readonly MessageArkKeyValuePair _arkMetaIcon = new() { Key = "#METAICON#", Value = null };

	private readonly MessageArkKeyValuePair _arkMetaPreview = new() { Key = "#METAPREVIEW#", Value = null };

	private readonly MessageArkKeyValuePair _arkMetaUrl = new() { Key = "#METAURL#", Value = null };


	public MsgArk34(
		string? desc = null, string? prompt = null, string? metaTitle = null,
		string? metaDesc = null, string? metaIcon = null, string? metaPreview = null,
		string? metaUrl = null, string? replyMsgId = null)
		=> (Id, Description, Prompt, MetaTitle, MetaDescription, MetaIcon, MetaPreview, MetaUrl, Ark) = (
			replyMsgId,
			desc,
			prompt,
			metaTitle,
			metaDesc,
			metaIcon,
			metaPreview,
			metaUrl,
			new()
			{
				TemplateId = 34,
				Kv = new()
				{
					_arkDescription,
					_arkPrompt,
					_arkMetaTitle,
					_arkMetaDescription,
					_arkMetaIcon,
					_arkMetaPreview,
					_arkMetaUrl
				}
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

	public string? MetaTitle
	{
		get => _arkMetaTitle.Value;

		set => _arkMetaTitle.Value = value;
	}

	public string? MetaDescription
	{
		get => _arkMetaDescription.Value;

		set => _arkMetaDescription.Value = value;
	}

	public string? MetaIcon
	{
		get => _arkMetaIcon.Value;

		set => _arkMetaIcon.Value = value;
	}

	public string? MetaPreview
	{
		get => _arkMetaPreview.Value;

		set => _arkMetaPreview.Value = value;
	}

	public string? MetaUrl
	{
		get => _arkMetaUrl.Value;

		set => _arkMetaUrl.Value = value;
	}


	public MsgArk34 WithRepliedMessageId(string? repliedMessageId)
	{
		Id = repliedMessageId;

		return this;
	}

	public MsgArk34 WithDescription(string? description)
	{
		Description = description;

		return this;
	}

	public MsgArk34 WithPrompt(string? prompt)
	{
		Prompt = prompt;
		return this;
	}

	public MsgArk34 WithMetaTitle(string? metaTitle)
	{
		MetaTitle = metaTitle;

		return this;
	}

	public MsgArk34 WithMetaDescription(string? metaDescription)
	{
		MetaDescription = metaDescription;

		return this;
	}

	public MsgArk34 WithMetaIcon(string? iconLink)
	{
		MetaIcon = iconLink;

		return this;
	}

	public MsgArk34 WithMetaPreview(string? metaPreview)
	{
		MetaPreview = metaPreview;

		return this;
	}

	public MsgArk34 WithMetaUrl(string? metaUrl)
	{
		MetaUrl = metaUrl;
		
		return this;
	}
}
