#pragma warning disable CS1591

namespace Sudoku.Bot.Communication.Models.MessageTemplates;

public sealed class MsgImage : MessageToCreate
{
	public MsgImage(string? image = null, string? replyMsgId = null) => (Id, Image) = (replyMsgId, image);


	public MsgImage SetReplyMsgId(string? msgId)
	{
		Id = msgId;

		return this;
	}

	public MsgImage SetImage(string? image)
	{
		Image = image;
		
		return this;
	}
}
