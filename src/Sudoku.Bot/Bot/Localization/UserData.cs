namespace Sudoku.Bot.Localization;

/// <summary>
/// 表示一个用户的基本信息。
/// </summary>
/// <remarks>
/// 特别注意，所有这个类型里的内容都是不会及时更新的。你需要调用 <see cref="Read(string)"/> 读取后，
/// 写入新数值，并用 <see cref="Write(UserData)"/> 刷新本地配置。
/// </remarks>
/// <seealso cref="Read(string)"/>
/// <seealso cref="Write(UserData)"/>
public sealed class UserData
{
	/// <summary>
	/// 表示序列化用到的配置。
	/// </summary>
	private static readonly JsonSerializerOptions Options = new()
	{
		IncludeFields = false,
		IgnoreReadOnlyFields = true,
		PropertyNameCaseInsensitive = false,
		WriteIndented = true,
		IndentCharacter = '\t',
		IndentSize = 1
	};

	/// <summary>
	/// 一个文件锁。文件锁会在用户尝试访问文件操作时触发。这里简单作一个限制，只要是访问文件，无论读写，无论是否读取/写入的是一个文件，都锁上。
	/// </summary>
	private static readonly Lock FileLock = new();


	/// <summary>
	/// 表示用户的虚拟 ID。
	/// </summary>
	public string Id { get; set; } = "";

	/// <summary>
	/// 表示用户的昵称。因为 QQ 尚未提供访问用户实际 QQ 号和昵称的权限，所以只能用虚拟昵称来绑定用户。不同用户的虚拟昵称可以一样，因为虚拟 ID 可以直接区分。
	/// </summary>
	public string VirtualNickname { get; set; } = "<匿名>";

	/// <summary>
	/// 连续签到的天数。
	/// </summary>
	public int ComboCheckedInDays { get; set; } = 1;

	/// <summary>
	/// 表示用户的经验值。
	/// </summary>
	public long ExperienceValue { get; set; } = 0;

	/// <summary>
	/// 表示用户的金币。
	/// </summary>
	public long CoinValue { get; set; } = 0;

	/// <summary>
	/// 表示用户上一轮签到的时间。
	/// </summary>
	public DateTime LastCheckIn { get; set; } = DateTime.MinValue;


	/// <summary>
	/// 更新用户信息的对象，通过参数更新，然后更新完内容后，再重新储存到本地。
	/// </summary>
	/// <param name="id">用户的 ID。</param>
	/// <param name="dataChanger">对象的实际内容。</param>
	public static void Update(string id, Action<UserData> dataChanger)
	{
		lock (FileLock)
		{
			var data = Read(id);
			dataChanger(data);
			Write(data);
		}
	}

	/// <summary>
	/// 读取指定用户的 ID 的配置文件。如果文件不存在，则会创建一个，并读取其默认结果。
	/// </summary>
	/// <param name="id">用户的 ID。</param>
	/// <returns>用户信息。</returns>
	public static UserData Read(string id)
	{
		lock (FileLock)
		{
			var path = $@"A:\QQ机器人\user-data\{id}.json";
			if (!File.Exists(path))
			{
				File.WriteAllText(
					path,
					$$"""
				{
					"{{nameof(Id)}}": "{{id}}",
					"{{nameof(VirtualNickname)}}": "<匿名>",
					"{{nameof(ComboCheckedInDays)}}": 1,
					"{{nameof(LastCheckIn)}}": {{DateTime.MinValue}},
					"{{nameof(ExperienceValue)}}": 0,
					"{{nameof(CoinValue)}}": 0
				}
				"""
				);
				return new() { Id = id };
			}

			var json = File.ReadAllText(path);
			var result = JsonSerializer.Deserialize<UserData>(json, Options);
			return result!;
		}
	}

	/// <summary>
	/// 写入指定用户的 ID 的配置文件，并直接替换源文件。
	/// </summary>
	/// <param name="data">配置文件。</param>
	public static void Write(UserData data)
	{
		lock (FileLock)
		{
			var path = $@"A:\QQ机器人\user-data\{data.Id}.json";
			var json = JsonSerializer.Serialize(data, Options);
			File.WriteAllText(path, json);
		}
	}
}
