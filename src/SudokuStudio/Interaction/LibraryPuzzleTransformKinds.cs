namespace SudokuStudio.Interaction;

/// <summary>
/// Represents a list of adjustable kinds that describes which way the puzzle can be transformed.
/// </summary>
[Flags]
public enum LibraryPuzzleTransformKinds
{
	/// <summary>
	/// Indicates no transform.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates digit transform.
	/// </summary>
	Digit = 1,

	/// <summary>
	/// Indicates row-swap transform.
	/// </summary>
	RowSwap = 1 << 1,

	/// <summary>
	/// Indicates column-swap transform.
	/// </summary>
	ColumnSwap = 1 << 2,

	/// <summary>
	/// Indicates band-swap transform.
	/// </summary>
	BandSwap = 1 << 3,

	/// <summary>
	/// Indicates tower-swap transform.
	/// </summary>
	TowerSwap = 1 << 4,

	/// <summary>
	/// Indicates mirror left-right transform.
	/// </summary>
	MirrowLeftRight = 1 << 5,

	/// <summary>
	/// Indicates mirror top-bottom transform.
	/// </summary>
	MirrorTopBottom = 1 << 6,

	/// <summary>
	/// Indicates mirror diagonal transform.
	/// </summary>
	MirrorDiagonal = 1 << 7,

	/// <summary>
	/// Indicates mirror anti-diagonal transform.
	/// </summary>
	MirrorAntidigaonal = 1 << 8,

	/// <summary>
	/// Indicates rotate clockwise transform.
	/// </summary>
	RotateClockwise = 1 << 9,

	/// <summary>
	/// Indicates rotate counterclockwise transform.
	/// </summary>
	RotateCounterclockwise = 1 << 10
}
