namespace Sudoku.Workflow.Bot.Oicq.Operation;

/// <summary>
/// 提供有关于每日一题的一些功能性操作。
/// </summary>
/// <remarks>
/// 由于文件读写操作较慢，又考虑到项目使用异步操作较多，因此本类型里所有的方法均为带锁的同步方法，基本等价于包裹了一层
/// <see langword="lock"/>(<see langword="typeof"/>(<see cref="DailyPuzzleOperations"/>))
/// 的代码片段。
/// </remarks>
public static class DailyPuzzleOperations
{
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
			throw new();
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
	/// 从本地读取每日一题的答案。
	/// </summary>
	/// <returns>每日一题的答案。如果当天没有记录（比如机器人临时维护）导致题目尚未生成，本地没有数据的时候，会返回 <see langword="null"/>。</returns>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static int[]? ReadDailyPuzzleAnswer()
	{
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			throw new();
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
}
