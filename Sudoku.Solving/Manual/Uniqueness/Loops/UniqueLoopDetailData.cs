using System;
using System.Collections.Generic;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	/// <summary>
	/// Provides all data for unique loops (basic and extended types).
	/// </summary>
	public abstract class UniqueLoopDetailData : IEquatable<UniqueLoopDetailData>
	{
		/// <summary>
		/// Provides passing data when initializing an instance of derived types.
		/// </summary>
		/// <param name="cells">All cells.</param>
		/// <param name="digits">All digits.</param>
		protected UniqueLoopDetailData(IReadOnlyList<int> cells, IReadOnlyList<int> digits) =>
			(Cells, Digits) = (cells, digits);


		/// <summary>
		/// <para>
		/// Indicates the type of the unique loop,
		/// where the value is between 1 to 4.
		/// </para>
		/// <para>
		/// You can find all types in detail in 'remarks' part of this page.
		/// </para>
		/// </summary>
		/// <remarks>
		/// All types:
		/// <list type="table">
		/// <item>Type 1<term></term>
		/// <description>Basic type.</description>
		/// </item>
		/// <item>Type 2<term></term>
		/// <description>Generalized locked candidates type.</description>
		/// </item>
		/// <item>Type 3<term></term>
		/// <description>
		/// Generalized subset type (+ naked or hidden subset).
		/// </description>
		/// </item>
		/// <item>Type 4<term></term>
		/// <description>Conjugate pair type.</description>
		/// </item>
		/// </list>
		/// </remarks>
		public abstract int Type { get; }

		/// <summary>
		/// Indicates all cells used.
		/// </summary>
		public IReadOnlyList<int> Cells { get; }

		/// <summary>
		/// Indicates all digits used.
		/// </summary>
		public IReadOnlyList<int> Digits { get; }

		/// <inheritdoc/>
		public sealed override bool Equals(object? obj) =>
			obj is UniqueLoopDetailData comparer && Equals(comparer);

		/// <inheritdoc/>
		public virtual bool Equals(UniqueLoopDetailData other) =>
			ToString() == other.ToString();

		/// <inheritdoc/>
		public override int GetHashCode() => ToString().GetHashCode();

		/// <inheritdoc/>
		public abstract override string ToString();


		/// <summary>
		/// Indicates whether two instances have a same value.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool operator ==(UniqueLoopDetailData left, UniqueLoopDetailData right) =>
			left.Equals(right);

		/// <summary>
		/// Indicates whether two instances have two different values.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool operator !=(UniqueLoopDetailData left, UniqueLoopDetailData right) =>
			!(left == right);
	}
}
