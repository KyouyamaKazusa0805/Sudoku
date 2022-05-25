namespace Sudoku.Runtime.Serialization;

/// <summary>
/// The internal color structure.
/// </summary>
internal readonly struct ColorInternal
{
	/// <summary>
	/// The alpha value.
	/// </summary>
	public byte A { get; init; }

	/// <summary>
	/// The red value.
	/// </summary>
	public byte R { get; init; }

	/// <summary>
	/// The green value.
	/// </summary>
	public byte G { get; init; }

	/// <summary>
	/// The blue value.
	/// </summary>
	public byte B { get; init; }
}
