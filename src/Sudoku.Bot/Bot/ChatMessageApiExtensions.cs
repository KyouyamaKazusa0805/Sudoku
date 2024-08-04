namespace Sudoku.Bot;

/// <summary>
/// <see cref="ChatMessageApi"/> 的扩展方法。
/// </summary>
public static class ChatMessageApiExtensions
{
	/// <summary>
	/// 按回复形式，往群里发送指定的消息。
	/// </summary>
	/// <param name="this">当前对象。</param>
	/// <param name="message">当发送消息时，提供的环境（如群的 ID、发送人 ID 等）的数据。</param>
	/// <param name="str">需要发送的字符串。</param>
	public static async Task<ChatMessageResp> SendGroupMessageAsync(this ChatMessageApi @this, ChatMessage message, string str)
		=> await @this.SendGroupMessageAsync(message.GroupOpenId, str, passiveMsgId: message.Id);

	/// <summary>
	/// 按回复形式，往群里发送指定的图片。图片<b>必须</b>上传到图床之中，并将图床 URL 路径传入。
	/// </summary>
	/// <param name="this">当前对象。</param>
	/// <param name="message">当发送消息时，提供的环境（如群的 ID、发送人 ID 等）的数据。</param>
	/// <param name="imageUrl">需要发送的图片的 URL。</param>
	public static async Task<ChatMessageResp> SendGroupImageAsync(this ChatMessageApi @this, ChatMessage message, string imageUrl)
	{
		// 因为是回复消息，所以发送的图片会被处理为图文排版消息。
		// 图文消息需要两步操作：先按图片单独发出；在发出后获得相应结果后，
		// 将结果的 FileInfo 和图文消息实例化参数关联上，并按“文本 + 图片”的方式发出，才能达成。

		// 先发图。发图会获得相应。
		var response = await @this.SendGroupMediaAsync(message.GroupOpenId, resourceUrl: imageUrl);

		// 将相应取出，传入图文消息模板里，ChatMessageType 选取 Media（多媒体消息）。
		return await @this.SendGroupMessageAsync(
			message.GroupOpenId,
			string.Empty,
			ChatMessageType.Media,
			media: new() { FileInfo = response.FileInfo },
			passiveMsgId: message.Id
		);
	}
}
