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

	/// <summary>
	/// 往群里发送指定的图片。图片必须上传到图床之中，并将图床 URL 路径传入。
	/// </summary>
	public static async Task<ChatMessageResp> SendGroupImageAsync(this ChatMessageApi @this, ChatMessage message, string pictureUrl)
	{
		var response = await @this.SendGroupMediaAsync(message.GroupOpenId, resourceUrl: pictureUrl);
		return await @this.SendGroupMessageAsync(
			message.GroupOpenId,
			string.Empty,
			ChatMessageType.Media,
			media: new() { FileInfo = response.FileInfo },
			passiveMsgId: message.Id
		);
	}
}
