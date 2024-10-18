namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Provides an easy way to control property value <see cref="PropertyAttribute.Setter"/>.
/// </summary>
/// <seealso cref="PropertyAttribute.Setter"/>
public static class PropertySetters
{
	/// <summary>
	/// Represents <see langword="init"/>.
	/// </summary>
	public const string Init = "init";

	/// <summary>
	/// Represents <see langword="set"/>.
	/// </summary>
	public const string Set = "set";

	/// <summary>
	/// Represents <see langword="internal set"/>.
	/// </summary>
	public const string InternalSet = "internal set";

	/// <summary>
	/// Represents <see langword="protected internal set"/>.
	/// </summary>
	public const string ProtectedInternalSet = "protected internal set";

	/// <summary>
	/// Represents <see langword="protected set"/>.
	/// </summary>
	public const string ProtectedSet = "protected set";

	/// <summary>
	/// Represents <see langword="private set"/>.
	/// </summary>
	public const string PrivateSet = "private set";
}
