using System.Collections.Generic;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Provides all data for Borescoper's deadly pattern
	/// (basic and extended types).
	/// </summary>
	public abstract class BdpDetailData
	{
		/// <summary>
		/// Provides passing data when initializing an instance of derived types.
		/// </summary>
		/// <param name="cells">All cells.</param>
		/// <param name="digits">All digits.</param>
		protected BdpDetailData(IReadOnlyList<int> cells, IReadOnlyList<int> digits) =>
			(Cells, Digits) = (cells, digits);


		/// <summary>
		/// <para>
		/// Indicates the type of the Borescoper's deadly pattern.
		/// where the value is between 1 and 4.
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

		/// <inheritdoc/>
		public IReadOnlyList<int> Digits { get; }

		/// <inheritdoc/>
		public IReadOnlyList<int> Cells { get; }


		/// <inheritdoc/>
		public abstract override string ToString();
	}
}
