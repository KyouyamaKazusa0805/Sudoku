namespace Sudoku.Workflow.Bot.Oicq.Handlers;

/// <summary>
/// 提供一个文件或文件夹存储（读、写）操作的处理器类型。
/// </summary>
/// <remarks>
/// 由于文件读写操作较慢，又考虑到项目使用异步操作较多，因此本类型里所有的方法均为带锁的同步方法，基本等价于包裹了一层
/// <see langword="lock"/>(<see langword="typeof"/>(<see cref="StorageHandler"/>))
/// 的代码片段。
/// </remarks>
public static class StorageHandler
{
	/// <summary>
	/// 从本地读取每日一题的答案。
	/// </summary>
	/// <returns>每日一题的答案。如果当天没有记录（比如机器人临时维护）导致题目尚未生成，本地没有数据的时候，会返回 <see langword="null"/>。</returns>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static int[]? ReadDailyPuzzleAnswer()
	{
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			throw new InvalidOperationException("严重错误：无法找到“我的文档”文件夹。");
		}

		var botDataFolder = $"""{folder}\BotData""";
		if (!Directory.Exists(botDataFolder))
		{
			Directory.CreateDirectory(botDataFolder);
		}

		var dailyPuzzleFolder = $"""{botDataFolder}\DailyPuzzle""";
		if (!Directory.Exists(dailyPuzzleFolder))
		{
			Directory.CreateDirectory(dailyPuzzleFolder);
		}

		var fileName = $"""{dailyPuzzleFolder}\Answer.json""";
		if (!File.Exists(fileName))
		{
			return null;
		}

		var json = File.ReadAllText(fileName);
		return Deserialize<int[]>(json);
	}

	/// <summary>
	/// 从配置文件路径读取所有用户信息，并返回 <see cref="User"/> 数组。
	/// </summary>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static User[]? ReadAll(Predicate<string> predicate)
	{
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			return null;
		}

		var botDataFolder = $"""{folder}\BotData""";
		if (!Directory.Exists(botDataFolder))
		{
			return null;
		}

		var botUsersDataFolder = $"""{botDataFolder}\Users""";
		if (!Directory.Exists(botUsersDataFolder))
		{
			return null;
		}

		var result = new List<User>();
		var di = new DirectoryInfo(botUsersDataFolder);
		foreach (var file in di.EnumerateFiles("*.json"))
		{
			var id = Path.GetFileNameWithoutExtension(file.FullName);
			if (predicate(id))
			{
				result.Add(Deserialize<User>(File.ReadAllText(file.FullName))!);
			}
		}

		return result.ToArray();
	}

	/// <summary>
	/// 读取指定 QQ 号码的用户数据，并返回 <see cref="User"/> 实例。如果用户不存在本地数据，则默认将参数 <paramref name="default"/> 的结果返回。
	/// </summary>
	[MethodImpl(MethodImplOptions.Synchronized)]
	[return: NotNullIfNotNull(nameof(@default))]
	public static User? Read(string userId, User? @default = null)
	{
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			return @default;
		}

		var botDataFolder = $"""{folder}\BotData""";
		if (!Directory.Exists(botDataFolder))
		{
			return @default;
		}

		var botUsersDataFolder = $"""{botDataFolder}\Users""";
		if (!Directory.Exists(botUsersDataFolder))
		{
			return @default;
		}

		var fileName = $"""{botUsersDataFolder}\{userId}.json""";
		return File.Exists(fileName) ? Deserialize<User>(File.ReadAllText(fileName)) : @default;
	}

	/// <summary>
	/// 将每日一题的答案抄写到本地。
	/// </summary>
	/// <param name="grid">题目答案。</param>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static void WriteDailyPuzzleAnswer(scoped in Grid grid)
	{
		var answerList = new int[9];
		for (var i = 0; i < 9; i++)
		{
			answerList[i] = grid[HouseCells[17][i]];
		}

		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			throw new InvalidOperationException("严重错误：无法找到“我的文档”文件夹。");
		}

		var botDataFolder = $"""{folder}\BotData""";
		if (!Directory.Exists(botDataFolder))
		{
			Directory.CreateDirectory(botDataFolder);
		}

		var dailyPuzzleFolder = $"""{botDataFolder}\DailyPuzzle""";
		if (!Directory.Exists(dailyPuzzleFolder))
		{
			Directory.CreateDirectory(dailyPuzzleFolder);
		}

		var fileName = $"""{dailyPuzzleFolder}\Answer.json""";
		File.WriteAllText(fileName, Serialize(answerList));
	}

	/// <summary>
	/// 将 <see cref="User"/> 数据写出到本地文件。
	/// </summary>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static void Write(User userData)
	{
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			throw new InvalidOperationException("严重错误：无法找到“我的文档”文件夹。");
		}

		var botDataFolder = $"""{folder}\BotData""";
		if (!Directory.Exists(botDataFolder))
		{
			Directory.CreateDirectory(botDataFolder);
		}

		var botUsersDataFolder = $"""{botDataFolder}\Users""";
		if (!Directory.Exists(botUsersDataFolder))
		{
			Directory.CreateDirectory(botUsersDataFolder);
		}

		var userId = userData.Number;
		var fileName = $"""{botUsersDataFolder}\{userId}.json""";
		File.WriteAllText(fileName, Serialize(userData));
	}

	/// <summary>
	/// 产生一个图片缓存路径，它会将指定参数的回调函数，所产生的 <see cref="ISudokuPainter"/> 的对象给绘制出来，并保存到此缓存路径下。
	/// </summary>
	/// <param name="painterCreator">回调函数，用来生成一个 <see cref="ISudokuPainter"/> 的绘图实例。</param>
	/// <returns>图片在本地存储的缓存路径。</returns>
	[SupportedOSPlatform(OperatingSystemNames.Windows)]
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static string? GenerateCachedPicturePath(Func<ISudokuPainter> painterCreator)
	{
		var painter = painterCreator();
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
