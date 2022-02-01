using Sudoku.Collections;
using Sudoku.Presentation;

namespace Sudoku;

/// <summary>
/// Encapsulates a conjugate pair.
/// </summary>
/// <remarks>
/// <b>Conjugate pair</b> is a candidate pair (two candidates),
/// these two candidates is in the same region where all cells has only
/// two position can fill this candidate.
/// </remarks>
/// <param name="From">Indicates the cell that is the start cell.</param>
/// <param name="To">Indicates the cell that is end cell.</param>
/// <param name="Digit">Indicates the digit used.</param>
/// <param name="Map">Indicates the pair of maps used.</param>
public readonly record struct ConjugatePair(int From, int To, int Digit, in Cells Map) :
	IDefaultable<ConjugatePair>,
	IEqualityOperators<ConjugatePair, ConjugatePair>
{
	/// <summary>
	/// <inheritdoc cref="IDefaultable{T}.Default"/>
	/// </summary>
	public static readonly ConjugatePair Default = new(-1, -1, -1, Cells.Empty);


	/// <summary>
	/// Initializes a <see cref="ConjugatePair"/> instance with from and to cell offset and a digit.
	/// </summary>
	/// <param name="from">The from cell.</param>
	/// <param name="to">The to cell.</param>
	/// <param name="digit">The digit.</param>
	public ConjugatePair(int from, int to, int digit) : this(from, to, digit, new() { from, to })
	{
	}

	/// <summary>
	/// Initializes a <see cref="ConjugatePair"/> instance with the map and the digit.
	/// The map should contains two cells, the first one is the start one, and the second one is the end one.
	/// </summary>
	/// <param name="map">The map.</param>
	/// <param name="digit">The digit.</param>
	public ConjugatePair(in Cells map, int digit) : this(map[0], map[1], digit, map)
	{
	}


	/// <summary>
	/// Indicates the line that two cells lie in.
	/// </summary>
	public int Line
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Map.CoveredLine;
	}

	/// <summary>
	/// Indicates the region that two cells lie in.
	/// </summary>
	/// <remarks><inheritdoc cref="Cells.CoveredRegions"/></remarks>
	public int Regions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Map.CoveredRegions;
	}

	/// <inheritdoc/>
	readonly bool IDefaultable<ConjugatePair>.IsDefault => this == Default;

	/// <inheritdoc/>
	static ConjugatePair IDefaultable<ConjugatePair>.Default => Default;


	/// <summary>
	/// Determine whether the two conjugate pairs are same.
	/// </summary>
	/// <param name="other">The other instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(in ConjugatePair other) => Map == other.Map && Digit == other.Digit;

	/// <inheritdoc cref="object.GetHashCode"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(Map, Digit);

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() =>
		$"{new Coordinate((byte)From)} == {new Coordinate((byte)To)}({Digit + 1})";
}
