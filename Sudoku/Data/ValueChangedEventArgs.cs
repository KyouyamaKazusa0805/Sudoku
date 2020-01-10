using System;
using Sudoku.Diagnostics.CodeAnalysis;

namespace Sudoku.Data
{
	/// <summary>
	/// Provides the data for the event 'ValueChanged'.
	/// </summary>
	public sealed class ValueChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes the instance with some data.
		/// </summary>
		/// <param name="cellOffset">The cell offset. Must be in range 0 to 80.</param>
		/// <param name="oldMask">The old mask before modified.</param>
		/// <param name="newMask">The mask to modify the cell.</param>
		/// <param name="setValue">
		/// The value to set on the cell. If the action is deletion,
		/// this argument should be -1.
		/// </param>
		public ValueChangedEventArgs(int cellOffset, short oldMask, short newMask, int setValue) =>
			(CellOffset, OldMask, NewMask, SetValue) = (cellOffset, oldMask, newMask, setValue);


		/// <summary>
		/// The cell offset. Must be in range 0 to 80.
		/// </summary>
		public int CellOffset { get; }

		/// <summary>
		/// The old mask before modified.
		/// </summary>
		public short OldMask { get; }

		/// <summary>
		/// The new mask after modified.
		/// </summary>
		public short NewMask { get; }

		/// <summary>
		/// The value. -1 when this value is not required.
		/// </summary>
		public int SetValue { get; }


		/// <summary>
		/// Deconstruct the instance to four values.
		/// </summary>
		/// <param name="cellOffset">(out parameter) The cell offset.</param>
		/// <param name="oldMask">(out parameter) The old mask.</param>
		/// <param name="newMask">(out parameter) The new mask.</param>
		/// <param name="setValue">(out parameter) the set value.</param>
		[OnDeconstruction]
		public void Deconstruct(
			out int cellOffset, out short oldMask, out short newMask, out int setValue) =>
			(cellOffset, oldMask, newMask, setValue) = (CellOffset, OldMask, NewMask, SetValue);
	}
}
