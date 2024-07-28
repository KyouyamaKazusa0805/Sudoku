namespace Sudoku.Bot;

/// <summary>
/// <see cref="ChatMessageApi"/> 的扩展方法。
/// </summary>
public static class ChatMessageApiExtensions
{
	/// <summary>
	/// 往群里发送指定的消息。
	/// </summary>
	public static async Task<ChatMessageResp> SendGroupMessageAsync(this ChatMessageApi @this, ChatMessage message, string str)
		=> await @this.SendGroupMessageAsync(message.GroupOpenId, str, passiveMsgId: message.Id);
}
