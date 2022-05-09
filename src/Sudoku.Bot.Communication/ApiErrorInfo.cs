namespace Sudoku.Bot.Communication;

/// <summary>
/// Indicates the important info data on error encountered in APIs.
/// </summary>
public sealed class ApiErrorInfo
{
	/// <summary>
	/// Initializes an <see cref="ApiErrorInfo"/> instance via the specified data.
	/// </summary>
	/// <param name="path">The URL link that makes the request on API.</param>
	/// <param name="method">The request method.</param>
	/// <param name="code">The error code.</param>
	/// <param name="detail">The details for the error.</param>
	/// <param name="freezeTime">The freeze time instance.</param>
	public ApiErrorInfo(string path, string method, int code, string detail, FreezeTime freezeTime)
		=> (Path, Method, Code, Detail, FreezeTime) = (path, method, code, detail, freezeTime);


	/// <summary>
	/// The URL link that makes the request on API.
	/// </summary>
	public string Path { get; init; }

	/// <summary>
	/// The request method.
	/// </summary>
	public string Method { get; init; }

	/// <summary>
	/// The error code.
	/// </summary>
	public int Code { get; set; }

	/// <summary>
	/// The details for the error.
	/// </summary>
	public string Detail { get; set; }

	/// <summary>
	/// The freeze time instance.
	/// </summary>
	public FreezeTime FreezeTime { get; init; }
}
