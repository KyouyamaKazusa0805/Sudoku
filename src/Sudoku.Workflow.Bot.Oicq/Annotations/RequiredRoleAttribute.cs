namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 提供一个特性，用于一个命令模块上，表示该命令模块可被触发的用户的角色。
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RequiredRoleAttribute : CommandAnnotationAttribute
{
	/// <summary>
	/// 表示全部用户均可的枚举结果常量。
	/// </summary>
	private const GroupRoleKind AllRoles = GroupRoleKind.God | GroupRoleKind.Owner | GroupRoleKind.Manager | GroupRoleKind.DefaultMember;


	/// <summary>
	/// 表示用户需要触发该指令的时候的角色权限列表。如果该枚举结果不包含用户对应的角色情况，就无法触发该指令。
	/// </summary>
	/// <remarks>
	/// <para>你可以使用 <see cref="GroupRoleKind"/>.<see langword="operator"/> | 运算符叠加多个权限角色的枚举数值。</para>
	/// <para><i>默认情况下，所有角色都可以触发该指令。</i></para>
	/// </remarks>
	public GroupRoleKind SenderRole { get; init; } = AllRoles;

	/// <summary>
	/// 表示机器人自身触发该指令的时候的角色权限列表。如果该枚举结果，机器人不满足，该指令也无法触发。
	/// 这个属性主要用于预先设定机器人在触发之前需要满足的角色权限，比如只有群主可以改头衔之类的。
	/// </summary>
	/// <remarks><i>
	/// 默认情况下，该属性为 <see cref="GroupRoleKind.None"/>，即无需权限即可使用。
	/// </i></remarks>
	/// <seealso cref="GroupRoleKind.None"/>
	public GroupRoleKind BotRole { get; init; }
}
