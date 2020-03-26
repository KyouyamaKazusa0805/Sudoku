using System.Runtime.CompilerServices;
using Sudoku;
using Sudoku.Data.Extensions;

namespace System
{
	public readonly struct PrimaryElementTuple<T1, T2, T3> : ITuple
	{
		public PrimaryElementTuple(T1 v1, T2 v2, T3 v3) : this(v1, v2, v3, 1)
		{
		}

		public PrimaryElementTuple(T1 v1, T2 v2, T3 v3, int primaryElementKey)
		{
			(Value1, Value2, Value3, PrimaryElementKey) = (
				v1, v2, v3,
				primaryElementKey >= 1 && primaryElementKey <= 3
					? primaryElementKey
					: throw new ArgumentOutOfRangeException(nameof(primaryElementKey)));
		}


		public T1 Value1 { get; }

		public T2 Value2 { get; }

		public T3 Value3 { get; }

		public int PrimaryElementKey { get; }

		int ITuple.Length => 3;


		object? ITuple.this[int index]
		{
			get
			{
				return PrimaryElementKey switch
				{
					1 => Value1,
					2 => Value2,
					3 => Value3,
					_ => throw Throwing.ImpossibleCase
				};
			}
		}


		public void Deconstruct(out T1 v1, out T2 v2, out T3 v3) =>
			(v1, v2, v3) = (Value1, Value2, Value3);

		public override string ToString() => ((ITuple)this)[PrimaryElementKey].NullableToString();
	}
}
