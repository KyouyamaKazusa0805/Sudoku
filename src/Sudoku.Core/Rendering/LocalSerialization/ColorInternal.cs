namespace Sudoku.Rendering.LocalSerialization;

/// <summary>
/// The internal color structure.
/// </summary>
internal struct ColorInternal
{
	/// <summary>
	/// The alpha value.
	/// </summary>
	public byte A { get; set; }

	/// <summary>
	/// The red value.
	/// </summary>
	public byte R { get; set; }

	/// <summary>
	/// The green value.
	/// </summary>
	public byte G { get; set; }

	/// <summary>
	/// The blue value.
	/// </summary>
	public byte B { get; set; }
}
