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
	/// 默认的 JSON 序列化配置项。
	/// </summary>
	private static readonly JsonSerializerOptions DefaultOptions = new() { WriteIndented = true };


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

		File.WriteAllText(groupLibraryDataFileName, Serialize(puzzleLibraries, DefaultOptions));
	}

	/// <summary>
	/// 表示是否指定的 <paramref name="library"/> 的名称和 <paramref name="name"/> 近似或相同。
	/// 所谓的近似，指的是名称的大小写被忽略掉的情况，比如用户打字的时候习惯使用小写：“sdc”，那么题库名叫“SDC”、“SdC”的时候都可以匹配上。
	/// </summary>
	/// <param name="library">题库数据。</param>
	/// <param name="name">题库名称。</param>
	/// <returns>一个 <see cref="bool"/> 结果，表示是否名称相同或近似。</returns>
	/// <remarks>
	/// 该方法作为本类型为数不多的、不操作 I/O 的方法，该方法不带有
	/// [<see cref="MethodImplAttribute"/>(<see cref="MethodImplOptions.Synchronized"/>)] 标记。
	/// </remarks>
	public static bool NameEquals(PuzzleLibrary library, string name) => library.Name.Equals(name, StringComparison.OrdinalIgnoreCase);

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
					let finalName = name[..name.LastIndexOf(".txt")] // 忽略掉末尾的 .txt。
					let fullName = fileInfo.FullName
					select new PuzzleLibrary { GroupId = groupId, Name = finalName, Path = fullName }
				).ToList(),
				DefaultOptions
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
	public static int GetPuzzlesCount(string groupId, string libraryName)
		=> GetLibrary(groupId, libraryName) switch { { } lib => GetPuzzlesCount(lib), _ => -1 };

	/// <summary>
	/// <inheritdoc cref="GetPuzzlesCount(string, string)" path="/summary"/>
	/// </summary>
	/// <param name="puzzleLibrary"><see cref="PuzzleLibrary"/> 的实例。</param>
	/// <returns>
	/// <inheritdoc cref="GetPuzzlesCount(string, string)" path="/returns"/>
	/// </returns>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static int GetPuzzlesCount(PuzzleLibrary puzzleLibrary)
		=> File.ReadLines(puzzleLibrary.Path).Count(static line => !string.IsNullOrWhiteSpace(line) && Grid.TryParse(line, out _));

	/// <summary>
	/// 通过指定的 <see cref="PuzzleLibrary"/> 数据实例，获得题库路径，以及完成题目的数量，以这两个数值来确定抽取的题目。
	/// </summary>
	/// <param name="puzzleLibrary">一个 <see cref="PuzzleLibrary"/> 类型的实例。</param>
	/// <returns>返回一个 <see cref="Grid"/> 实例，表示结果。</returns>
	/// <exception cref="InvalidOperationException">
	/// 如果题库已经完成，就会产生此异常。这里属于是一个补救措施，因为调用此方法之前应本身就先得确保题库可以抽取题目。
	/// </exception>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static Grid GetPuzzleFor(PuzzleLibrary puzzleLibrary)
	{
		var current = puzzleLibrary.FinishedPuzzlesCount;
		var total = GetPuzzlesCount(puzzleLibrary);
		if (current == total)
		{
			throw new InvalidOperationException("This puzzle library has already been finished.");
		}

		var path = puzzleLibrary.Path;
		var i = -1;
		foreach (var line in File.ReadLines(path))
		{
			// 注意，这里 i 初始值必须是 -1。因为循环迭代的过程之中，题目如果合法，那么这里会优先给 i 进行自增运算，所以 -1 初始使用的时候也会变为 0。
			// 如果你初始化的时候写的 0，那么第一次使用 i 就已经成 1 了。
			// 以及，我不喜欢写 i++ 在内联的表达式里，因为 i 在执行完的时候才会触发此表达式的执行，这一点要不是故意的，我一般都不会这么用。
			// 当然，for 循环最后一部分我还是写的 i++ 的，这个另说。
			if (Grid.TryParse(line, out var grid) && ++i == current)
			{
				return grid;
			}
		}

		throw new InvalidOperationException("Something goes wrong with this puzzle library.");
	}

	/// <summary>
	/// 根据群号（QQ）和题库名称去搜索该群的匹配题库。如果找不到，则返回 <see langword="null"/>。
	/// </summary>
	/// <param name="groupId">群号。</param>
	/// <param name="libraryName">题库名称。</param>
	/// <returns>返回 <see cref="PuzzleLibrary"/> 实例，表示题库的基本信息。</returns>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static PuzzleLibrary? GetLibrary(string groupId, string libraryName)
		=> GetLibraries(groupId) is { } libraries ? Array.Find(libraries, puzzleLibrary => NameEquals(puzzleLibrary, libraryName)) : null;

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
