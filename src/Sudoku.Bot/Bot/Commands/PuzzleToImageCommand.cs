namespace Sudoku.Bot.Commands;

/// <summary>
/// 表示将题目文本转图片的指令。
/// </summary>
[Command("转图片")]
[CommandUsage("转图片 <题目字符串>", IsSyntax = true)]
public sealed class PuzzleToImageCommand : Command
{
	/// <inheritdoc/>
	public override async Task GroupCallback(ChatMessageApi api, ChatMessage message)
	{
		if (message.GetPlainArguments() is var str && Grid.TryParse(str, out var grid))
		{
			using var canvas = new GridCanvas(600, 10);
			canvas.Clear();
			canvas.DrawGrid(in grid);

			var imageLocalPath = $@"{Program.BotCachePath}\output.png";
			canvas.SavePictureTo(imageLocalPath);

			var (imageUrl, _) = await CloudObjectStorage.UploadFileAsync(imageLocalPath);
			await api.SendGroupImageAsync(message, imageUrl);

			File.Delete(imageLocalPath);
		}
		else
		{
			await api.SendGroupMessageAsync(message, DefaultInfoString);
		}
	}
}
