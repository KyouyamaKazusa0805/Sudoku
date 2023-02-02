namespace SudokuStudio.Interaction;

/// <summary>
/// Defines a kind of coordinate label to be displayed.
/// </summary>
public enum CoordinateLabelDisplayKind
{
	/// <summary>
	/// Indicates the coordinate labels will not be displayed.
	/// </summary>
	None,

	/// <summary>
	/// Indicates the coordinate labels will be displayed as <see cref="RxCyNotation">RxCy Notation</see>.
	/// </summary>
	/// <seealso cref="RxCyNotation"/>
	RxCy,

	/// <summary>
	/// Indicates the coordinate labels will be displayed as <see cref="K9Notation">K9 Notation</see>.
	/// </summary>
	/// <seealso cref="K9Notation"/>
	K9
}
