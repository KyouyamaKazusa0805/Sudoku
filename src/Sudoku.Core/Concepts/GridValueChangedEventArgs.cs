namespace Sudoku.Concepts;

partial struct GridValueChangedEventArgs
{
	/// <summary>
	/// The backing field of property <see cref="GridRef"/>.
	/// </summary>
	/// <seealso cref="GridRef"/>
	private readonly ref readonly Grid _gridRef;


	/// <summary>
	/// Indicates the cell to be updated and changed its value.
	/// </summary>
	public int Cell { get; }

	/// <summary>
	/// Indicates applied digit. -1 is for clear the cell, changed its status to <see cref="CellStatus.Empty"/>.
	/// </summary>
	/// <seealso cref="CellStatus"/>
	public int SetDigit { get; }

	/// <summary>
	/// Indicates the mask value that is the old and original value in the current grid.
	/// </summary>
	public short OldMaskValue { get; }

	/// <summary>
	/// Indicates the mask value as replacer to update the old value.
	/// </summary>
	public short NewMaskValue { get; }

	/// <summary>
	/// Indicates the reference to the grid whose inner value has been changed.
	/// </summary>
	public ref readonly Grid GridRef => ref _gridRef;
}
