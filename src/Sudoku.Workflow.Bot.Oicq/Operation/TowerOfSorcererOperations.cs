namespace Sudoku.Workflow.Bot.Oicq.Operation;

/// <summary>
/// 提供魔塔相关的操作。
/// </summary>
internal static class TowerOfSorcererOperations
{
	/// <summary>
	/// 获取魔塔的第 <paramref name="index"/> 个题目。
	/// </summary>
	/// <param name="index">表示第几个题目。</param>
	/// <returns>返回一个 <see cref="Grid"/> 盘面对象。</returns>
	public static Grid GetPuzzleFor(int index)
	{
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			goto ThrowForPathNotExist;
		}

		folder = $"""{folder}\BotData""";
		if (!Directory.Exists(folder))
		{
			goto ThrowForPathNotExist;
		}

		folder = $"""{folder}\TowerOfSorcerer""";
		if (!Directory.Exists(folder))
		{
			goto ThrowForPathNotExist;
		}

		var puzzleFile = $"""{folder}\魔塔.txt""";
		if (!File.Exists(puzzleFile))
		{
			goto ThrowForPathNotExist;
		}

		var i = -1;
		foreach (var line in File.ReadAllLines(puzzleFile))
		{
			if (Grid.TryParse(line, out var grid) && ++i == index)
			{
				return grid;
			}
		}

		goto ThrowForFileIsBroken;

	ThrowForPathNotExist:
		throw new NotSupportedException("该功能不受支持，因为魔塔的题目不存在。");

	ThrowForFileIsBroken:
		throw new NotSupportedException("该功能不受支持，因为魔塔必需的题库已损坏。");
	}
}
