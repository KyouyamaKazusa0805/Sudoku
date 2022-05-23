namespace Sudoku.Bot.Communication;

/// <summary>
/// Indicates the handler that can filter some requests.
/// </summary>
public sealed class HttpLoggingHandler : DelegatingHandler
{
	/// <summary>
	/// Indicates the print buffer length.
	/// </summary>
	private const int PrintLength = 1024;


	/// <summary>
	/// Initializes a <see cref="HttpLoggingHandler"/> instance via the specified <see cref="HttpMessageHandler"/>
	/// instance as the inner handler.
	/// </summary>
	/// <param name="innerHandler">The <see cref="HttpMessageHandler"/> instance.</param>
	public HttpLoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler)
	{
	}


	/// <inheritdoc/>
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		string newline = Environment.NewLine;

		static string evaluator(Match m) => Regex.Replace(m.Groups[0].Value, """[^\.]""", "*");
		string requestString = Regex.Replace(request.ToString(), """(?<=Bot\s+)[^\n]+""", evaluator);
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
			requestContent = string.IsNullOrWhiteSpace(requestContent)
				? StringResource.Get("NoContent")!
				: StringResource.Get("ContentCannotBeDecoded")!;
		}

		requestContent = $"[HttpHandler][Request]{newline}{requestString}{newline}{requestContent}";

		var response = await base.SendAsync(request, cancellationToken);
		if (cancellationToken.IsCancellationRequested)
		{
			string requestIsCancelled = StringResource.Get("RequestCancelled")!;
			Logging.Error($"{requestContent}\n{requestIsCancelled}");
			return response;
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
			responseContent = string.IsNullOrWhiteSpace(responseContent)
				? StringResource.Get("NoContent")!
				: StringResource.Get("ContentCannotBeDecoded")!;
		}

		responseContent = $"[HttpHandler][Response]{newline}{responseString}{newline}{responseContent}{newline}";

		var debug = Logging.Debug;
		var error = Logging.Error;
		(responseStatusCode < HttpStatusCode.BadRequest ? debug : error)($"{requestContent}\n{responseContent}");

		return response;
	}
}
