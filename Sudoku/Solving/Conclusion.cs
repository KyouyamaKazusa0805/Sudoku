using System;
using Sudoku.Data.Meta;

namespace Sudoku.Solving
{
	public readonly struct Conclusion : IEquatable<Conclusion>
	{
		public Conclusion(ConclusionType conclusionType, int cellOffset, int digit) =>
			(Type, CellOffset, Digit) = (conclusionType, cellOffset, digit);


		public int CellOffset { get; }

		public int Digit { get; }

		public ConclusionType Type { get; }


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

		public override bool Equals(object? obj) =>
			obj is Conclusion comparer && Equals(comparer);

		public bool Equals(Conclusion other) => GetHashCode() == other.GetHashCode();

		public override int GetHashCode() => ((int)Type + 1) * (CellOffset * 9 + Digit);

		public override string ToString()
		{
			return $@"r{CellOffset / 9 + 1}c{CellOffset % 9 + 1} {Type switch
			{
				ConclusionType.Assignment => "=",
				ConclusionType.Elimination => "<>",
				_ => throw new InvalidOperationException("Cannot get the string due to invalid conclusion type.")
			}} {Digit + 1}";
		}


		public static bool operator ==(Conclusion left, Conclusion right) =>
			left.Equals(right);

		public static bool operator !=(Conclusion left, Conclusion right) =>
			!(left == right);
	}
}
