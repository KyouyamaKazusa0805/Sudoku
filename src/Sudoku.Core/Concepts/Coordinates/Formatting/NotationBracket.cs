namespace Sudoku.Concepts.Coordinates.Formatting;

/// <summary>
/// Represents a kind of notation bracket that will be used on formatting combined coordinates.
/// </summary>
public enum NotationBracket
{
	/// <summary>
	/// Indicates there's no bracket that combines multiple parts, like <c>r2c2, r3c3</c>.
	/// </summary>
	None,

	/// <summary>
	/// Indicates there's a round bracket <c>()</c> that combines multiple parts, like <c>(r2c2, r3c3)</c>.
	/// </summary>
	Round,

	/// <summary>
	/// Indicates there's a square bracket <c>[]</c> that combines multiple parts, like <c>[r2c2, r3c3]</c>.
	/// </summary>
	Square,

	/// <summary>
	/// Indicates there's a curly bracket <c>{}</c> that combines multiple parts, like <c>{r2c2, r3c3}</c>.
	/// </summary>
	Curly,

	/// <summary>
	/// Indicates there's an angle bracket <c><![CDATA[<>]]></c> that combines multiple parts, like <c><![CDATA[<r2c2, r2c3>]]></c>.
	/// </summary>
	Angle
}
