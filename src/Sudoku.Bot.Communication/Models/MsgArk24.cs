#pragma warning disable CS1591

namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the templated message whose ID is 24, which contains a paragraph of plain text, a link and a thumbnail.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/gosdk/api/message/message_template.html#%E6%A0%B7%E5%BC%8F-id-24">this link</see>.
/// </remarks>
public sealed class MsgArk24 : MessageToCreate
{
	private readonly MessageArkKeyValuePair _arkDescription = new() { Key = "#DESC#", Value = null };

	private readonly MessageArkKeyValuePair _arkPrompt = new() { Key = "#PROMPT#", Value = null };

	private readonly MessageArkKeyValuePair _arkTitle = new() { Key = "#TITLE#", Value = null };

	private readonly MessageArkKeyValuePair _arkMetaDescription = new() { Key = "#METADESC#", Value = null };

	private readonly MessageArkKeyValuePair _arkThumbnail = new() { Key = "#IMG#", Value = null };

	private readonly MessageArkKeyValuePair _arkLink = new() { Key = "#LINK#", Value = null };

	private readonly MessageArkKeyValuePair _arkSubtitle = new() { Key = "#SUBTITLE#", Value = null };


	public MsgArk24(
		string? description = null, string? prompt = null, string? title = null,
		string? metaDescription = null, string? image = null, string? link = null,
		string? subtitle = null, string? replyMessageId = null)
		=> (Id, Description, Prompt, Title, MetaDescription, Thumbnail, Link, Subtitle, Ark) = (
			replyMessageId,
			description,
			prompt,
			title,
			metaDescription,
			image,
			link,
			subtitle,
			new()
			{
				TemplateId = 24,
				Kv = new()
				{
					_arkDescription,
					_arkPrompt,
					_arkTitle,
					_arkMetaDescription,
					_arkThumbnail,
					_arkLink,
					_arkSubtitle
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

	public string? Title
	{
		get => _arkTitle.Value;

		set => _arkTitle.Value = value;
	}

	public string? MetaDescription
	{
		get => _arkMetaDescription.Value;

		set => _arkMetaDescription.Value = value;
	}

	public string? Thumbnail
	{
		get => _arkThumbnail.Value;

		set => _arkThumbnail.Value = value;
	}

	public string? Link
	{
		get => _arkLink.Value;

		set => _arkLink.Value = value;
	}

	public string? Subtitle
	{
		get => _arkSubtitle.Value;

		set => _arkSubtitle.Value = value;
	}


	public MsgArk24 WithReplyMessageId(string? msgId)
	{
		Id = msgId;

		return this;
	}

	public MsgArk24 WithDescription(string? desc)
	{
		Description = desc;

		return this;
	}

	public MsgArk24 WithPrompt(string? prompt)
	{
		Prompt = prompt;

		return this;
	}

	public MsgArk24 WithTitle(string? title)
	{
		Title = title;

		return this;
	}

	public MsgArk24 WithMetaDesc(string? metaDescription)
	{
		MetaDescription = metaDescription;

		return this;
	}

	public MsgArk24 WithImage(string? imgLink)
	{
		Thumbnail = imgLink;

		return this;
	}

	public MsgArk24 WithLink(string? link)
	{
		Link = link;

		return this;
	}

	public MsgArk24 WithSubTitle(string? subTitle)
	{
		Subtitle = subTitle;
		
		return this;
	}
}
