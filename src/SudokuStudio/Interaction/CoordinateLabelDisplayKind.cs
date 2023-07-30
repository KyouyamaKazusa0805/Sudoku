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
	/// Indicates the coordinate labels will be displayed as <see cref="CellNotationKind.RxCy">RxCy Notation</see>.
	/// </summary>
	/// <seealso cref="CellNotationKind.RxCy"/>
	RxCy,

	/// <summary>
	/// Indicates the coordinate labels will be displayed as <see cref="CellNotationKind.K9">K9 Notation</see>.
	/// </summary>
	/// <seealso cref="CellNotationKind.K9"/>
	K9
}
