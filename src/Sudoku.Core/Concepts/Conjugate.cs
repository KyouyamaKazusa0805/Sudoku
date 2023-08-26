namespace Sudoku.Concepts;

using ConjugateImpl = IConjugatePair<Conjugate, HouseMask, int, Cell, Digit, House, CellMap>;

/// <summary>
/// Represents a <see href="http://sudopedia.enjoysudoku.com/Conjugate_pair.html">conjugate pair</see>.
/// </summary>
/// <remarks>
/// A <b>Conjugate pair</b> is a pair of two candidates, in the same house where all cells has only
/// two position can fill this candidate.
/// </remarks>
/// <param name="mask">Indicates the target mask.</param>
[Equals]
[GetHashCode]
[EqualityOperators]
public readonly partial struct Conjugate([DataMember(MemberKinds.Field)] int mask) : ConjugateImpl
{
	/// <summary>
	/// Initializes a <see cref="Conjugate"/> instance with from and to cell offset and a digit.
	/// </summary>
	/// <param name="from">The from cell.</param>
	/// <param name="to">The to cell.</param>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conjugate(Cell from, Cell to, Digit digit) : this(digit << 20 | from << 10 | to)
	{
	}

	/// <summary>
	/// Initializes a <see cref="Conjugate"/> instance with the map and the digit.
	/// The map should contains two cells, the first one is the start one, and the second one is the end one.
	/// </summary>
	/// <param name="map">The map.</param>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conjugate(scoped in CellMap map, Digit digit) : this(map[0], map[1], digit)
	{
	}


	/// <inheritdoc/>
	public Cell From
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _mask & 1023;
	}

	/// <inheritdoc/>
	public Cell To
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _mask >> 10 & 1023;
	}

	/// <inheritdoc/>
	[HashCodeMember]
	public Digit Digit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _mask >> 20 & 15;
	}

	/// <inheritdoc/>
	public House Line
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Map.CoveredLine;
	}

	/// <inheritdoc/>
	public HouseMask Houses
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Map.CoveredHouses;
	}

	/// <inheritdoc/>
	[HashCodeMember]
	public CellMap Map
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => [From, To];
	}

	/// <inheritdoc/>
	int ConjugateImpl.Mask => _mask;

	private Candidate FromCandidate => From * 9 + Digit;

	private Candidate ToCandidate => To * 9 + Digit;


	[DeconstructionMethod]
	public partial void Deconstruct([DeconstructionMethodArgument(nameof(FromCandidate))] out Candidate fromCand, [DeconstructionMethodArgument(nameof(ToCandidate))] out Candidate toCand);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Conjugate other) => Map == other.Map && Digit == other.Digit;

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => $"{CellsMap[From]} == {CellsMap[To]}({Digit + 1})";
}
