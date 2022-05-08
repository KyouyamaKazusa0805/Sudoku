namespace Sudoku.Bot.Communication;

/// <summary>
/// 经过封装的HttpClient
/// <para>内置了请求日志功能</para>
/// </summary>
public static class BotHttpClient
{
	/// <summary>
	/// 临时冻结无权限访问的URL
	/// <para>
	/// value.Item1 - 解封时间(DateTime)
	/// value.Item2 - 再次封禁增加的时间(TimeSpan)
	/// </para>
	/// </summary>
	private static readonly Dictionary<string, FreezeTime> FreezeUrl = new();


	/// <summary>
	/// URL访问失败的默认冻结时间
	/// </summary>
	public static TimeSpan FreezeAddTime { get; set; } = TimeSpan.FromSeconds(30);

	/// <summary>
	/// URL访问失败的最高冻结时间
	/// </summary>
	public static TimeSpan FreezeMaxTime { get; set; } = TimeSpan.FromHours(1);

	/// <summary>
	/// Http客户端
	/// <para>这里设置禁止重定向：AllowAutoRedirect = false</para>
	/// <para>这里设置超时时间为15s</para>
	/// </summary>
	public static HttpClient HttpClient { get; }
		= new(new HttpLoggingHandler(new HttpClientHandler { AllowAutoRedirect = false }))
		{
			Timeout = TimeSpan.FromSeconds(15)
		};


	/// <summary>
	/// 发起HTTP异步请求
	/// </summary>
	/// <param name="request">请求消息</param>
	/// <param name="action">请求失败的回调函数</param>
	/// <returns></returns>
	public static async Task<HttpResponseMessage?> SendAsync(
		HttpRequestMessage request, Action<HttpResponseMessage, FreezeTime>? action = null)
	{
		string reqUrl = request.RequestUri!.ToString();
		if (FreezeUrl.TryGetValue(reqUrl, out var freezeTime) && freezeTime.EndTime > DateTime.Now)
		{
			Log.Warn($"[HttpSend] 目标接口处于冻结状态，暂时无法访问：{reqUrl}");
			return null;
		}

		var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
		if (response.IsSuccessStatusCode)
		{
			if (FreezeUrl.ContainsKey(reqUrl))
			{
				FreezeUrl.Remove(reqUrl);
			}

			return response;
		}

		if (response.Content.Headers.ContentType?.MediaType == "application/json")
		{
			string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var err = JsonSerializer.Deserialize<ApiStatus>(responseContent);
			if (err?.Code >= 400)
			{
				if (FreezeUrl.TryGetValue(reqUrl, out freezeTime))
				{
					freezeTime.AddTime *= 2;
					if (freezeTime.AddTime > FreezeMaxTime)
					{
						freezeTime.AddTime = FreezeMaxTime;
					}

					freezeTime.EndTime = DateTime.Now + freezeTime.AddTime;
				}
				else
				{
					freezeTime = new() { AddTime = TimeSpan.FromSeconds(5) };

					// 重点打击 11264 和 11265 错误，无接口访问权限；轻微处理其它错误
					freezeTime.EndTime = DateTime.Now
						+ (err?.Code == 11264 || err?.Code == 11265 ? FreezeAddTime : freezeTime.AddTime);
				}
			}
		}

		freezeTime ??= new() { EndTime = DateTime.Now, AddTime = TimeSpan.FromSeconds(5) };
		action?.Invoke(response, freezeTime);

		return null;
	}

	/// <summary>
	/// HTTP异步GET
	/// </summary>
	/// <param name="url">请求地址</param>
	/// <returns></returns>
	public static async Task<HttpResponseMessage?> GetAsync(string url)
	{
		HttpRequestMessage request = new() { RequestUri = new Uri(url), Content = null, Method = HttpMethod.Get };
		return await HttpClient.SendAsync(request).ConfigureAwait(false);
	}

	/// <summary>
	/// HTTP异步Post
	/// </summary>
	/// <param name="url">请求地址</param>
	/// <param name="content">请求内容</param>
	/// <returns></returns>
	public static async Task<HttpResponseMessage?> PostAsync(string url, HttpContent content)
	{
		HttpRequestMessage request = new() { RequestUri = new Uri(url), Content = content, Method = HttpMethod.Post };
		return await HttpClient.SendAsync(request).ConfigureAwait(false);
	}

	/// <summary>
	/// HTTP异步Put
	/// </summary>
	/// <param name="url">请求地址</param>
	/// <param name="content">请求内容</param>
	/// <returns></returns>
	public static async Task<HttpResponseMessage?> PutAsync(string url, HttpContent content)
	{
		HttpRequestMessage request = new() { RequestUri = new Uri(url), Content = content, Method = HttpMethod.Put };
		return await HttpClient.SendAsync(request).ConfigureAwait(false);
	}

	/// <summary>
	/// HTTP异步Delete
	/// </summary>
	/// <param name="url">请求地址</param>
	/// <param name="content">请求内容</param>
	/// <returns></returns>
	public static async Task<HttpResponseMessage?> DeleteAsync(string url, HttpContent content)
	{
		HttpRequestMessage request = new() { RequestUri = new Uri(url), Content = content, Method = HttpMethod.Delete };
		return await HttpClient.SendAsync(request).ConfigureAwait(false);
	}
}
