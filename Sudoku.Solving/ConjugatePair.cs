using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Solving.Utils;

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
		/// The inner data structure.
		/// </summary>
		private readonly GridMap _map;


		/// <summary>
		/// Initializes an instance with from and to cell offset
		/// and a digit.
		/// </summary>
		/// <param name="from">The from cell.</param>
		/// <param name="to">The to cell.</param>
		/// <param name="digit">The digit.</param>
		public ConjugatePair(int from, int to, int digit)
		{
			(Digit, From, To) = (digit, from, to);
			_map = new GridMap(stackalloc[] { from, to });
		}


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


		/// <inheritdoc/>
		public override bool Equals(object? obj) =>
			obj is ConjugatePair comparer && Equals(comparer);

		/// <inheritdoc/>
		public bool Equals(ConjugatePair other) =>
			_map == other._map && Digit == other.Digit;

		/// <inheritdoc/>
		public override int GetHashCode() => _map.GetHashCode() ^ Digit;

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override string ToString()
		{
			int v = Digit + 1;
			string fromCell = CellUtils.ToString(From);
			string toCell = CellUtils.ToString(To);
			return $"{fromCell}=={toCell}({v})";
		}


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(ConjugatePair left, ConjugatePair right) =>
			left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(ConjugatePair left, ConjugatePair right) =>
			!(left == right);
	}
}
