namespace Sudoku.Concepts;

/// <summary>
/// Represents a <see href="https://sunnieshine.github.io/Sudoku/terms/conjugate-pair">conjugate pair</see>.
/// </summary>
/// <remarks>
/// <para>
/// A <b>Conjugate pair</b> is a pair of two candidates, in the same house where all cells has only
/// two position can fill this candidate.
/// </para>
/// <para>
/// For more information please visit
/// <see href="http://sudopedia.enjoysudoku.com/Conjugate_pair.html">this link</see>.
/// </para>
/// </remarks>
public readonly struct Conjugate :
	IDefaultable<Conjugate>,
	IEquatable<Conjugate>
#if FEATURE_GENERIC_MATH
	,
	IEqualityOperators<Conjugate, Conjugate>
#endif
{
	/// <summary>
	/// <inheritdoc cref="IDefaultable{T}.Default"/>
	/// </summary>
	public static readonly Conjugate Default = new(-1, -1, -1);


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
	public Conjugate(in Cells map, int digit) : this(map[0], map[1], digit)
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

	/// <inheritdoc/>
	bool IDefaultable<Conjugate>.IsDefault
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this == Default;
	}

	/// <inheritdoc/>
	static Conjugate IDefaultable<Conjugate>.Default
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Default;
	}


	/// <inheritdoc cref="object.Equals(object?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) =>
		obj is Conjugate comparer && Equals(comparer);

	/// <summary>
	/// Determine whether the two conjugate pairs are same.
	/// </summary>
	/// <param name="other">The other instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Conjugate other) => Map == other.Map && Digit == other.Digit;

	/// <inheritdoc cref="object.GetHashCode"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(Map, Digit);

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => $"{Cells.Empty + From} == {Cells.Empty + To}({Digit + 1})";


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Conjugate left, Conjugate right) => left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Conjugate left, Conjugate right) => !(left == right);
}
