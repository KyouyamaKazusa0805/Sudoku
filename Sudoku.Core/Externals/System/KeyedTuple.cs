#if false
#pragma warning disable CS1591

using System.Runtime.CompilerServices;
using Sudoku.Constants;
using Sudoku.Extensions;

namespace System
{
	public sealed record KeyedTuple<T1, T2>(T1 Item1, T2 Item2, int PriorKey) : ITuple
	{
		public KeyedTuple(T1 item1, T2 item2) : this(item1, item2, 1) { }


		int ITuple.Length => 2;


		object? ITuple.this[int index] => PriorKey switch { 1 => Item1, 2 => Item2, _ => Throwings.ImpossibleCase };

		public override string ToString() => ((ITuple)this)[PriorKey].NullableToString();
	}

	public sealed record KeyedTuple<T1, T2, T3>(T1 Item1, T2 Item2, T3 Item3, int PriorKey) : ITuple
	{
		public KeyedTuple(T1 item1, T2 item2, T3 item3) : this(item1, item2, item3, 1) { }


		int ITuple.Length => 3;


		object? ITuple.this[int index] => PriorKey switch { 1 => Item1, 2 => Item2, 3 => Item3, _ => Throwings.ImpossibleCase };

		public override string ToString() => ((ITuple)this)[PriorKey].NullableToString();
	}

	public sealed record KeyedTuple<T1, T2, T3, T4>(T1 Item1, T2 Item2, T3 Item3, T4 Item4, int PriorKey) : ITuple
	{
		public KeyedTuple(T1 item1, T2 item2, T3 item3, T4 item4) : this(item1, item2, item3, item4, 1) { }


		int ITuple.Length => 4;


		object? ITuple.this[int index] => PriorKey switch { 1 => Item1, 2 => Item2, 3 => Item3, 4 => Item4, _ => Throwings.ImpossibleCase };

		public override string ToString() => ((ITuple)this)[PriorKey].NullableToString();
	}
}
#endif