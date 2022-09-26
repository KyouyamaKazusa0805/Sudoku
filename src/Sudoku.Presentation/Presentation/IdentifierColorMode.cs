namespace Sudoku.Presentation;

/// <summary>
/// Indicates a mode to display a color used by type <see cref="Identifier"/>.
/// </summary>
/// <seealso cref="Identifier"/>
public enum IdentifierColorMode
{
	/// <summary>
	/// Indicates the raw mode. The mode uses alpha, red, green and blue to represent a color.
	/// </summary>
	Raw,

	/// <summary>
	/// Indicates the ID mode. The mode uses an ID to represent a color.
	/// </summary>
	Id,

	/// <summary>
	/// Indicates the named mode. The mode uses an enumeration field name to differ color.
	/// </summary>
	Named
}
