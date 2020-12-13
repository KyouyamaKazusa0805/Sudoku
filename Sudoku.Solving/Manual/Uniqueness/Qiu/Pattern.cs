using System;
using Sudoku.Data;
using Sudoku.DocComments;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	/// <summary>
	/// Encapsulates a pattern for Qiu's deadly pattern (QDP).
	/// </summary>
	public readonly struct Pattern : IValueEquatable<Pattern>
	{
		/// <summary>
		/// Initializes an instance with the specified maps.
		/// </summary>
		/// <param name="square">(<see langword="in"/> parameter) The square.</param>
		/// <param name="baseLine">(<see langword="in"/> parameter) The base line.</param>
		/// <param name="pair">(<see langword="in"/> parameter) The pair.</param>
		public Pattern(in GridMap square, in GridMap baseLine, in GridMap pair)
		{
			Square = square;
			BaseLine = baseLine;
			Pair = pair;
		}


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


		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="pair">(<see langword="out"/> parameter) The pair map.</param>
		/// <param name="square">(<see langword="out"/> parameter) The square map.</param>
		/// <param name="baseLine">(<see langword="out"/> parameter) The base line map.</param>
		public void Deconstruct(out GridMap pair, out GridMap square, out GridMap baseLine)
		{
			pair = Pair;
			square = Square;
			baseLine = BaseLine;
		}

		/// <inheritdoc/>
		public override bool Equals(object? obj) => obj is Pattern other && Equals(other);

		/// <inheritdoc/>
		[CLSCompliant(false)]
		public bool Equals(in Pattern other) =>
			Square == other.Square && BaseLine == other.BaseLine && Pair == other.Pair;

		/// <inheritdoc/>
		public override int GetHashCode() => FullMap.GetHashCode();

		/// <inheritdoc cref="object.ToString"/>
		public override string ToString() => FullMap.ToString();


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(in Pattern left, in Pattern right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(in Pattern left, in Pattern right) => !(left == right);
	}
}
