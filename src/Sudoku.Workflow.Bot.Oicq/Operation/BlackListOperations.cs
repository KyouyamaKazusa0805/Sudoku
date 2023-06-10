namespace Sudoku.Workflow.Bot.Oicq.Operation;

/// <summary>
/// 表示关于黑名单操作的类型。
/// </summary>
internal static class BlackListOperations
{
	/// <summary>
	/// 将指定用户账号的用户添加进入黑名单之中。
	/// </summary>
	/// <param name="userId">账号。</param>
	/// <param name="deleteData">表示在加入黑名单后，该用户的账号信息是否需要被删除。默认为 <see langword="false"/>。</param>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static void Append(string userId, bool deleteData = false)
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

		var fileName = $"""{botDataFolder}\BlackList.txt""";
		File.AppendAllText(fileName, $"{userId}{Environment.NewLine}");

		if (deleteData)
		{
			UserOperations.Write(new() { Number = userId });
		}
	}

	/// <summary>
	/// 查询本地黑名单列表里，是否有指定用户。
	/// </summary>
	/// <param name="userId">指定用户的账号。</param>
	/// <returns>一个 <see cref="bool"/> 结果，表示他是否在该表里。</returns>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static bool Contains(string userId)
	{
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			return false;
		}

		var botDataFolder = $"""{folder}\BotData""";
		if (!Directory.Exists(botDataFolder))
		{
			return false;
		}

		var fileName = $"""{botDataFolder}\BlackList.txt""";
		if (!File.Exists(fileName))
		{
			return false;
		}

		return File.ReadAllLines(fileName).Any(line => line.Contains(userId));
	}
}
