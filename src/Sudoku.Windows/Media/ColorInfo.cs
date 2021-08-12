using System.Windows.Media;

namespace Sudoku.Windows.Media;

/// <summary>
/// The color information, which contains the color instance and its string representation of a hex value.
/// </summary>
public sealed class ColorInfo
{
	/// <summary>
	/// Indicates the color.
	/// </summary>
	public Color Color { get; set; }

	/// <summary>
	/// Indicates the hex string.
	/// </summary>
	public string? HexString { get; set; }
}
