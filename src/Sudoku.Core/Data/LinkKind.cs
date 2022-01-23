namespace Sudoku.Data;

/// <summary>
/// Defines a link kind.
/// </summary>
public enum LinkKind : byte
{
	/// <summary>
	/// Indicates the default-kind link.
	/// </summary>
	Default = 0,

	/// <summary>
	/// Indicates the weak link.
	/// </summary>
	Weak,

	/// <summary>
	/// Indicates the strong link.
	/// </summary>
	Strong,

	/// <summary>
	/// Indicates the conjugate pair.
	/// </summary>
	ConjugatePair,

	/// <summary>
	/// Indicates the line-kind link.
	/// </summary>
	Line
}
