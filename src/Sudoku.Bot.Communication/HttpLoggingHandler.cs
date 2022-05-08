namespace Sudoku.Bot.Communication;

/// <summary>
/// HttpClient请求拦截器
/// </summary>
public sealed class HttpLoggingHandler : DelegatingHandler
{
	/// <summary>
	/// Indicates the print buffer length.
	/// </summary>
	private const int PrintLength = 1024;


	/// <summary>
	/// HttpClient请求拦截器构造函数
	/// </summary>
	/// <param name="innerHandler"></param>
	public HttpLoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler)
	{
	}


	/// <summary>
	/// 发起异步Http请求
	/// </summary>
	/// <param name="request"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	protected override async Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request, CancellationToken cancellationToken)
	{
		string newline = Environment.NewLine;

		static string evaluator(Match m) => Regex.Replace(m.Groups[0].Value, """[^\.]""", "*");
		string requestString = Regex.Replace(request.ToString(), """(?<=Bot\s+)[^\n]+""", evaluator); // 敏感信息脱敏
		string requestContent = request.Content is null
			? string.Empty
			: await request.Content.ReadAsStringAsync(CancellationToken.None);

		var requestContentType = request.Content?.Headers.ContentType;
		if (requestContent.Length > PrintLength)
		{
			requestContent = requestContent[..PrintLength];
		}

		if (requestContentType?.CharSet is null && requestContentType?.MediaType != "application/json")
		{
			requestContent = string.IsNullOrWhiteSpace(requestContent) ? "（没有内容）" : "（内容无法解码）";
		}

		requestContent = $"[HttpHandler][Request]{newline}{requestString}{newline}{requestContent}";

		var response = await base.SendAsync(request, cancellationToken);
		if (cancellationToken.IsCancellationRequested)
		{
			Log.Error($"{requestContent}\n请求已取消！");
			return response; // 请求已取消
		}

		string responseString = response.ToString();
		string responseContent = response.Content is null
			? string.Empty
			: await response.Content.ReadAsStringAsync(CancellationToken.None);

		var responseStatusCode = response.StatusCode;
		var responseContentType = response.Content?.Headers.ContentType;
		if (responseContent.Length > PrintLength)
		{
			responseContent = responseContent[..PrintLength];
		}

		if (responseContentType?.CharSet is null && responseContentType?.MediaType != "application/json")
		{
			responseContent = string.IsNullOrWhiteSpace(responseContent) ? "（没有内容）" : "（内容无法解码）";
		}

		responseContent = $"[HttpHandler][Response]{newline}{responseString}{newline}{responseContent}{newline}";

		if (responseStatusCode < HttpStatusCode.BadRequest)
		{
			Log.Debug(requestContent + '\n' + responseContent);
		}
		else
		{
			Log.Error(requestContent + '\n' + responseContent);
		}

		return response;
	}
}
