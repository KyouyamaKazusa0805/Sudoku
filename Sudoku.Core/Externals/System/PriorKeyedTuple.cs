#if CSHARP_9_PREVIEW
#pragma warning disable CS1591
#endif

using System.Runtime.CompilerServices;
using Sudoku.Constants;
using Sudoku.Extensions;

namespace System
{
	/// <summary>
	/// Provides a tuple with a primary element, which means the tuple contains multiple items,
	/// but the only specified item can be output as <see cref="string"/> text.
	/// </summary>
	/// <typeparam name="T1">The type of the property <see cref="PriorKeyedTuple{T1, T2}.Item1"/>.</typeparam>
	/// <typeparam name="T2">The type of the property <see cref="PriorKeyedTuple{T1, T2}.Item2"/>.</typeparam>
	public sealed record PriorKeyedTuple<T1, T2>(T1 Item1, T2 Item2, int PriorKey) : ITuple
	{
		/// <summary>
		/// Initializes an instance with the specified 2 items, and the first one is the prior key.
		/// </summary>
		/// <param name="item1">The item 1.</param>
		/// <param name="item2">The item 2.</param>
		public PriorKeyedTuple(T1 item1, T2 item2) : this(item1, item2, 1) { }


		/// <inheritdoc/>
		int ITuple.Length => 2;


		/// <inheritdoc/>
		object? ITuple.this[int index] =>
			PriorKey switch
			{
				1 => Item1,
				2 => Item2,
				_ => Throwings.ImpossibleCase
			};

		/// <inheritdoc/>
		public override string ToString() => ((ITuple)this)[PriorKey].NullableToString();
	}

	/// <summary>
	/// Provides a tuple with a primary element, which means the tuple contains multiple items,
	/// but the only specified item can be output as <see cref="string"/> text.
	/// </summary>
	/// <typeparam name="T1">The type of the property <see cref="PriorKeyedTuple{T1, T2, T3}.Item1"/>.</typeparam>
	/// <typeparam name="T2">The type of the property <see cref="PriorKeyedTuple{T1, T2, T3}.Item2"/>.</typeparam>
	/// <typeparam name="T3">The type of the property <see cref="PriorKeyedTuple{T1, T2, T3}.Item3"/>.</typeparam>
	public sealed record PriorKeyedTuple<T1, T2, T3>(T1 Item1, T2 Item2, T3 Item3, int PriorKey) : ITuple
	{
		/// <summary>
		/// Initializes an instance with the specified 3 items, and the first one is the prior key.
		/// </summary>
		/// <param name="item1">The item 1.</param>
		/// <param name="item2">The item 2.</param>
		/// <param name="item3">The item 3.</param>
		public PriorKeyedTuple(T1 item1, T2 item2, T3 item3) : this(item1, item2, item3, 1) { }


		/// <inheritdoc/>
		int ITuple.Length => 3;


		/// <inheritdoc/>
		object? ITuple.this[int index] =>
			PriorKey switch
			{
				1 => Item1,
				2 => Item2,
				3 => Item3,
				_ => Throwings.ImpossibleCase
			};

		/// <inheritdoc/>
		public override string ToString() => ((ITuple)this)[PriorKey].NullableToString();
	}

	/// <summary>
	/// Provides a tuple with a primary element, which means the tuple contains multiple items,
	/// but the only specified item can be output as <see cref="string"/> text.
	/// </summary>
	/// <typeparam name="T1">The type of the property <see cref="PriorKeyedTuple{T1, T2, T3, T4}.Item1"/>.</typeparam>
	/// <typeparam name="T2">The type of the property <see cref="PriorKeyedTuple{T1, T2, T3, T4}.Item2"/>.</typeparam>
	/// <typeparam name="T3">The type of the property <see cref="PriorKeyedTuple{T1, T2, T3, T4}.Item3"/>.</typeparam>
	/// <typeparam name="T4">The type of the property <see cref="PriorKeyedTuple{T1, T2, T3, T4}.Item4"/>.</typeparam>
	public sealed record PriorKeyedTuple<T1, T2, T3, T4>(T1 Item1, T2 Item2, T3 Item3, T4 Item4, int PriorKey) : ITuple
	{
		/// <summary>
		/// Initializes an instance with the specified 4 items, and the first one is the prior key.
		/// </summary>
		/// <param name="item1">The item 1.</param>
		/// <param name="item2">The item 2.</param>
		/// <param name="item3">The item 3.</param>
		/// <param name="item4">The item 4.</param>
		public PriorKeyedTuple(T1 item1, T2 item2, T3 item3, T4 item4) : this(item1, item2, item3, item4, 1) { }


		/// <inheritdoc/>
		int ITuple.Length => 4;


		/// <inheritdoc/>
		object? ITuple.this[int index] =>
			PriorKey switch
			{
				1 => Item1,
				2 => Item2,
				3 => Item3,
				4 => Item4,
				_ => Throwings.ImpossibleCase
			};

		/// <inheritdoc/>
		public override string ToString() => ((ITuple)this)[PriorKey].NullableToString();
	}
}
