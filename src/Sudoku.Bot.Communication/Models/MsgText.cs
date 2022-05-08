namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 文字消息
/// </summary>
public class MsgText : MessageToCreate
{
	/// <summary>
	/// 构建文字消息
	/// </summary>
	/// <param name="content">消息内容</param>
	/// <param name="replyMsgId">要回复的消息Id</param>
	public MsgText(string? content = null, string? replyMsgId = null) => (Id, Content) = (replyMsgId, content);


	/// <summary>
	/// 设置要回复的目标消息
	/// </summary>
	/// <param name="msgId">目标消息的Id</param>
	/// <returns></returns>
	public MsgText SetReplyMsgId(string? msgId) { Id = msgId; return this; }

	/// <summary>
	/// 设置消息内容
	/// </summary>
	/// <param name="content">文字内容</param>
	/// <returns></returns>
	public MsgText SetContent(string? content) { Content = content; return this; }
}
