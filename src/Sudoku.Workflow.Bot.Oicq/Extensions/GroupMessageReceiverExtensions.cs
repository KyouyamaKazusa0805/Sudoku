namespace Mirai.Net.Data.Messages.Receivers;

/// <summary>
/// 为类型 <see cref="GroupMessageReceiver"/> 实例提供扩展方法。
/// </summary>
/// <seealso cref="GroupMessageReceiver"/>
public static class GroupMessageReceiverExtensions
{
	/// <summary>
	/// 根据指定的回调函数生成 <see cref="ISudokuPainter"/> 对象，将该对象产生的图片保存至本地，然后将其发送出去，然后自动删掉本地的图片缓存。
	/// </summary>
	/// <param name="this">用来发送消息的对象。</param>
	/// <param name="painterCreator">
	/// <inheritdoc cref="StorageHandler.GenerateCachedPicturePath(Func{ISudokuPainter}?)" path="/param[@name='painterCreator']"/>
	/// </param>
	/// <returns>一个 <see cref="Task"/> 对象，包裹了异步执行的基本信息。</returns>
	[SupportedOSPlatform(OperatingSystemNames.Windows)]
	public static async Task SendPictureThenDeleteAsync(this GroupMessageReceiver @this, Func<ISudokuPainter>? painterCreator = null)
	{
		painterCreator ??= () => RunningContexts[@this.GroupId].DrawingContext.Painter!;
		var picturePath = StorageHandler.GenerateCachedPicturePath(painterCreator)!;

		await @this.SendMessageAsync(new ImageMessage { Path = picturePath });

		File.Delete(picturePath);
	}
}
