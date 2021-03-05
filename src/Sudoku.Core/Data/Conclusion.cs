using System;
using Sudoku.DocComments;

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a conclusion representation while solving in logic.
	/// </summary>
	public readonly struct Conclusion : IValueEquatable<Conclusion>, IValueComparable<Conclusion>, IComparable<Conclusion>
	{
		/// <summary>
		/// Initializes an instance with a conclusion type, a cell offset and a digit.
		/// </summary>
		/// <param name="conclusionType">The conclusion type.</param>
		/// <param name="cell">The cell offset.</param>
		/// <param name="digit">The digit.</param>
		public Conclusion(ConclusionType conclusionType, int cell, int digit)
		{
			ConclusionType = conclusionType;
			Cell = cell;
			Digit = digit;
		}

		/// <summary>
		/// Initializes an instance with a conclusion type and a candidate offset.
		/// </summary>
		/// <param name="conclusionType">The conclusion type.</param>
		/// <param name="candidate">The candidate offset.</param>
		public Conclusion(ConclusionType conclusionType, int candidate)
			: this(conclusionType, candidate / 9, candidate % 9)
		{
		}


		/// <summary>
		/// The cell offset.
		/// </summary>
		public int Cell { get; }

		/// <summary>
		/// The digit.
		/// </summary>
		public int Digit { get; }

		/// <summary>
		/// The conclusion type to control the action of applying.
		/// If the type is <see cref="ConclusionType.Assignment"/>,
		/// this conclusion will be set value (Set a digit into a cell);
		/// otherwise, a candidate will be removed.
		/// </summary>
		public ConclusionType ConclusionType { get; }


		/// <summary>
		/// Put this instance into the specified grid.
		/// </summary>
		/// <param name="grid">(<see langword="ref"/> parameter) The grid.</param>
		public void ApplyTo(ref SudokuGrid grid)
		{
			switch (ConclusionType)
			{
				case ConclusionType.Assignment:
				{
					grid[Cell] = Digit;
					break;
				}
				case ConclusionType.Elimination:
				{
					grid[Cell, Digit] = false;
					break;
				}
			}
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="conclusionType">(<see langword="out"/> parameter) The type of this conclusion.</param>
		/// <param name="candidate">(<see langword="out"/> parameter) The candidate.</param>
		public void Deconstruct(out ConclusionType conclusionType, out int candidate)
		{
			conclusionType = ConclusionType;
			candidate = Cell * 9 + Digit;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="conclusionType">(<see langword="out"/> parameter) The type of this conclusion.</param>
		/// <param name="cell">(<see langword="out"/> parameter) The cell.</param>
		/// <param name="digit">(<see langword="out"/> parameter) The digit.</param>
		public void Deconstruct(out ConclusionType conclusionType, out int cell, out int digit)
		{
			conclusionType = ConclusionType;
			cell = Cell;
			digit = Digit;
		}

		/// <inheritdoc/>
		public override bool Equals(object? obj) => obj is Conclusion comparer && Equals(comparer);

		/// <inheritdoc/>
		[CLSCompliant(false)]
		public bool Equals(in Conclusion other) => GetHashCode() == other.GetHashCode();

		/// <inheritdoc cref="object.GetHashCode"/>
		public override int GetHashCode() => ((int)ConclusionType + 1) * (Cell * 9 + Digit);

		/// <inheritdoc/>
		[CLSCompliant(false)]
		public int CompareTo(in Conclusion other) => GetHashCode() - other.GetHashCode();

		/// <inheritdoc cref="object.ToString"/>
		public override string ToString() =>
			$@"r{(Cell / 9 + 1).ToString()}c{(Cell % 9 + 1).ToString()} {ConclusionType switch
			{
				ConclusionType.Assignment => "=",
				ConclusionType.Elimination => "<>"
			}} {(Digit + 1).ToString()}";


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(in Conclusion left, in Conclusion right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(in Conclusion left, in Conclusion right) => !(left == right);
	}
}
