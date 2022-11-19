namespace Mirai.Net.Data.Messages.Receivers;

/// <summary>
/// Provides with extension methods on <see cref="GroupMessageReceiver"/>.
/// </summary>
/// <seealso cref="GroupMessageReceiver"/>
public static class GroupMessageReceiverExtensions
{
	/// <summary>
	/// Sends the picture message, and then delete the local file.
	/// </summary>
	/// <param name="this">The group message receiver.</param>
	/// <param name="painterCreator">
	/// <inheritdoc cref="InternalReadWrite.GenerateCachedPicturePath(Func{ISudokuPainter}?)" path="/param[@name='painterCreator']"/>
	/// </param>
	/// <returns>A task that handles the operation.</returns>
	[SupportedOSPlatform("windows")]
	public static async Task SendPictureThenDeleteAsync(this GroupMessageReceiver @this, Func<ISudokuPainter>? painterCreator = null)
	{
		var picturePath = InternalReadWrite.GenerateCachedPicturePath(painterCreator)!;

		await @this.SendMessageAsync(new ImageMessage { Path = picturePath });

		File.Delete(picturePath);
	}
}
