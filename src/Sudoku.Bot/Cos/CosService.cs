namespace Sudoku.Bot.Cos;

/// <summary>
/// 使用腾讯云的对象存储桶服务上传本地图片，以此方式达成图床对接。
/// </summary>
public static class CosService
{
	/// <summary>
	/// 提供一个上传图片服务的单例。
	/// </summary>
	internal static readonly CosXml? ServiceProvider;

	/// <summary>
	/// 表示本地读取的配置。
	/// </summary>
	private static readonly CosConfig? CosConfig;


	/// <summary>
	/// 初始化 <see cref="ServiceProvider"/> 和 <see cref="CosConfig"/> 字段的数据。
	/// </summary>
	/// <seealso cref="ServiceProvider"/>
	/// <seealso cref="CosConfig"/>
	static CosService()
	{
		if (!File.Exists(Program.CosConfigPath))
		{
			// 本地甚至连配置文件都找不着，直接退出，不初始化对象。
			return;
		}

		var json = File.ReadAllText(Program.CosConfigPath);
		CosConfig = JsonSerializer.Deserialize<CosConfig>(json, Program.JsonOptions)!;
		ServiceProvider = new CosXmlServer(
			new CosXmlConfig.Builder()
				.IsHttps(CosConfig.IsHttps)
				.SetRegion(CosConfig.Region)
				.SetDebugLog(CosConfig.DebugLog)
				.Build(),
			new DefaultQCloudCredentialProvider(
				CosConfig.SecretId,
				CosConfig.SecretKey,
				CosConfig.ExpiredTime
			)
		);
	}


	/// <summary>
	/// 从本地上传文件（一般是图片）到腾讯云存储桶，并返回 <see cref="string"/> 结果，表示其文件上传后的路径。
	/// </summary>
	/// <param name="filePath">文件的本地绝对路径。</param>
	/// <returns>一个异步任务，并返回 <see cref="string"/> 结果，是上传之后的文件的路径。</returns>
	/// <exception cref="InvalidOperationException">服务初始化失败时产生此异常。</exception>
	public static async Task<string> UploadFileAsync(string filePath)
	{
		// 校验数据。如果单例初始化失败（如本地文件不存在）就直接退出。
		if (ServiceProvider is null || CosConfig is null)
		{
			throw new InvalidOperationException("服务初始化失败。");
		}

		// 初始化 TransferManager 对象。
		var config = new TransferConfig();
		var manager = new TransferManager(ServiceProvider, config);

		// 上传对象。
		var fileExtension = Path.GetExtension(filePath);
		var newFilePath = $"{Guid.NewGuid()}{fileExtension}";
		var uploadTask = new COSXMLUploadTask(CosConfig.Bucket, newFilePath);
		uploadTask.SetSrcPath(filePath);

		// 返回结果。
		var resultLink = $"{CosConfig.Link}/{newFilePath}";
		var uploadResult = await manager.UploadAsync(uploadTask);
		WriteLog($"图片上传图床成功，上传信息：{uploadResult.GetResultInfo()}");
		return resultLink;
	}
}
