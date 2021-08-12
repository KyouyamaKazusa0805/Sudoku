namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	/// <summary>
	/// Encapsulates a pattern for Qiu's deadly pattern (QDP).
	/// </summary>
	[AutoDeconstruct(nameof(Pair), nameof(Square), nameof(BaseLine))]
	[AutoHashCode(nameof(FullMap))]
	[AutoEquality(nameof(Pair), nameof(Square), nameof(BaseLine))]
	public readonly partial struct Pattern : IValueEquatable<Pattern>
	{
		/// <summary>
		/// Initializes an instance with the specified maps.
		/// </summary>
		/// <param name="square">The square.</param>
		/// <param name="baseLine">The base line.</param>
		/// <param name="pair">The pair.</param>
		public Pattern(in Cells square, in Cells baseLine, in Cells pair)
		{
			Square = square;
			BaseLine = baseLine;
			Pair = pair;
		}


		/// <summary>
		/// Indicates the square.
		/// </summary>
		public Cells Square { get; }

		/// <summary>
		/// Indicates the base line.
		/// </summary>
		public Cells BaseLine { get; }

		/// <summary>
		/// Indicates the pair.
		/// </summary>
		public Cells Pair { get; }

		/// <summary>
		/// Indicates the full map.
		/// </summary>
		public Cells FullMap => Square | BaseLine | Pair;


		/// <inheritdoc cref="object.ToString"/>
		public override string ToString() => FullMap.ToString();
	}
}
