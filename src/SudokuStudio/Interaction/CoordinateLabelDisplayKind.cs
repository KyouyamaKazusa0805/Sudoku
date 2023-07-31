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
	/// Indicates the coordinate labels will be displayed as <see cref="CellNotation.Kind.RxCy">RxCy Notation</see>.
	/// </summary>
	/// <seealso cref="CellNotation.Kind.RxCy"/>
	RxCy,

	/// <summary>
	/// Indicates the coordinate labels will be displayed as <see cref="CellNotation.Kind.K9">K9 Notation</see>.
	/// </summary>
	/// <seealso cref="CellNotation.Kind.K9"/>
	K9
}
