using System;
using System.Collections.Generic;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Provides all data for avoidable rectangles (basic and extended types).
	/// </summary>
	[Obsolete]
	public abstract class ArDetailData : IRectangleDetailData
	{
		/// <summary>
		/// Provides passing data when initializing an instance of derived types.
		/// </summary>
		/// <param name="cells">All cells.</param>
		/// <param name="digits">All digits.</param>
		protected ArDetailData(
			IReadOnlyList<int> cells, IReadOnlyList<int> digits) =>
			(Cells, Digits) = (cells, digits);


		/// <summary>
		/// <para>
		/// Indicates the type of the avoidable rectangle,
		/// where the value is between 1 and 3.
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
		/// </list>
		/// </remarks>
		public abstract int Type { get; }

		/// <inheritdoc/>
		public IReadOnlyList<int> Digits { get; }

		/// <inheritdoc/>
		public IReadOnlyList<int> Cells { get; }


		/// <inheritdoc/>
		public abstract override string ToString();
	}
}
