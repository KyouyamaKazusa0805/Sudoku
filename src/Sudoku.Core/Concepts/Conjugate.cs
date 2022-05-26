namespace Sudoku.Concepts;

/// <summary>
/// Represents a <see href="https://sunnieshine.github.io/Sudoku/terms/conjugate-pair">conjugate pair</see>.
/// </summary>
/// <remarks>
/// A <b>Conjugate pair</b> is a pair of two candidates, in the same house where all cells has only
/// two position can fill this candidate.
/// </remarks>
[MaskStyledDataType<int>("From", 10, typeof(int), "To", 20, typeof(int), "Digit", 24, typeof(int))]
[AutoOverridesEquals(nameof(Map), nameof(Digit))]
[AutoOverridesToString(nameof(From), nameof(To), nameof(Digit), Pattern = "{Sudoku.Concepts.Collections.Cells.Empty + [0]} == {Sudoku.Concepts.Collections.Cells.Empty + [1]}({[2] + 1})")]
[AutoOverloadsEqualityOperators]
[AutoImplementsDefaultable("Default", Pattern = "new(-1, -1, -1)")]
public readonly partial struct Conjugate :
	IDefaultable<Conjugate>,
	IEquatable<Conjugate>,
	IEqualityOperators<Conjugate, Conjugate>
{
	/// <summary>
	/// Initializes a <see cref="Conjugate"/> instance with the map and the digit.
	/// The map should contains two cells, the first one is the start one, and the second one is the end one.
	/// </summary>
	/// <param name="map">The map.</param>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conjugate(in Cells map, int digit) : this(map[0], map[1], digit)
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
	/// Indicates the house that two cells lie in.
	/// </summary>
	/// <remarks><inheritdoc cref="Cells.CoveredHouses"/></remarks>
	public int Houses
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Map.CoveredHouses;
	}

	/// <summary>
	/// Indicates the whole map.
	/// </summary>
	public Cells Map
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Cells.Empty + From + To;
	}
}
