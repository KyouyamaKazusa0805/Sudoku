using System;
using Sudoku.DocComments;

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
		/// <param name="cell">The cell offset. Must be in range 0 to 80.</param>
		/// <param name="oldMask">The old mask before modified.</param>
		/// <param name="newMask">The mask to modify the cell.</param>
		/// <param name="setValue">
		/// The value to set on the cell. If the action is deletion,
		/// this argument should be -1.
		/// </param>
		public ValueChangedEventArgs(int cell, short oldMask, short newMask, int setValue) =>
			(Cell, OldMask, NewMask, SetValue) = (cell, oldMask, newMask, setValue);


		/// <summary>
		/// The cell offset. Must be in range 0 to 80.
		/// </summary>
		public int Cell { get; }

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


		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="cell">(<see langword="out"/> parameter) The cell offset.</param>
		/// <param name="oldMask">(<see langword="out"/> parameter) The old mask.</param>
		/// <param name="newMask">(<see langword="out"/> parameter) The new mask.</param>
		/// <param name="setValue">(<see langword="out"/> parameter) the set value.</param>
		public void Deconstruct(out int cell, out short oldMask, out short newMask, out int setValue) =>
			(cell, oldMask, newMask, setValue) = (Cell, OldMask, NewMask, SetValue);
	}
}
