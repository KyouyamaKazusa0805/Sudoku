namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 图片消息
/// </summary>
public class MsgImage : MessageToCreate
{
	/// <summary>
	/// 构建图片消息
	/// </summary>
	/// <param name="image">图片URL</param>
	/// <param name="replyMsgId">要回复的消息id</param>
	public MsgImage(string? image = null, string? replyMsgId = null) => (Id, Image) = (replyMsgId, image);


	/// <summary>
	/// 设置要回复的目标消息
	/// </summary>
	/// <param name="msgId">目标消息的Id</param>
	/// <returns></returns>
	public MsgImage SetReplyMsgId(string? msgId) { Id = msgId; return this; }

	/// <summary>
	/// 设置图片网址
	/// </summary>
	/// <param name="image">图片URL</param>
	/// <returns></returns>
	public MsgImage SetImage(string? image) { Image = image; return this; }
}
