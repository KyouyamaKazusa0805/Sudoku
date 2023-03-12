namespace Sudoku.Platforms.QQ.Data.Parsing;

/// <summary>
/// Indicates the required role attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RequiredRoleAttribute : CommandLineParsingItemAttribute
{
	/// <summary>
	/// Indicates all roles are included.
	/// </summary>
	private const GroupRoleKind AllRoles = GroupRoleKind.God | GroupRoleKind.Owner | GroupRoleKind.Manager | GroupRoleKind.DefaultMember;


	/// <summary>
	/// Indicates the supported sender roles.
	/// The role is used for checking whether the module should be executed if a person emits a command to execute this module.
	/// </summary>
	/// <remarks>
	/// <para>You can use <see cref="GroupRoleKind"/>.<see langword="operator"/> | to merge multiple role kinds into one.</para>
	/// <para><i>By default, the value is all possible roles included.</i></para>
	/// </remarks>
	public GroupRoleKind SenderRole { get; init; } = AllRoles;

	/// <summary>
	/// Indicates the required bot permission. The kind is only set with one flag, as the highest allowed permission.
	/// </summary>
	/// <remarks><i>
	/// By default, the value is <see cref="GroupRoleKind.None"/>, which means the operation does not require any higher permissions.
	/// </i></remarks>
	/// <seealso cref="GroupRoleKind.None"/>
	public GroupRoleKind BotRole { get; init; }
}
