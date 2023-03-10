namespace Sudoku.Concepts;

/// <summary>
/// Represents a <see href="https://sunnieshine.github.io/Sudoku/terms/conjugate-pair">conjugate pair</see>.
/// </summary>
/// <remarks>
/// A <b>Conjugate pair</b> is a pair of two candidates, in the same house where all cells has only
/// two position can fill this candidate.
/// </remarks>
public readonly partial struct Conjugate : IEquatable<Conjugate>, IEqualityOperators<Conjugate, Conjugate, bool>
{
	/// <summary>
	/// Indicates the mask.
	/// </summary>
	private readonly int _mask;


	/// <summary>
	/// Initializes a <see cref="Conjugate"/> instance with from and to cell offset and a digit.
	/// </summary>
	/// <param name="from">The from cell.</param>
	/// <param name="to">The to cell.</param>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conjugate(int from, int to, int digit) => _mask = digit << 20 | from << 10 | to;

	/// <summary>
	/// Initializes a <see cref="Conjugate"/> instance with the map and the digit.
	/// The map should contains two cells, the first one is the start one, and the second one is the end one.
	/// </summary>
	/// <param name="map">The map.</param>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conjugate(scoped in CellMap map, int digit) : this(map[0], map[1], digit)
	{
	}


	/// <summary>
	/// Indicates the cell that starts with the conjugate pair.
	/// </summary>
	public int From
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _mask & 1023;
	}

	/// <summary>
	/// Indicates the cell that ends with the conjugate pair.
	/// </summary>
	public int To
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _mask >> 10 & 1023;
	}

	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public int Digit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _mask >> 20 & 15;
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
	/// <remarks><inheritdoc cref="CellMap.CoveredHouses"/></remarks>
	public int Houses
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Map.CoveredHouses;
	}

	/// <summary>
	/// Indicates the whole map.
	/// </summary>
	public CellMap Map
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => CellsMap[From] + To;
	}

	private int FromCandidate => From * 9 + Digit;

	private int ToCandidate => To * 9 + Digit;


	[DeconstructionMethod]
	public partial void Deconstruct([DeconstructionMethodArgument(nameof(FromCandidate))] out int fromCand, [DeconstructionMethodArgument(nameof(ToCandidate))] out int toCand);

	[GeneratedOverriddingMember(GeneratedEqualsBehavior.TypeCheckingAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Conjugate other) => Map == other.Map && Digit == other.Digit;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Map), nameof(Digit))]
	public override partial int GetHashCode();

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => $"{CellsMap[From]} == {RxCyNotation.ToCandidateString(To * 9 + Digit)}";


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Conjugate left, Conjugate right) => left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Conjugate left, Conjugate right) => !left.Equals(right);
}
