namespace Sudoku.Concepts;

/// <summary>
/// Defines a type that is triggered when the specified value in a grid has been changed.
/// </summary>
[DisallowParameterlessConstructor]
public readonly ref partial struct GridValueChangedEventArgs
{
	/// <summary>
	/// The backing field of property <see cref="GridRef"/>.
	/// </summary>
	/// <seealso cref="GridRef"/>
	private readonly ref readonly Grid _gridRef;


	/// <summary>
	/// Initializes a <see cref="GridValueChangedEventArgs"/> instance via the specified grid to be updated,
	/// the cell, old and new mask information to be updated.
	/// </summary>
	/// <param name="gridRef">The reference to the grid.</param>
	/// <param name="appliedDigit">The applied digit. -1 is for empty.</param>
	/// <param name="cell">The cell used.</param>
	/// <param name="oldMaskValue">The old and original mask value in the grid.</param>
	/// <param name="newMaskValue">The new value to be replaced with.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal GridValueChangedEventArgs(in Grid gridRef, int cell, int appliedDigit, short oldMaskValue, short newMaskValue)
	{
		_gridRef = ref gridRef;
		(Cell, SetDigit, OldMaskValue, NewMaskValue) = (cell, appliedDigit, oldMaskValue, newMaskValue);
	}


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
