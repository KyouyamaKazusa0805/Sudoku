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

#if false
	/// <summary>
	/// Gets the puzzle library configuration file of the target group specified as its group ID.
	/// </summary>
	/// <param name="groupId">The group ID.</param>
	/// <returns>The group library collection.</returns>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static PuzzleLibraryCollection? ReadLibraryConfiguration(string groupId)
	{
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			throw new InvalidOperationException("严重错误：无法找到“我的文档”文件夹。");
		}

		var botDataFolder = $"""{folder}\BotData""";
		if (!Directory.Exists(botDataFolder))
		{
			return null;
		}

		var libraryFolder = $"""{botDataFolder}\Puzzles""";
		if (!Directory.Exists(libraryFolder))
		{
			return null;
		}

		var groupLibraryFolder = $"""{libraryFolder}\{groupId}""";
		if (!Directory.Exists(groupLibraryFolder))
		{
			return null;
		}

		var filePath = $"""{groupLibraryFolder}\config.json""";
		if (!File.Exists(filePath))
		{
			return null;
		}

		var json = File.ReadAllText(filePath);
		return Deserialize<PuzzleLibraryCollection>(json);
	}

	/// <summary>
	/// Gets the puzzle library for the target group specified as its group ID.
	/// </summary>
	/// <param name="groupId">The group ID.</param>
	/// <returns>The puzzle library data.</returns>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static PuzzleLibraryCollection? ReadLibraries(string groupId)
	{
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			throw new InvalidOperationException("严重错误：无法找到“我的文档”文件夹。");
		}

		var botDataFolder = $"""{folder}\BotData""";
		if (!Directory.Exists(botDataFolder))
		{
			return null;
		}

		var libraryFolder = $"""{botDataFolder}\Puzzles""";
		if (!Directory.Exists(libraryFolder))
		{
			return null;
		}

		var groupLibraryFolder = $"""{libraryFolder}\{groupId}""";
		if (!Directory.Exists(groupLibraryFolder))
		{
			return null;
		}

		var final = new List<PuzzleLibraryData>();
		var di = new DirectoryInfo(groupLibraryFolder);
		foreach (var textFile in di.EnumerateFiles("*.txt"))
		{
			if (textFile is { Length: not 0, FullName: var path })
			{
				final.Add(new() { Name = Path.GetFileNameWithoutExtension(path), GroupId = groupId, PuzzleFilePath = path });
			}
		}

		return new() { GroupId = groupId, PuzzleLibraries = final };
	}

	/// <summary>
	/// Gets the puzzle library for the target group specified as its group ID, and the specified library name.
	/// </summary>
	/// <param name="groupId">The group ID.</param>
	/// <param name="libraryName">The library name.</param>
	/// <returns>The puzzle library data.</returns>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static PuzzleLibraryData? ReadLibrary(string groupId, string libraryName)
	{
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			throw new InvalidOperationException("严重错误：无法找到“我的文档”文件夹。");
		}

		var botDataFolder = $"""{folder}\BotData""";
		if (!Directory.Exists(botDataFolder))
		{
			return null;
		}

		var libraryFolder = $"""{botDataFolder}\Puzzles""";
		if (!Directory.Exists(libraryFolder))
		{
			return null;
		}

		var groupLibraryFolder = $"""{libraryFolder}\{groupId}""";
		if (!Directory.Exists(groupLibraryFolder))
		{
			return null;
		}

		var di = new DirectoryInfo(groupLibraryFolder);
		return di.EnumerateFiles($"{libraryName}.txt").FirstOrDefault() switch
		{
			{ FullName: var fullName } => new() { Name = Path.GetFileNameWithoutExtension(fullName), GroupId = groupId, PuzzleFilePath = fullName },
			_ => null
		};
	}
#endif

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

#if false
	/// <summary>
	/// Writes library data to the bot configuration file path.
	/// </summary>
	/// <param name="libraryCollection">The library collection.</param>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static void WriteLibraryConfiguration(PuzzleLibraryCollection libraryCollection)
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

		var libraryFolder = $"""{botDataFolder}\Puzzles""";
		if (!Directory.Exists(libraryFolder))
		{
			Directory.CreateDirectory(libraryFolder);
		}

		var groupId = libraryCollection.GroupId;
		var groupLibraryFolder = $"""{libraryFolder}\{groupId}""";
		if (!Directory.Exists(groupLibraryFolder))
		{
			Directory.CreateDirectory(groupLibraryFolder);
		}

		var fileName = $"""{groupLibraryFolder}\config.json""";
		File.WriteAllText(fileName, Serialize(libraryCollection));
	}

	/// <summary>
	/// Checks for the specified library, to get the total number of valid puzzles stored in the file.
	/// </summary>
	/// <param name="groupId">The group ID.</param>
	/// <param name="libraryName">The library name.</param>
	/// <returns>
	/// The number of valid puzzles. -1 for the case that the specified group does not store any library whose name is the specified name.
	/// </returns>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static int CheckValidPuzzlesCountInPuzzleLibrary(string groupId, string libraryName)
		=> ReadLibrary(groupId, libraryName) switch { { } lib => CheckValidPuzzlesCountInPuzzleLibrary(lib), _ => -1 };

	/// <inheritdoc cref="CheckValidPuzzlesCountInPuzzleLibrary(string, string)"/>
	/// <param name="puzzleLibrary">The <see cref="PuzzleLibraryData"/> instance.</param>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static int CheckValidPuzzlesCountInPuzzleLibrary(PuzzleLibraryData puzzleLibrary)
	{
		return File.ReadLines(puzzleLibrary.PuzzleFilePath).Count(lineValidator);


		static bool lineValidator(string line) => !string.IsNullOrWhiteSpace(line) && Grid.TryParse(line, out _);
	}
#endif

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
