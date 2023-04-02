namespace Sudoku.Workflow.Bot.Oicq.Operation;

/// <summary>
/// 提供一个文件或文件夹存储（读、写）操作的处理器类型。
/// </summary>
/// <remarks>
/// 由于文件读写操作较慢，又考虑到项目使用异步操作较多，因此本类型里所有的方法均为带锁的同步方法，基本等价于包裹了一层
/// <see langword="lock"/>(<see langword="typeof"/>(<see cref="UserOperations"/>))
/// 的代码片段。
/// </remarks>
public static class UserOperations
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
				result.Add(Deserialize<User>(File.ReadAllText(file.FullName), DefaultOptions)!);
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
		return File.Exists(fileName) ? Deserialize<User>(File.ReadAllText(fileName), DefaultOptions) : @default;
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
			throw new();
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
		File.WriteAllText(fileName, Serialize(userData, DefaultOptions));
	}
}
