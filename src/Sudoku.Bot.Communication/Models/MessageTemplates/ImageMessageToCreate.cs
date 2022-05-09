#pragma warning disable CS1591

namespace Sudoku.Bot.Communication.Models.MessageTemplates;

/// <summary>
/// Indicates the message that is a picture. The picture message can also take plain text.
/// </summary>
public sealed class ImageMessageToCreate : MessageToCreate
{
	public ImageMessageToCreate(string? content = null, string? image = null, string? replyMsgId = null)
		=> (Id, Image, Content) = (replyMsgId, image, content);


	public ImageMessageToCreate WithRepliedMessageId(string? repliedMessageId)
	{
		Id = repliedMessageId;

		return this;
	}

	public ImageMessageToCreate WithContent(string? content)
	{
		Content = content;

		return this;
	}

	public ImageMessageToCreate WithImage(string? image)
	{
		Image = image;
		
		return this;
	}
}
