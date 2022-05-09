#pragma warning disable CS1591

namespace Sudoku.Bot.Communication.Models.MessageTemplates;

public sealed class MsgText : MessageToCreate
{
	public MsgText(string? content = null, string? replyMsgId = null) => (Id, Content) = (replyMsgId, content);


	public MsgText SetReplyMsgId(string? msgId)
	{
		Id = msgId;

		return this;
	}

	public MsgText SetContent(string? content)
	{
		Content = content;
		
		return this;
	}
}
