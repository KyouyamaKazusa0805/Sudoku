namespace Sudoku.Workflow.Bot.Oicq.Storage;

/// <summary>
/// 一个静态类，提供一系列和本地交互有关题库数据的操作方法集。
/// </summary>
/// <remarks>
/// <inheritdoc cref="StorageHandler" path="/remarks"/>
/// </remarks>
public static class PuzzleLibraryOperations
{
	/// <summary>
	/// 更新指定群名的指定题库名称的题库数据。
	/// </summary>
	/// <param name="groupId">群号。</param>
	/// <param name="library">题库数据，这个数据用于替换掉。</param>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static void UpdateLibrary(string groupId, PuzzleLibrary library)
	{
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			throw new InvalidOperationException("严重错误：无法找到“我的文档”文件夹。");
		}

		var botDataFolder = $"""{folder}\BotData""";
		if (!Directory.Exists(botDataFolder))
		{
			return;
		}

		var libraryFolder = $"""{botDataFolder}\PuzzleLibrary""";
		if (!Directory.Exists(libraryFolder))
		{
			return;
		}

		var groupLibraryFolder = $"""{libraryFolder}\{groupId}""";
		if (!Directory.Exists(groupLibraryFolder))
		{
			return;
		}

		var groupLibraryDataFileName = $"""{groupLibraryFolder}\{groupId}.json""";
		if (!File.Exists(groupLibraryDataFileName))
		{
			return;
		}

		var puzzleLibraries = Deserialize<List<PuzzleLibrary>>(File.ReadAllText(groupLibraryDataFileName))!;
		if (puzzleLibraries.FindIndex(puzzleLibrary => puzzleLibrary.Name == library.Name) is var index and not -1)
		{
			puzzleLibraries[index] = library;
		}

		File.WriteAllText(groupLibraryDataFileName, Serialize(puzzleLibraries));
	}

	/// <summary>
	/// 初始化指定群号（QQ）的群的所有题库的本地数据。
	/// </summary>
	/// <param name="groupId">群号。</param>
	/// <returns>返回一个 <see cref="bool"/> 结果，表示是否完成初始化。如果失败（例如中途文件夹不存在等），则返回 <see langword="false"/>。</returns>
	/// <remarks>
	/// 在“<c>BotData/PuzzleLibrary</c>”文件夹里包含众多子文件夹，这些文件夹命名均为群号，表示的是一个群里包裹的所有题库完成数据和题库本身。
	/// 通过“<c>BotData/PuzzleLibrary/群号</c>”文件夹里搜索所有文本文件，就可以获得指定群的题库数据；
	/// 而和文件夹同名的 JSON 文件则是该群完成的此群的题库数据的序列化文件。
	/// </remarks>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static bool Initialize(string groupId)
	{
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			throw new InvalidOperationException("严重错误：无法找到“我的文档”文件夹。");
		}

		var botDataFolder = $"""{folder}\BotData""";
		if (!Directory.Exists(botDataFolder))
		{
			return false;
		}

		var libraryFolder = $"""{botDataFolder}\PuzzleLibrary""";
		if (!Directory.Exists(libraryFolder))
		{
			return false;
		}

		var groupLibraryFolder = $"""{libraryFolder}\{groupId}""";
		if (!Directory.Exists(groupLibraryFolder))
		{
			return false;
		}

		File.WriteAllText(
			$"""{groupLibraryFolder}\{groupId}.json""",
			Serialize(
				(
					from fileInfo in new DirectoryInfo(groupLibraryFolder).EnumerateFiles("*.txt")
					let name = fileInfo.Name
					let fullName = fileInfo.FullName
					select new PuzzleLibrary { GroupId = groupId, Name = name, Path = fullName }
				).ToList()
			)
		);
		return true;
	}

	/// <summary>
	/// 根据指定群号（QQ）和题库名称，获取该题库的完整题目的数量。
	/// </summary>
	/// <param name="groupId">群号。</param>
	/// <param name="libraryName">题库名称。</param>
	/// <returns>
	/// 返回一个数字，表示这个题库有多少个题目。如果题库没有找到，则返回 -1。
	/// </returns>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static int CheckValidPuzzlesCountInPuzzleLibrary(string groupId, string libraryName)
		=> GetLibrary(groupId, libraryName) switch { { } lib => CheckValidPuzzlesCountInPuzzleLibrary(lib), _ => -1 };

	/// <summary>
	/// <inheritdoc cref="CheckValidPuzzlesCountInPuzzleLibrary(string, string)" path="/summary"/>
	/// </summary>
	/// <param name="puzzleLibrary"><see cref="PuzzleLibrary"/> 的实例。</param>
	/// <returns>
	/// <inheritdoc cref="CheckValidPuzzlesCountInPuzzleLibrary(string, string)" path="/returns"/>
	/// </returns>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static int CheckValidPuzzlesCountInPuzzleLibrary(PuzzleLibrary puzzleLibrary)
		=> File.ReadLines(puzzleLibrary.Path).Count(static line => !string.IsNullOrWhiteSpace(line) && Grid.TryParse(line, out _));

	/// <summary>
	/// 根据群号（QQ）和题库名称去搜索该群的匹配题库。如果找不到，则返回 <see langword="null"/>。
	/// </summary>
	/// <param name="groupId">群号。</param>
	/// <param name="libraryName">题库名称。</param>
	/// <returns>返回 <see cref="PuzzleLibrary"/> 实例，表示题库的基本信息。</returns>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static PuzzleLibrary? GetLibrary(string groupId, string libraryName)
		=> GetLibraries(groupId) is { } libraries ? Array.Find(libraries, puzzleLibrary => puzzleLibrary.Name == libraryName) : null;

	/// <summary>
	/// 根据群号（QQ）获取本群的所有题库信息，并返回一个 <see cref="PuzzleLibrary"/> 构成的数组。如果本群不存在本地数据，
	/// 则返回 <see langword="null"/>。
	/// </summary>
	/// <param name="groupId">群号。</param>
	/// <returns>一个由 <see cref="PuzzleLibrary"/> 元素构成的数组。</returns>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static PuzzleLibrary[]? GetLibraries(string groupId)
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

		var libraryFolder = $"""{botDataFolder}\PuzzleLibrary""";
		if (!Directory.Exists(libraryFolder))
		{
			return null;
		}

		var groupLibraryFolder = $"""{libraryFolder}\{groupId}""";
		if (!Directory.Exists(groupLibraryFolder))
		{
			return null;
		}

		var groupLibraryDataFileName = $"""{groupLibraryFolder}\{groupId}.json""";
		if (!File.Exists(groupLibraryDataFileName))
		{
			return null;
		}

		return Deserialize<PuzzleLibrary[]>(File.ReadAllText(groupLibraryDataFileName));
	}

	/// <summary>
	/// <inheritdoc cref="GetLibraries(string)" path="/summary"/>
	/// </summary>
	/// <param name="group">一个 <see cref="Group"/> 对象，表示的是当前操作的群的基本数据。</param>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static PuzzleLibrary[]? GetLibraries(Group group) => GetLibraries(group.Id);
}
