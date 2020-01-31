using System;
using Sudoku.Data.Meta;

namespace Sudoku.Solving
{
	/// <summary>
	/// Encapsulates a conclusion representation while solving in logic.
	/// </summary>
	public readonly struct Conclusion : IEquatable<Conclusion>
	{
		/// <summary>
		/// Initializes an instance with a conclusion type, a cell offset and a digit.
		/// </summary>
		/// <param name="conclusionType">The conclusion type.</param>
		/// <param name="cellOffset">The cell offset.</param>
		/// <param name="digit">The digit.</param>
		public Conclusion(ConclusionType conclusionType, int cellOffset, int digit) =>
			(Type, CellOffset, Digit) = (conclusionType, cellOffset, digit);

		/// <summary>
		/// Initializes an instance with a conclusion type and a candidate offset.
		/// </summary>
		/// <param name="conclusionType">The conclusion type.</param>
		/// <param name="candidateOffset">The candidate offset.</param>
		public Conclusion(ConclusionType conclusionType, int candidateOffset)
			: this(conclusionType, candidateOffset / 9, candidateOffset % 9)
		{
		}


		/// <summary>
		/// The cell offset.
		/// </summary>
		public int CellOffset { get; }

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
		public ConclusionType Type { get; }


		/// <summary>
		/// Put this instance into the specified grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		public void ApplyTo(Grid grid)
		{
			switch (Type)
			{
				case ConclusionType.Assignment:
					grid[CellOffset] = Digit;
					break;
				case ConclusionType.Elimination:
					grid[CellOffset, Digit] = true;
					break;
				default:
					throw new InvalidOperationException("Cannot apply to grid due to invalid conclusion type.");
			}
		}

		/// <inheritdoc/>
		public override bool Equals(object? obj) =>
			obj is Conclusion comparer && Equals(comparer);

		/// <summary>
		/// Indicates whether the current object has the same value with the other one.
		/// </summary>
		/// <param name="other">The other value to compare.</param>
		/// <returns>
		/// The result of this comparsion. <see langword="true"/> if two instances hold a same
		/// value; otherwise, <see langword="false"/>.
		/// </returns>
		public bool Equals(Conclusion other) => GetHashCode() == other.GetHashCode();

		/// <inheritdoc/>
		public override int GetHashCode() => ((int)Type + 1) * (CellOffset * 9 + Digit);

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return $@"r{CellOffset / 9 + 1}c{CellOffset % 9 + 1} {Type switch
			{
				ConclusionType.Assignment => "=",
				ConclusionType.Elimination => "<>",
				_ => throw new InvalidOperationException("Cannot get the string due to invalid conclusion type.")
			}} {Digit + 1}";
		}


		/// <summary>
		/// Indicates whether two instances have a same value.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool operator ==(Conclusion left, Conclusion right) =>
			left.Equals(right);

		/// <summary>
		/// Indicates whether two instances have two different values.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool operator !=(Conclusion left, Conclusion right) =>
			!(left == right);
	}
}
