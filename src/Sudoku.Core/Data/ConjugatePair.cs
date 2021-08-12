using System.Text.Json;
using Sudoku.CodeGenerating;

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a conjugate pair.
	/// </summary>
	/// <remarks>
	/// <b>Conjugate pair</b> is a candidate pair (two candidates),
	/// these two candidates is in the same region where all cells has only
	/// two position can fill this candidate.
	/// </remarks>
	[AutoHashCode(nameof(BaseHashCode), nameof(Map))]
	[AutoEquality(nameof(Map), nameof(Digit))]
	public readonly partial struct ConjugatePair : IValueEquatable<ConjugatePair>, IJsonSerializable<ConjugatePair, ConjugatePair.JsonConverter>
	{
		/// <summary>
		/// Initializes an instance with from and to cell offset
		/// and a digit.
		/// </summary>
		/// <param name="from">The from cell.</param>
		/// <param name="to">The to cell.</param>
		/// <param name="digit">The digit.</param>
		public ConjugatePair(int from, int to, int digit)
		{
			Digit = digit;
			From = from;
			To = to;
			Map = new() { from, to };
		}

		/// <summary>
		/// Initializes an instance with the map and the digit. The map should contains two cells,
		/// the first one is the start one, and the second one is the end one.
		/// </summary>
		/// <param name="map">The map.</param>
		/// <param name="digit">The digit.</param>
		public ConjugatePair(in Cells map, int digit)
		{
			Digit = digit;
			From = map[0];
			To = map[1];
			Map = map;
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

		/// <summary>
		/// Indicates the line that two cells lie in.
		/// </summary>
		public int Line => Map.CoveredLine;

		/// <summary>
		/// Indicates the region that two cells lie in.
		/// </summary>
		/// <remarks>
		/// The return value will be an <see cref="int"/> indicating the covered region. Bits set 1
		/// are covered regions.
		/// </remarks>
		public int Regions => Map.CoveredRegions;

		/// <summary>
		/// Indicates the inner map.
		/// </summary>
		public Cells Map { get; }

		/// <summary>
		/// Indicates the base hash code.
		/// </summary>
		private int BaseHashCode => Digit << 17;


		/// <inheritdoc cref="object.ToString"/>
		public override string ToString() =>
			$"{new Cells { From }.ToString()} == {new Cells { To }.ToString()}({(Digit + 1).ToString()})";
	}
}
