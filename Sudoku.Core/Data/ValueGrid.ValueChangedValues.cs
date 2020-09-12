using System.Diagnostics.CodeAnalysis;

namespace Sudoku.Data
{
	public unsafe partial struct ValueGrid
	{
		/// <summary>
		/// Encapsulates the arguments for the event value changed.
		/// </summary>
		[SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "<Pending>")]
		[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "<Pending>")]
		public /*readonly*/ struct ValueChangedValues
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
			public ValueChangedValues(int cellOffset, short oldMask, short newMask, int setValue) =>
				(CellOffset, OldMask, NewMask, SetValue) = (cellOffset, oldMask, newMask, setValue);


			/// <summary>
			/// The cell offset. Must be in range 0 to 80.
			/// </summary>
			public int CellOffset { get; init; }

			/// <summary>
			/// The old mask before modified.
			/// </summary>
			public short OldMask { get; init; }

			/// <summary>
			/// The new mask after modified.
			/// </summary>
			public short NewMask { get; init; }

			/// <summary>
			/// The value. -1 when this value is not required.
			/// </summary>
			public int SetValue { get; init; }


			/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
			/// <param name="cellOffset">(<see langword="out"/> parameter) The cell offset.</param>
			/// <param name="oldMask">(<see langword="out"/> parameter) The old mask.</param>
			/// <param name="newMask">(<see langword="out"/> parameter) The new mask.</param>
			/// <param name="setValue">(<see langword="out"/> parameter) the set value.</param>
			public void Deconstruct(out int cellOffset, out short oldMask, out short newMask, out int setValue) =>
				(cellOffset, oldMask, newMask, setValue) = (CellOffset, OldMask, NewMask, SetValue);
		}
	}
}
