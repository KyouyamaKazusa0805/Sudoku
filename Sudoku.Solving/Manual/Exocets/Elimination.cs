using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.DocComments;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Encapsulates an instance to represent a series of eliminations in JE or SE.
	/// </summary>
	public readonly struct Elimination : IValueEquatable<Elimination>
	{
		/// <summary>
		/// Initializes an instance with the eliminations and the reason.
		/// </summary>
		/// <param name="eliminations">(<see langword="in"/> parameter) The eliminations.</param>
		/// <param name="reason">The reason why those candidates can be eliminated.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Elimination(in ImmutableArray<int> eliminations, EliminatedReason reason)
		{
			Eliminations = eliminations;
			Reason = reason;
		}


		/// <summary>
		/// Indicates how many eliminations the instance contains.
		/// </summary>
		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Eliminations.Length;
		}

		/// <summary>
		/// Indicates the eliminations.
		/// </summary>
		public ImmutableArray<int> Eliminations { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

		/// <summary>
		/// Indicates the reason why these candidates can be eliminated.
		/// </summary>
		public EliminatedReason Reason { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }


		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object? obj) => obj is Elimination comparer && Equals(comparer);

		/// <inheritdoc/>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(in Elimination other) => Eliminations == other.Eliminations && Reason == other.Reason;

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode() => Eliminations.GetHashCode() ^ (int)Reason << 17 & 0x135246;

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override unsafe string ToString()
		{
			// To replace each value into separate words.
			// For example: TargetPair => target pair
			string header = Regex.Replace(
				input: Reason.ToString(),
				pattern: @"[A-Z]",
				evaluator: static m => $"{m.Value.ToLower()} ",
				options: RegexOptions.ExplicitCapture,
				matchTimeout: TimeSpan.FromSeconds(5)
			);

			// Converts the first value into upper case.
			// For example: target pair => Target pair
			fixed (char* ptr = header)
			{
				//*ptr = char.ToUpper(*ptr);
				*ptr -= ' ';
			}

			// Returns the result.
			string elimStr = new ConclusionCollection(AsSpan()).ToString();
			return $"* {header} elimination: {elimStr}";
		}

		/// <summary>
		/// Converts all elements to <see cref="Conclusion"/>.
		/// </summary>
		/// <returns>The <see cref="ReadOnlySpan{T}"/> of type <see cref="Conclusion"/>.</returns>
		/// <seealso cref="ReadOnlySpan{T}"/>
		/// <seealso cref="Conclusion"/>
		public ReadOnlySpan<Conclusion> AsSpan()
		{
			var result = new Conclusion[Eliminations.Length];
			for (int i = 0; i < Eliminations.Length; i++)
			{
				result[i] = new(ConclusionType.Elimination, Eliminations[i]);
			}

			return result;
		}

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlySpan<Conclusion>.Enumerator GetEnumerator() => AsSpan().GetEnumerator();


		/// <inheritdoc cref="Operators.operator =="/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(in Elimination left, in Elimination right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(in Elimination left, in Elimination right) => !(left == right);

		/// <summary>
		/// To merge two different instances, and return the merged result.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>
		/// The merged result. The result will contain all eliminations from two instances, and
		/// the reason will use <see langword="operator"/> <c>|</c> to merge them.
		/// </returns>
		public static Elimination operator |(in Elimination left, in Elimination right)
		{
			int[] merged = new int[left.Eliminations.Length + right.Eliminations.Length];
			for (int i = 0; i < left.Eliminations.Length; i++)
			{
				merged[i] = left.Eliminations[i];
			}
			for (int i = 0; i < right.Eliminations.Length; i++)
			{
				merged[i + left.Eliminations.Length] = right.Eliminations[i];
			}

			var flags = left.Reason | right.Reason;
			var mergedArray = ImmutableArray.CreateRange(merged);
			return new(mergedArray, flags);
		}
	}
}
