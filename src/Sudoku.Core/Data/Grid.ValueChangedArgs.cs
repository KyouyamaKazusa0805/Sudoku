namespace Sudoku.Data;

partial struct Grid
{
	/// <summary>
	/// Provides arguments for the event <see cref="ValueChanged"/>.
	/// </summary>
	/// <seealso cref="ValueChanged"/>
	public readonly partial struct ValueChangedArgs : IValueEquatable<ValueChangedArgs>
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
		public ValueChangedArgs(int cell, short oldMask, short newMask, int setValue)
		{
			Cell = cell;
			OldMask = oldMask;
			NewMask = newMask;
			SetValue = setValue;
		}


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


		/// <summary>
		/// Deconstruct the instance.
		/// </summary>
		public void Deconstruct(out int cell, out short oldMask, out short newMask, out int setValue)
		{
			cell = Cell;
			oldMask = OldMask;
			newMask = NewMask;
			setValue = SetValue;
		}

		/// <inheritdoc cref="object.Equals(object?)"/>
		public override bool Equals(object? obj) => obj is ValueChangedArgs comparer && Equals(comparer);

		/// <inheritdoc cref="object.GetHashCode"/>
		public override int GetHashCode() => HashCode.Combine(Cell, OldMask, NewMask, SetValue);

		/// <inheritdoc/>
		public bool Equals(in ValueChangedArgs other) =>
			Cell == other.Cell && OldMask == other.OldMask && NewMask == other.NewMask
			&& SetValue == other.SetValue;


		/// <inheritdoc/>
		public static bool operator ==(in ValueChangedArgs left, in ValueChangedArgs right) => left.Equals(right);

		/// <inheritdoc/>
		public static bool operator !=(in ValueChangedArgs left, in ValueChangedArgs right) => !(left == right);
	}
}
