using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.DocComments;

namespace Sudoku.Solving
{
	/// <summary>
	/// Encapsulates a conjugate pair.
	/// </summary>
	/// <remarks>
	/// <b>Conjugate pair</b> is a candidate pair (two candidates),
	/// these two candidates is in the same region where all cells has only
	/// two position can fill this candidate.
	/// </remarks>
	public readonly struct ConjugatePair : IEquatable<ConjugatePair>
	{
		/// <summary>
		/// Initializes an instance with from and to cell offset
		/// and a digit.
		/// </summary>
		/// <param name="from">The from cell.</param>
		/// <param name="to">The to cell.</param>
		/// <param name="digit">The digit.</param>
		public ConjugatePair(int from, int to, int digit) =>
			(Digit, From, To, Map) = (digit, from, to, new GridMap { from, to });

		/// <summary>
		/// Initializes an instance with the map and the digit.
		/// </summary>
		/// <param name="map">The map.</param>
		/// <param name="digit">The digit.</param>
		public ConjugatePair(GridMap map, int digit) => (Digit, From, To, Map) = (digit, map.First, map.SetAt(1), map);


		/// <summary>
		/// Indicates the 'from' cell.
		/// </summary>
		public int From { get; }

		/// <summary>
		/// Indicates the 'to' cell.
		/// </summary>
		public int To { get; }

		/// <summary>
		/// Indicates the digit.
		/// </summary>
		public int Digit { get; }

		/// <summary>
		/// Indicates the line that two cells lie in.
		/// </summary>
		public int Line => Map.CoveredLine;

		/// <summary>
		/// Indicates the region that two cells lie in.
		/// </summary>
		public IEnumerable<int> Region => Map.CoveredRegions;

		/// <summary>
		/// Indicates the inner map.
		/// </summary>
		public GridMap Map { get; }


		/// <inheritdoc/>
		public override bool Equals(object? obj) => obj is ConjugatePair comparer && Equals(comparer);

		/// <inheritdoc/>
		public bool Equals(ConjugatePair other) => Map == other.Map && Digit == other.Digit;

		/// <inheritdoc cref="object.GetHashCode"/>
		public override int GetHashCode() => Map.GetHashCode() ^ Digit;

		/// <inheritdoc cref="object.ToString"/>
		public override string ToString()
		{
			int v = Digit + 1;
			string fromCell = new GridMap { From }.ToString();
			string toCell = new GridMap { To }.ToString();
			return $"{fromCell} == {toCell}({v})";
		}


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(ConjugatePair left, ConjugatePair right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(ConjugatePair left, ConjugatePair right) => !(left == right);
	}
}
