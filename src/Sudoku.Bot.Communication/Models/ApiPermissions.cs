namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 接口权限列表对象
/// </summary>
public class ApiPermissions
{
	/// <summary>
	/// 接口权限列表
	/// </summary>
	[JsonPropertyName("apis")]
	public List<ApiPermission>? List { get; set; }
}
