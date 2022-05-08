namespace Sudoku.Bot.Communication;

/// <summary>
/// API请求出错的关键信息
/// </summary>
public class ApiErrorInfo
{
	/// <summary>
	/// API请求出错的关键信息
	/// </summary>
	/// <param name="path">接口地址</param>
	/// <param name="method">请求方式</param>
	/// <param name="code">错误代码</param>
	/// <param name="detail">错误详情</param>
	/// <param name="freezeTime">接口被暂时停用的时间</param>
	public ApiErrorInfo(string path, string method, int code, string detail, FreezeTime freezeTime)
	{
		Path = path;
		Method = method;
		Code = code;
		Detail = detail;
		FreezeTime = freezeTime;
	}


	/// <summary>
	/// 接口地址
	/// </summary>
	public string Path { get; init; }

	/// <summary>
	/// 请求方式
	/// </summary>
	public string Method { get; init; }

	/// <summary>
	/// 错误代码
	/// </summary>
	public int Code { get; set; }

	/// <summary>
	/// 错误信息
	/// </summary>
	public string Detail { get; set; }

	/// <summary>
	/// 接口被暂时停用的时间
	/// </summary>
	public FreezeTime FreezeTime { get; init; }
}
