using System;

namespace Sudoku.Drawing
{
	public readonly struct Id : IComparable<Id>, IEquatable<Id>
	{
		public Id(int value) => Value = value;


		public int Value { get; }


		public override bool Equals(object? obj) =>
			obj is Id comparer && Equals(comparer);

		public bool Equals(Id other) => Value == other.Value;

		public override int GetHashCode() => Value;

		public int CompareTo(Id other) => Value.CompareTo(other.Value);

		public override string ToString() => Value.ToString();


		public static bool operator ==(Id left, Id right) => left.Equals(right);

		public static bool operator !=(Id left, Id right) => !left.Equals(right);

		public static bool operator >(Id left, Id right) => left.CompareTo(right) > 0;

		public static bool operator <(Id left, Id right) => left.CompareTo(right) < 0;

		public static bool operator >=(Id left, Id right) => left.CompareTo(right) >= 0;

		public static bool operator <=(Id left, Id right) => left.CompareTo(right) <= 0;


		public static implicit operator int(Id id) => id.Value;

		public static implicit operator Id(int value) => new Id(value);
	}
}
