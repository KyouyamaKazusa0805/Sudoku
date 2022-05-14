namespace Sudoku.Bot.Communication;

/// <summary>
/// Provides a data type that can easily control the messaging for bot client.
/// </summary>
internal static class BotHttpClient
{
	/// <summary>
	/// Indicates the URL that the bot cannot visit due to lacking of permission.
	/// </summary>
	private static readonly Dictionary<string, FreezeTime> FreezeUrl = new();


	/// <summary>
	/// Indicates the default time that freeze the bot.
	/// </summary>
	public static TimeSpan FreezeAddTime { get; set; } = TimeSpan.FromSeconds(30);

	/// <summary>
	/// Indicates the maximum time that can freeze the bot elapsed.
	/// </summary>
	public static TimeSpan FreezeMaxTime { get; set; } = TimeSpan.FromHours(1);

	/// <summary>
	/// Indicates the client instance.
	/// </summary>
	public static HttpClient HttpClient { get; }
		= new(new HttpLoggingHandler(new HttpClientHandler { AllowAutoRedirect = false }))
		{
			Timeout = TimeSpan.FromSeconds(15)
		};


	/// <summary>
	/// Sends the request asynchronously.
	/// </summary>
	/// <param name="request">The main message for the request.</param>
	/// <param name="failedCallback">The callback that is invoked when the request is failed.</param>
	/// <returns>
	/// A task instance encapsulates the <see cref="HttpResponseMessage"/> instance as the result value.
	/// </returns>
	public static async Task<HttpResponseMessage?> SendAsync(
		HttpRequestMessage request, Action<HttpResponseMessage, FreezeTime>? failedCallback)
	{
		string reqUrl = request.RequestUri!.ToString();
		if (FreezeUrl.TryGetValue(reqUrl, out var freezeTime) && freezeTime.EndTime > DateTime.Now)
		{
			string text = StringResource.Get("RequestUrlFailedDueToBeingFrozen")!;
			Log.Warn($"[HttpSend] {text}{reqUrl}");

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
			if (JsonSerializer.Deserialize<ApiStatus>(responseContent) is { Code: var code and >= 400 } err)
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

					// 11264 or 11265 means the URL has no permission to visit.
					freezeTime.EndTime = DateTime.Now + (code is 11264 or 11265 ? FreezeAddTime : freezeTime.AddTime);
				}
			}
		}

		freezeTime ??= new() { EndTime = DateTime.Now, AddTime = TimeSpan.FromSeconds(5) };

		failedCallback?.Invoke(response, freezeTime);

		return null;
	}
}
