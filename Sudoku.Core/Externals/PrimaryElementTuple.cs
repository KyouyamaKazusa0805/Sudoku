using System.Runtime.CompilerServices;
using Sudoku;
using Sudoku.Data.Extensions;

namespace System
{
	/// <summary>
	/// Provides a tuple with a primary element.
	/// </summary>
	/// <typeparam name="T1">The type of value 1.</typeparam>
	/// <typeparam name="T2">The type of value 2.</typeparam>
	/// <typeparam name="T3">The type of value 3.</typeparam>
	public readonly struct PrimaryElementTuple<T1, T2, T3> : ITuple
	{
		/// <summary>
		/// Initializes an instance with three values, and the first one
		/// is the primary key.
		/// </summary>
		/// <param name="v1">The value 1.</param>
		/// <param name="v2">The value 2.</param>
		/// <param name="v3">The value 3.</param>
		public PrimaryElementTuple(T1 v1, T2 v2, T3 v3) : this(v1, v2, v3, 1)
		{
		}

		/// <summary>
		/// Initializes an instance with three values and a primary key.
		/// </summary>
		/// <param name="v1">The value 1.</param>
		/// <param name="v2">The value 2.</param>
		/// <param name="v3">The value 3.</param>
		/// <param name="primaryElementKey">The primary key.</param>
		public PrimaryElementTuple(T1 v1, T2 v2, T3 v3, int primaryElementKey)
		{
			(Value1, Value2, Value3, PrimaryElementKey) = (
				v1, v2, v3,
				primaryElementKey >= 1 && primaryElementKey <= 3
					? primaryElementKey
					: throw new ArgumentOutOfRangeException(nameof(primaryElementKey)));
		}


		/// <summary>
		/// Indicates the value 1.
		/// </summary>
		public T1 Value1 { get; }

		/// <summary>
		/// Indicates the value 2.
		/// </summary>
		public T2 Value2 { get; }

		/// <summary>
		/// Indicates the value 3.
		/// </summary>
		public T3 Value3 { get; }

		/// <summary>
		/// Indicates the index of the primary value.
		/// </summary>
		public int PrimaryElementKey { get; }

		/// <inheritdoc/>
		int ITuple.Length => 3;


		/// <inheritdoc/>
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


		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="v1">(<see langword="out"/> parameter) The value 1.</param>
		/// <param name="v2">(<see langword="out"/> parameter) The value 2.</param>
		/// <param name="v3">(<see langword="out"/> parameter) The value 3.</param>
		public void Deconstruct(out T1 v1, out T2 v2, out T3 v3) =>
			(v1, v2, v3) = (Value1, Value2, Value3);

		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override string ToString() => ((ITuple)this)[PrimaryElementKey].NullableToString();
	}
}
