using System.Runtime.CompilerServices;
using Sudoku.Constants;
using Sudoku.Extensions;

namespace System
{
	/// <summary>
	/// Provides a tuple with a primary element.
	/// </summary>
	/// <typeparam name="T1"> The type of the value 1.</typeparam>
	/// <typeparam name="T2"> The type of the value 2.</typeparam>
	public readonly struct PrimaryElementTuple<T1, T2> : ITuple
	{
		/// <summary>
		/// Initializes an instance with four values, and the first one
		/// is the primary key.
		/// </summary>
		public PrimaryElementTuple(T1 v1, T2 v2) : this(v1, v2, 1)
		{
		}

		/// <summary>
		/// Initializes an instance with four elements and a primary key.
		/// </summary>
		/// <param name="v1"> The value 1.</param>
		/// <param name="v2"> The value 2.</param>
		/// <param name="primaryElementKey">The primary key.</param>
		public PrimaryElementTuple(T1 v1, T2 v2, int primaryElementKey) =>
			(Value1, Value2, PrimaryElementKey) = (
				v1, v2,
				primaryElementKey >= 1 && primaryElementKey <= 2
					? primaryElementKey
					: throw new ArgumentOutOfRangeException(nameof(primaryElementKey)));


		/// <summary>
		/// Indicates the value 1.
		/// </summary>
		public T1 Value1 { get; }

		/// <summary>
		/// Indicates the value 2.
		/// </summary>
		public T2 Value2 { get; }

		/// <summary>
		/// Indicates the index of the primary value.
		/// </summary>
		public int PrimaryElementKey { get; }

		/// <inheritdoc/>
		int ITuple.Length => 2;

		/// <inheritdoc/>
		object? ITuple.this[int index] =>
			PrimaryElementKey switch
			{
				1 => Value1,
				2 => Value2,
				_ => throw Throwings.ImpossibleCase
			};


		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="v1">(<see langword="out"/> parameter) The value 1.</param>
		/// <param name="v2">(<see langword="out"/> parameter) The value 2.</param>
		public void Deconstruct(out T1 v1, out T2 v2) => (v1, v2) = (Value1, Value2);

		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override string ToString() => ((ITuple)this)[PrimaryElementKey].NullableToString();
	}

	/// <summary>
	/// Provides a tuple with a primary element.
	/// </summary>
	/// <typeparam name="T1"> The type of the value 1.</typeparam>
	/// <typeparam name="T2"> The type of the value 2.</typeparam>
	/// <typeparam name="T3"> The type of the value 3.</typeparam>
	public readonly struct PrimaryElementTuple<T1, T2, T3> : ITuple
	{
		/// <summary>
		/// Initializes an instance with five values, and the first one
		/// is the primary key.
		/// </summary>
		public PrimaryElementTuple(T1 v1, T2 v2, T3 v3) : this(v1, v2, v3, 1)
		{
		}

		/// <summary>
		/// Initializes an instance with five elements and a primary key.
		/// </summary>
		/// <param name="v1"> The value 1.</param>
		/// <param name="v2"> The value 2.</param>
		/// <param name="v3"> The value 3.</param>
		/// <param name="primaryElementKey">The primary key.</param>
		public PrimaryElementTuple(T1 v1, T2 v2, T3 v3, int primaryElementKey) =>
			(Value1, Value2, Value3, PrimaryElementKey) = (
				v1, v2, v3,
				primaryElementKey >= 1 && primaryElementKey <= 3
					? primaryElementKey
					: throw new ArgumentOutOfRangeException(nameof(primaryElementKey)));


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
		object? ITuple.this[int index] =>
			PrimaryElementKey switch
			{
				1 => Value1,
				2 => Value2,
				3 => Value3,
				_ => throw Throwings.ImpossibleCase
			};


		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="v1">(<see langword="out"/> parameter) The value 1.</param>
		/// <param name="v2">(<see langword="out"/> parameter) The value 2.</param>
		/// <param name="v3">(<see langword="out"/> parameter) The value 3.</param>
		public void Deconstruct(out T1 v1, out T2 v2, out T3 v3) => (v1, v2, v3) = (Value1, Value2, Value3);

		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override string ToString() => ((ITuple)this)[PrimaryElementKey].NullableToString();
	}

	/// <summary>
	/// Provides a tuple with a primary element.
	/// </summary>
	/// <typeparam name="T1"> The type of the value 1.</typeparam>
	/// <typeparam name="T2"> The type of the value 2.</typeparam>
	/// <typeparam name="T3"> The type of the value 3.</typeparam>
	/// <typeparam name="T4"> The type of the value 4.</typeparam>
	public readonly struct PrimaryElementTuple<T1, T2, T3, T4> : ITuple
	{
		/// <summary>
		/// Initializes an instance with six values, and the first one
		/// is the primary key.
		/// </summary>
		public PrimaryElementTuple(T1 v1, T2 v2, T3 v3, T4 v4) : this(v1, v2, v3, v4, 1)
		{
		}

		/// <summary>
		/// Initializes an instance with six elements and a primary key.
		/// </summary>
		/// <param name="v1"> The value 1.</param>
		/// <param name="v2"> The value 2.</param>
		/// <param name="v3"> The value 3.</param>
		/// <param name="v4"> The value 4.</param>
		/// <param name="primaryElementKey">The primary key.</param>
		public PrimaryElementTuple(T1 v1, T2 v2, T3 v3, T4 v4, int primaryElementKey) =>
			(Value1, Value2, Value3, Value4, PrimaryElementKey) = (
				v1, v2, v3, v4,
				primaryElementKey >= 1 && primaryElementKey <= 4
					? primaryElementKey
					: throw new ArgumentOutOfRangeException(nameof(primaryElementKey)));


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
		/// Indicates the value 4.
		/// </summary>
		public T4 Value4 { get; }

		/// <summary>
		/// Indicates the index of the primary value.
		/// </summary>
		public int PrimaryElementKey { get; }

		/// <inheritdoc/>
		int ITuple.Length => 4;

		/// <inheritdoc/>
		object? ITuple.this[int index] =>
			PrimaryElementKey switch
			{
				1 => Value1,
				2 => Value2,
				3 => Value3,
				4 => Value4,
				_ => throw Throwings.ImpossibleCase
			};


		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="v1">(<see langword="out"/> parameter) The value 1.</param>
		/// <param name="v2">(<see langword="out"/> parameter) The value 2.</param>
		/// <param name="v3">(<see langword="out"/> parameter) The value 3.</param>
		/// <param name="v4">(<see langword="out"/> parameter) The value 4.</param>
		public void Deconstruct(out T1 v1, out T2 v2, out T3 v3, out T4 v4) => (v1, v2, v3, v4) = (Value1, Value2, Value3, Value4);

		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override string ToString() => ((ITuple)this)[PrimaryElementKey].NullableToString();
	}
}