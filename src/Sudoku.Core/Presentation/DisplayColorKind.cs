namespace Sudoku.Presentation;

/// <summary>
/// Defines a display color kind.
/// </summary>
public enum DisplayColorKind : byte
{
	/// <summary>
	/// Indicates the normal color.
	/// </summary>
	Normal,

	/// <summary>
	/// Indicates the elimination color.
	/// </summary>
	Elimination,

	/// <summary>
	/// Indicates the exo-fin color.
	/// </summary>
	Exofin,

	/// <summary>
	/// Indicates the endo-fin color.
	/// </summary>
	Endofin,

	/// <summary>
	/// Indicates the cannibalism color.
	/// </summary>
	Cannibalism,

	/// <summary>
	/// Indicates the link color.
	/// </summary>
	Link
}
