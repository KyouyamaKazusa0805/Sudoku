#pragma warning disable CA1815

using Sudoku.Data;
using Sudoku.Data.Collections;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	/// <summary>
	/// Encapsulates a pattern for Qiu's deadly pattern (QDP).
	/// </summary>
	public readonly struct Pattern
	{
		/// <summary>
		/// Initializes an instance with the specified maps.
		/// </summary>
		/// <param name="square">The square.</param>
		/// <param name="baseLine">The base line.</param>
		/// <param name="pair">The pair.</param>
		public Pattern(GridMap square, GridMap baseLine, GridMap pair) =>
			(Square, BaseLine, Pair) = (square, baseLine, pair);


		/// <summary>
		/// Indicates the square.
		/// </summary>
		public GridMap Square { get; }

		/// <summary>
		/// Indicates the base line.
		/// </summary>
		public GridMap BaseLine { get; }

		/// <summary>
		/// Indicates the pair.
		/// </summary>
		public GridMap Pair { get; }

		/// <summary>
		/// Indicates the full map.
		/// </summary>
		public GridMap FullMap => Square | BaseLine | Pair;


		/// <include file='....\GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="pair">(<see langword="out"/> parameter) The pair map.</param>
		/// <param name="square">(<see langword="out"/> parameter) The square map.</param>
		/// <param name="baseLine">(<see langword="out"/> parameter) The base line map.</param>
		public void Deconstruct(out GridMap pair, out GridMap square, out GridMap baseLine) =>
			(pair, square, baseLine) = (Pair, Square, BaseLine);

		/// <inheritdoc cref="object.ToString"/>
		public override string ToString() => new CellCollection(FullMap).ToString();
	}
}
