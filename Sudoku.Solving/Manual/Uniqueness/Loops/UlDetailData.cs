using System;
using System.Collections.Generic;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	/// <summary>
	/// Provides all data for unique loops (basic and extended types).
	/// </summary>
	public abstract class UlDetailData : IEquatable<UlDetailData>
	{
		/// <summary>
		/// Provides passing data when initializing an instance of derived types.
		/// </summary>
		/// <param name="cells">All cells.</param>
		/// <param name="digits">All digits.</param>
		protected UlDetailData(IReadOnlyList<int> cells, IReadOnlyList<int> digits) =>
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
			obj is UlDetailData comparer && Equals(comparer);

		/// <inheritdoc/>
		public virtual bool Equals(UlDetailData other) =>
			ToString() == other.ToString();

		/// <inheritdoc/>
		public override int GetHashCode() => ToString().GetHashCode();

		/// <inheritdoc/>
		public abstract override string ToString();


		/// <include file='../../../../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(UlDetailData left, UlDetailData right) =>
			left.Equals(right);

		/// <include file='../../../../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(UlDetailData left, UlDetailData right) =>
			!(left == right);
	}
}
