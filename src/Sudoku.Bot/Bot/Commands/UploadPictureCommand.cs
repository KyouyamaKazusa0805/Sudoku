namespace Sudoku.Bot.Commands;

/// <summary>
/// 上传本地图片的指令。
/// </summary>
public sealed class UploadPictureCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => "上传图片";


	/// <inheritdoc/>
	public override async Task GroupCallback(ChatMessageApi api, ChatMessage message)
	{
		var (pictureLink, _) = await CloudObjectStorage.UploadFileAsync($@"{Program.DesktopPath}\test_file.jpg");
		await api.SendGroupImageAsync(message, pictureLink);
	}
}
