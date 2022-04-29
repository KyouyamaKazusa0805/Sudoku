namespace Sudoku.Bot.Oicq.Concepts.Helpers;

/// <summary>
/// Defines an HTTP protocol helper type for get/post operations.
/// </summary>
public sealed class HttpGetPost
{
	/// <summary>
	/// Gets the data via the specified URL. If failed, an empty string will be returned.
	/// </summary>
	/// <param name="url">The URL.</param>
	/// <returns>The string data.</returns>
	public static string Get(string url)
	{
		using var client = new HttpClient();
		try
		{
			return client.GetStringAsync(url).Result;
		}
		catch
		{
			return string.Empty;
		}
	}

	/// <summary>
	/// Gets the raw string value via the specified URL and the values as the options.
	/// If failed, an empty string will be returned.
	/// </summary>
	/// <param name="url">The URL.</param>
	/// <param name="values">The values as the options.</param>
	/// <returns>The string data.</returns>
	public static string Get(string url, IReadOnlyDictionary<string, string> values)
	{
		using var client = new HttpClient();
		try
		{
			string querystr =
#if NETSTANDARD
				string.Join("&", from kvp in values select $"{kvp.Key}={kvp.Value}");
#else
				string.Join('&', from kvp in values select $"{kvp.Key}={kvp.Value}");
#endif

			return client.GetStringAsync($"{url}?{querystr}").Result;
		}
		catch
		{
			return string.Empty;
		}
	}

	/// <summary>
	/// Posts the content.
	/// </summary>
	/// <param name="url">The URL.</param>
	/// <param name="values">The values.</param>
	/// <returns>The string result.</returns>
	public static string Post(string url, IReadOnlyCollection<KeyValuePair<string, string>> values)
	{
		using var client = new HttpClient();

		var content = new FormUrlEncodedContent(values);
		var response = client.PostAsync(url, content);
		var responseString = response.Result.Content.ReadAsStringAsync();
		return responseString.Result;
	}

	/// <summary>
	/// Posts the data as the JSON representation.
	/// </summary>
	/// <param name="url">The URL.</param>
	/// <param name="body">The body.</param>
	/// <returns>The string result.</returns>
	public static string Post(string url, string body)
	{
		using var client = new HttpClient();

		var content = new StringContent(body);
		content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
		var response = client.PostAsync(url, content);
		var responseString = response.Result.Content.ReadAsStringAsync();
		return responseString.Result;
	}
}
