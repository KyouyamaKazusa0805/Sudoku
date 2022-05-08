namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 系统默认身份组
/// </summary>
public static class DefaultRoles
{
	/// <summary>
	/// 获取系统默认身份组名称
	/// </summary>
	/// <param name="roleId"></param>
	/// <returns></returns>
	public static string? Get(string roleId)
		=> roleId switch
		{
			"1" => "普通成员",
			"2" => "管理员",
			"4" => "频道主",
			"5" => "子频道管理员",
			_ => null
		};
}
