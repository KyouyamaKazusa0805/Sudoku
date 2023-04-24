namespace Sudoku.Workflow.Bot.Oicq.ComponentModel;

/// <summary>
/// 表示一个 QQ 群里用户的所有可能角色情况。
/// </summary>
[Flags]
public enum GroupRoleKind
{
	/// <summary>
	/// 为该枚举的默认数值。这个数据一般不会被用在代码里，主要是用来判断和比较结果是否等于它自身。
	/// </summary>
	None = 0,

	/// <summary>
	/// 为群里最大权限的用户，指代的是这个群里的机器人的作者本人，属于是上帝账号。
	/// </summary>
	God = 1,

	/// <summary>
	/// 为群里的群主。
	/// </summary>
	Owner = 1 << 1,

	/// <summary>
	/// 为群里的管理员。
	/// </summary>
	Manager = 1 << 2,

	/// <summary>
	/// 为群里的普通成员。
	/// </summary>
	DefaultMember = 1 << 3
}
