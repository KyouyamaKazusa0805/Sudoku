namespace Sudoku.Data;

partial struct SudokuGrid
{
	/// <summary>
	/// Indicates the item that <see cref="GetPinnableReference()"/> selects and returns the reference.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This enumeration type is called to select what the inner field we want to fix. For example,
	/// if called <see cref="GetPinnableReference()"/> (parameterless method), the default value is
	/// <see cref="CurrentGrid"/>, and we'll get the table that contains all current statuses of the
	/// current grid using a <see cref="short"/>*. The code should be:
	/// <code>
	/// fixed (short* s = grid)
	/// {
	///     // Do something...
	/// }
	/// </code>
	/// The code is equivalent to
	/// <code><![CDATA[
	/// fixed (short* s = &grid.GetPinnableReference())
	/// {
	///     // Do something...
	/// }
	/// ]]></code>
	/// </para>
	/// <para>
	/// If you want to get the item <see cref="InitialGrid"/>, you should call the method
	/// <see cref="GetPinnableReference(PinnedItem)"/>.
	/// </para>
	/// </remarks>
	/// <see cref="GetPinnableReference()"/>
	/// <see cref="GetPinnableReference(PinnedItem)"/>
	[Closed]
	public enum PinnedItem : byte
	{
		/// <summary>
		/// Indicates the current grid should be pinned.
		/// </summary>
		CurrentGrid,

		/// <summary>
		/// Indicates the initial grid should be pinned.
		/// </summary>
		InitialGrid
	}
}
