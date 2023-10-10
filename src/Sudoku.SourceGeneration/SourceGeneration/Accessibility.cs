namespace Sudoku.SourceGeneration;

/// <summary>
/// Defines an accessibility kind.
/// </summary>
internal enum Accessibility
{
	None,

	/// <summary>
	/// Indicates the accessibility is <see langword="file"/>-scoped.
	/// </summary>
	[Name("file")]
	File,

	/// <summary>
	/// Indicates the accessibility is <see langword="private"/>.
	/// </summary>
	[Name("private")]
	Private,

	/// <summary>
	/// Indicates the accessibility is <see langword="protected"/>.
	/// </summary>
	[Name("protected")]
	Protected,

	/// <summary>
	/// Indicates the accessibility is <see langword="private protected"/>.
	/// </summary>
	[Name("private protected")]
	PrivateProtected,

	/// <summary>
	/// Indicates the accessibility is <see langword="internal"/>.
	/// </summary>
	[Name("internal")]
	Internal,

	/// <summary>
	/// Indicates the accessibility is <see langword="protected internal"/>.
	/// </summary>
	[Name("protected internal")]
	ProtectedInternal,

	/// <summary>
	/// Indicates the accessibility is <see langword="public"/>.
	/// </summary>
	[Name("public")]
	Public
}
