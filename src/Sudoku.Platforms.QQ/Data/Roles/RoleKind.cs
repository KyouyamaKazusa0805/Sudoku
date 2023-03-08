namespace Sudoku.Platforms.QQ.Data.Roles;

/// <summary>
/// Defines a role kind in a group. The value provides with a kind of role who emits and raises a module.
/// </summary>
[Flags]
public enum GroupRoleKind : int
{
	/// <summary>
	/// Indicates the default value. This value cannot be used in code, and it only offers a default value to be compared with.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the role is the god account, which means a person who writes code and creates modules to be used.
	/// This value is nearly equal to the author of this project.
	/// </summary>
	God = 1,

	/// <summary>
	/// Indicates the role is an owner who owns a group. In Chinese, "qún zhŭ".
	/// </summary>
	Owner = 1 << 1,

	/// <summary>
	/// Indicates the role is a manager who can do some works to collaborate with group owner. In Chinese, "guǎn lǐ yuán".
	/// </summary>
	Manager = 1 << 2,

	/// <summary>
	/// Indicates the role is a default member.
	/// </summary>
	DefaultMember = 1 << 3
}
