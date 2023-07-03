namespace System.SourceGeneration;

/// <summary>
/// Defines a kind of accessibility being defined in C#.
/// </summary>
public enum Accessibility
{
	/// <summary>
	/// Indicates the accessibility is invalid and not defined in this type.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	None,

	/// <summary>
	/// Indicates the accessibility is <see langword="file"/>-local, which means it can be accessed only in a whole file.
	/// </summary>
	File,

	/// <summary>
	/// Indicates the accessibility is <see langword="private"/>.
	/// </summary>
	Private,

	/// <summary>
	/// Indicates the accessibility is <see langword="protected"/>.
	/// </summary>
	Protected,

	/// <summary>
	/// Indicates the accessibility is <see langword="private protected"/>.
	/// </summary>
	PrivateProtected,

	/// <summary>
	/// Indicates the accessibility is <see langword="internal"/>.
	/// </summary>
	Internal,

	/// <summary>
	/// Indicates the accessibility is <see langword="protected internal"/>.
	/// </summary>
	ProtectedInternal,

	/// <summary>
	/// Indicates the accessibility is <see langword="public"/>.
	/// </summary>
	Public
}
