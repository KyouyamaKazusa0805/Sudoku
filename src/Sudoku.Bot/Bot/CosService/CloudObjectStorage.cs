namespace Sudoku.Bot.CosService;

/// <summary>
/// 提供一个模拟出图床效果的方式——使用腾讯云的云端对象存储服务上传本地图片，以此方式达成图床对接。
/// </summary>
public static class CloudObjectStorage
{
	/// <summary>
	/// 表示本地读取的配置。当初始化失败（如本地必要文件都不存在）时，此字段数值为 <see langword="null"/>。
	/// </summary>
	private static readonly CosConfig? Config;

	/// <summary>
	/// 提供一个上传图片服务的单例。当初始化失败（如本地必要文件都不存在）时，此字段数值为 <see langword="null"/>。
	/// </summary>
	private static readonly CosXml? CosServiceProvider;


	/// <summary>
	/// 初始化 <see cref="CosServiceProvider"/> 和 <see cref="Config"/> 字段的数据。
	/// </summary>
	/// <seealso cref="CosServiceProvider"/>
	/// <seealso cref="Config"/>
	static CloudObjectStorage()
	{
		if (!File.Exists(Program.CosConfigPath))
		{
			// 本地甚至连配置文件都找不着，直接退出，不初始化对象。
			return;
		}

		var json = File.ReadAllText(Program.CosConfigPath);
		Config = JsonSerializer.Deserialize<CosConfig>(json, Program.JsonOptions)!;
		CosServiceProvider = new CosXmlServer(
			new CosXmlConfig.Builder()
				.IsHttps(Config.IsHttps)
				.SetRegion(Config.Region)
				.SetDebugLog(Config.DebugLog)
				.Build(),
			new DefaultQCloudCredentialProvider(
				Config.SecretId,
				Config.SecretKey,
				Config.ExpiredTime
			)
		);
	}


	/// <summary>
	/// 从本地上传文件（一般是图片）到腾讯云存储桶，并返回 <see cref="UploadedResult"/> 结果。
	/// </summary>
	/// <param name="filePath">文件的本地绝对路径。</param>
	/// <returns>一个异步任务，并返回 <see cref="UploadedResult"/> 结果。</returns>
	/// <exception cref="InvalidOperationException">服务初始化失败时产生此异常。</exception>
	public static async Task<UploadedResult> UploadFileAsync(string filePath)
	{
		// 校验数据。如果单例初始化失败（如本地文件不存在）就直接退出。
		if (CosServiceProvider is null || Config is null)
		{
			throw new InvalidOperationException("服务初始化失败。");
		}

		var manager = new TransferManager(CosServiceProvider, new());
		var fileExtension = Path.GetExtension(filePath);
		var newFilePath = $"{Guid.NewGuid()}{fileExtension}";
		var uploadTask = new COSXMLUploadTask(Config.Bucket, newFilePath);
		uploadTask.SetSrcPath(filePath);

		// 返回结果。
		var resultLink = $"{Config.Link}/{newFilePath}";
		var uploadResult = await manager.UploadAsync(uploadTask);
		WriteLog($"图片上传图床成功，上传信息：{uploadResult.GetResultInfo()}");
		return new(resultLink, uploadResult.eTag);
	}
}
