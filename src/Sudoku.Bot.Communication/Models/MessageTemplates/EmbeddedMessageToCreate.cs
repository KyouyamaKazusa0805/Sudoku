#pragma warning disable CS1591

namespace Sudoku.Bot.Communication.Models.MessageTemplates;

/// <summary>
/// Defines a embedded message.
/// </summary>
/// <remarks>
/// For more information about embedded message, please visit
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/message/template/embed_message.html">this link</see>.
/// </remarks>
public sealed class EmbeddedMessageToCreate : MessageToCreate
{
	public EmbeddedMessageToCreate(
		string? title = null, string? prompt = null, string? thumbnail = null,
		List<MessageEmbedField>? embedFields = null, string? replyMsgId = null)
		=> (Id, EmbedFields, EmbedClass, Title, Prompt, Thumbnail, Embed) = (
			replyMsgId,
			embedFields ?? new(),
			new MessageEmbed { Thumbnail = EmbedThumbnail, Fields = EmbedFields },
			title,
			prompt,
			thumbnail,
			EmbedClass
		);


	public string? Title
	{
		get => EmbedClass.Title;

		set => EmbedClass.Title = value;
	}

	public string? Prompt
	{
		get => EmbedClass.Prompt;

		set => EmbedClass.Prompt = value;
	}

	public string? Thumbnail
	{
		get => EmbedThumbnail.Url;

		set => EmbedThumbnail.Url = value;
	}

	public List<MessageEmbedField> EmbedFields { get; set; }

	private MessageEmbedThumbnail EmbedThumbnail { get; set; } = new();

	private MessageEmbed EmbedClass { get; set; }


	public EmbeddedMessageToCreate WithRepliedMessageId(string? repliedMessageId)
	{
		Id = repliedMessageId;

		return this;
	}

	public EmbeddedMessageToCreate WithTitle(string? title)
	{
		Title = title;

		return this;
	}

	public EmbeddedMessageToCreate WithPrompt(string? prompt)
	{
		Prompt = prompt;

		return this;
	}

	public EmbeddedMessageToCreate WithThumbnail(string? thumbnail)
	{
		Thumbnail = thumbnail;

		return this;
	}

	public EmbeddedMessageToCreate AppendLine(string? content)
	{
		EmbedFields.Add(new() { Name = content });
		
		return this;
	}
}
