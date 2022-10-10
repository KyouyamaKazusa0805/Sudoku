namespace Microsoft.CodeAnalysis;

/// <summary>
/// Defines a C# accessibility.
/// </summary>
public enum CSharpAccessibility
{
	/// <summary>
	/// The accessibility is not applicable.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	None,

	/// <summary>
	/// The <see langword="file"/> accessibility.
	/// </summary>
	File,

	/// <summary>
	/// The <see langword="private"/> accessibility.
	/// </summary>
	Private,

	/// <summary>
	/// The <see langword="protected"/> accessibility.
	/// </summary>
	Protected,

	/// <summary>
	/// The <see langword="private protected"/> accessibility.
	/// </summary>
	PrivateProtected,

	/// <summary>
	/// The <see langword="internal"/> accessibility.
	/// </summary>
	Internal,

	/// <summary>
	/// The <see langword="protected internal"/> accessibility.
	/// </summary>
	ProtectedInternal,

	/// <summary>
	/// The <see langword="public"/> accessibility.
	/// </summary>
	Public
}
