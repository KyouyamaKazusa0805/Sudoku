/// <summary>
/// 主方法包裹的程序类。
/// </summary>
internal static partial class Program
{
	/// <summary>
	/// 腾讯云 COS 服务（Cloud Object Storage，云端对象存储）所需配置项的本地路径。
	/// </summary>
	public const string CosConfigPath = @"A:\QQ机器人\tencent-cos-config.json";

	/// <summary>
	/// 机器人的配置文件的本地路径。
	/// </summary>
	public const string BotConfigPath = @"A:\QQ机器人\bot.json";

	/// <summary>
	/// 机器人的临时缓存文件路径。
	/// </summary>
	public const string BotCachePath = @"A:\QQ机器人\cache";


	/// <summary>
	/// 表示桌面路径。
	/// </summary>
	public static readonly string DesktopPath = Environment.GetFolderPath(SpecialFolder.Desktop);

	/// <summary>
	/// 表示当前支持的游戏玩法模式。
	/// </summary>
	public static readonly GameMode[] GameModes = Enum.GetValues<GameMode>();

	/// <summary>
	/// 本地使用的解题工具。
	/// </summary>
	public static readonly Analyzer Analyzer = Analyzer.Balanced
		.WithUserDefinedOptions(new() { IsDirectMode = true, DistinctDirectMode = true });

	/// <summary>
	/// 在反序列化 JSON 期间使用到的解析控制选项。
	/// </summary>
	public static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

	/// <summary>
	/// 机器人的运行上下文。为一个并发字典，按群存储不同的上下文数据。
	/// </summary>
	internal static readonly ConcurrentDictionary<string, BotRunningContext> RunningContexts = new();

	/// <summary>
	/// 表示这个机器人是否是第一次启动。
	/// </summary>
	private static bool _isFirstLaunch = true;


	/// <summary>
	/// 本地出题工具。
	/// </summary>
	public static Generator Generator => new();

	/// <summary>
	/// 表示当前程序注册的匿名指令集。
	/// </summary>
	public static AnonymousCommand[] RegisteredAnonymousCommands => IAnonymousCommandBase.AssemblyCommands();

	/// <summary>
	/// 表示当前程序注册的指令集。
	/// </summary>
	public static Command[] RegisteredCommands => ICommandBase.AssemblyCommands();
}
