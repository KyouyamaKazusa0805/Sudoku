namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Declares the command and its permission.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class CommandAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="CommandAttribute"/> instance.
	/// </summary>
	public CommandAttribute() : this(Permissions.Member, Permissions.Administrator, Permissions.Owner)
	{
	}

	/// <summary>
	/// Initializes a <see cref="CommandAttribute"/> instance.
	/// </summary>
	/// <param name="permission">The permission.</param>
	public CommandAttribute(params Permissions[] permission) => AllowedPermissions = permission;


	/// <summary>
	/// Indicates whether the command is deprecated. If so, the command will not be used by users,
	/// and runtime will automatically skip the command.
	/// </summary>
	public bool IsDeprecated { get; init; }

	/// <summary>
	/// Indicates the permission.
	/// </summary>
	public Permissions[] AllowedPermissions { get; }
}
