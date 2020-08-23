using System;
using Sudoku.Data;
using Sudoku.Data.Collections;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Indicates the borescoper's deadly pattern.
	/// </summary>
	public readonly struct Pattern : IEquatable<Pattern>
	{
		/// <summary>
		/// Indicates the internal structure.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This mask is of type <see cref="long"/>:
		/// <code>
		/// 0      7     14     21     28     35     42     49     56
		/// ↓      ↓      ↓      ↓      ↓      ↓      ↓      ↓      ↓
		/// |-------|-------|-------|-------|-------|-------|-------|-------|
		/// </code>
		/// where the bit [0..56] is for 8 cells, the last 7 bits determine the pattern is a
		/// heptagon or a octagon. If the value is 127 (not available), the pattern will be a heptagon.
		/// </para>
		/// <para>
		/// Due to the rendering engine, you have to check this file rather than the tip window.
		/// </para>
		/// </remarks>
		private readonly long _mask;


		/// <summary>
		/// Initializes an instance with the specified mask.
		/// </summary>
		/// <param name="mask">The mask.</param>
		public Pattern(long mask) => _mask = mask;


		/// <summary>
		/// Indicates the pair 1.
		/// </summary>
		public (int _1, int _2) Pair1 => ((int)(_mask >> 7 & 127), (int)(_mask & 127));

		/// <summary>
		/// Indicates the pair 2.
		/// </summary>
		public (int _1, int _2) Pair2 => ((int)(_mask >> 21 & 127), (int)(_mask >> 14 & 127));

		/// <summary>
		/// Indicates the other three (or four) cells.
		/// </summary>
		/// <remarks>
		/// <b>If and only if</b> the fourth value in the returned quadruple is available.
		/// </remarks>
		public (int _1, int _2, int _3, int _4) CenterCells =>
			((int)(_mask >> 49 & 127), (int)(_mask >> 42 & 127), (int)(_mask >> 35 & 127), (int)(_mask >> 28 & 127));

		/// <summary>
		/// Indicates whether the specified pattern is a heptagon.
		/// </summary>
		public bool IsHeptagon => (_mask >> 28 & 127) == 127;

		/// <summary>
		/// Indicates the map of pair 1 cells.
		/// </summary>
		public GridMap Pair1Map => new() { Pair1._1, Pair1._2 };

		/// <summary>
		/// Indicates the map of pair 2 cells.
		/// </summary>
		public GridMap Pair2Map => new() { Pair2._1, Pair2._2 };

		/// <summary>
		/// The map of other three (or four) cells.
		/// </summary>
		public GridMap CenterCellsMap
		{
			get
			{
				var (a, b, c, d) = CenterCells;
				return IsHeptagon ? new() { a, b, c } : new GridMap { a, b, c, d };
			}
		}

		/// <summary>
		/// The map.
		/// </summary>
		public GridMap Map => Pair1Map | Pair2Map | CenterCellsMap;


		/// <include file='....\GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="object"]'/>
		public override bool Equals(object? obj) => obj is Pattern comparer && Equals(comparer);

		/// <inheritdoc/>
		public bool Equals(Pattern other) => _mask == other._mask;

		/// <include file='....\GlobalDocComments.xml' path='comments/method[@name="GetHashCode"]'/>
		public override int GetHashCode() => (int)_mask;

		/// <include file='....\GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override string ToString() => $"{new CellCollection(Map).ToString()}";


		/// <include file='....\GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(Pattern left, Pattern right) => left.Equals(right);

		/// <include file='....\GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(Pattern left, Pattern right) => !(left == right);
	}
}
