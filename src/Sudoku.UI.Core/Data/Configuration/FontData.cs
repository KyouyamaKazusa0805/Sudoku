namespace Sudoku.UI.Data.Configuration;

/// <summary>
/// Defines a font data.
/// </summary>
public sealed class FontData
{
	/// <summary>
	/// Indicates the font name.
	/// </summary>
	public required string? FontName { get; set; }

	/// <summary>
	/// Indicates the font scale.
	/// </summary>
	public required double FontScale { get; set; }
}
