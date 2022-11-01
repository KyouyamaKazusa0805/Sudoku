namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Declares the command and its permission.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal sealed class CommandAttribute : Attribute
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
	/// Indicates the permission.
	/// </summary>
	public Permissions[] AllowedPermissions { get; }
}
