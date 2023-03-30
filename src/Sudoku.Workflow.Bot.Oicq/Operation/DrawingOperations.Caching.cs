namespace Sudoku.Workflow.Bot.Oicq.Operation;

partial class DrawingOperations
{
	/// <summary>
	/// 产生一个图片缓存路径，根据 <see cref="ISudokuPainter"/> 类型的实例给绘制出来，并保存到此缓存路径下。
	/// </summary>
	/// <param name="painter">一个 <see cref="ISudokuPainter"/> 的绘图实例。</param>
	/// <returns>图片在本地存储的缓存路径。</returns>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static string? GenerateCachedPicturePath(ISudokuPainter painter)
	{
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			return null;
		}

		var botDataFolder = $"""{folder}\BotData""";
		if (!Directory.Exists(botDataFolder))
		{
			Directory.CreateDirectory(botDataFolder);
		}

		var cachedPictureFolder = $"""{botDataFolder}\TempPictures""";
		if (!Directory.Exists(cachedPictureFolder))
		{
			Directory.CreateDirectory(cachedPictureFolder);
		}

		var targetPath = default(string?);
		for (var index = 0; index < int.MaxValue; index++)
		{
			var picturePath = $"""{cachedPictureFolder}\temp{(index == 0 ? string.Empty : index.ToString())}.png""";
			if (!File.Exists(picturePath))
			{
				targetPath = picturePath;
				break;
			}
		}
		if (targetPath is null)
		{
			// Cannot find a suitable path. This case is very special.
			return null;
		}

		painter.SaveTo(targetPath);

		return targetPath;
	}
}
