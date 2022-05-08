namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 图片消息对象
/// </summary>
public class MsgDescImg : MessageToCreate
{
	/// <summary>
	/// 构建文字和图片同时存在的消息
	/// </summary>
	/// <param name="content">消息内容</param>
	/// <param name="image">图片URL</param>
	/// <param name="replyMsgId">要回复的消息id</param>
	public MsgDescImg(string? content = null, string? image = null, string? replyMsgId = null)
		=> (Id, Image, Content) = (replyMsgId, image, content);


	/// <summary>
	/// 设置要回复的目标消息
	/// </summary>
	/// <param name="msgId">目标消息的Id</param>
	/// <returns></returns>
	public MsgDescImg SetReplyMsgId(string? msgId) { Id = msgId; return this; }

	/// <summary>
	/// 设置消息内容
	/// </summary>
	/// <param name="content">文字内容</param>
	/// <returns></returns>
	public MsgDescImg SetContent(string? content) { Content = content; return this; }

	/// <summary>
	/// 设置图片URL
	/// </summary>
	/// <param name="image">图片URL</param>
	/// <returns></returns>
	public MsgDescImg SetImage(string? image) { Image = image; return this; }
}
