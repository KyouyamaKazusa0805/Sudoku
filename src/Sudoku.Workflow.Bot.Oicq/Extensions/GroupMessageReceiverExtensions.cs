namespace Mirai.Net.Data.Messages.Receivers;

/// <summary>
/// 为类型 <see cref="GroupMessageReceiver"/> 实例提供扩展方法。
/// </summary>
/// <seealso cref="GroupMessageReceiver"/>
public static class GroupMessageReceiverExtensions
{
	/// <summary>
	/// 根据指定的 <see cref="ISudokuPainter"/> 对象，将该对象产生的图片保存至本地，然后将其发送出去，然后自动删掉本地的图片缓存。
	/// </summary>
	/// <param name="this">用来发送消息的对象。</param>
	/// <param name="painter">
	/// <inheritdoc cref="DrawingOperations.GenerateCachedPicturePath(ISudokuPainter)" path="/param[@name='painterCreator']"/>
	/// </param>
	/// <returns>一个 <see cref="Task"/> 对象，包裹了异步执行的基本信息。</returns>
	public static async Task SendPictureThenDeleteAsync(this GroupMessageReceiver @this, ISudokuPainter painter)
	{
		var picturePath = DrawingOperations.GenerateCachedPicturePath(painter)!;
		await @this.SendMessageAsync(new ImageMessage { Path = picturePath });

		File.Delete(picturePath);
	}
}
