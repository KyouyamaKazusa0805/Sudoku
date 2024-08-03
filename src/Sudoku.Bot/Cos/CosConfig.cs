namespace Sudoku.Bot.Cos;

/// <summary>
/// 本地的 COS 服务配置。
/// </summary>
internal sealed class CosConfig
{
	/// <summary>
	/// 表示是否以 HTTPS 进行访问。
	/// </summary>
	public bool IsHttps { get; set; } = true;

	/// <summary>
	/// 是否需要调试日志的输出。
	/// </summary>
	public bool DebugLog { get; set; } = false;

	/// <summary>
	/// 表示超时的时间（秒为单位）。
	/// </summary>
	public int ExpiredTime { get; set; } = 300;

	/// <summary>
	/// 本地的 Secret ID。
	/// </summary>
	public string SecretId { get; set; } = string.Empty;

	/// <summary>
	/// 本地的 Secret Key。
	/// </summary>
	public string SecretKey { get; set; } = string.Empty;

	/// <summary>
	/// 存储桶的区域。
	/// </summary>
	public string Region { get; set; } = string.Empty;

	/// <summary>
	/// 表示存储桶的名称。
	/// </summary>
	public string Bucket { get; set; } = string.Empty;

	/// <summary>
	/// 表示存储桶的路径地址。图片等文件的文件名，会直接添加到该地址的后面，形成的结果 URL 就是文件的最终访问地址。
	/// 地址为临时地址，临时可访问的有效时长为 1 个小时。
	/// </summary>
	public string Link { get; set; } = string.Empty;
}
