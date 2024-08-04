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
	/// 在反序列化 JSON 期间使用到的解析控制选项。
	/// </summary>
	public static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

	/// <summary>
	/// 表示这个机器人是否是第一次启动。
	/// </summary>
	private static bool _isFirstLaunch = true;
}
