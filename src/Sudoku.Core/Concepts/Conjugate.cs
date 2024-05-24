namespace Sudoku.Concepts;

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
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly partial struct Conjugate([PrimaryConstructorParameter(MemberKinds.Field)] ConjugateMask mask) :
	ICoordinateConvertible<Conjugate>,
	ICoordinateParsable<Conjugate>,
	IEquatable<Conjugate>,
	IEqualityOperators<Conjugate, Conjugate, bool>
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
	public Conjugate(ref readonly CellMap map, Digit digit) : this(map[0], map[1], digit)
	{
	}


	/// <summary>
	/// Indicates the "from" cell, i.e. the base cell that starts the conjugate pair.
	/// </summary>
	public Cell From => _mask & 1023;

	/// <summary>
	/// Indicates the "to" cell, i.e. the target cell that ends the conjugate pair.
	/// </summary>
	public Cell To => _mask >> 10 & 1023;

	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	[HashCodeMember]
	public Digit Digit => _mask >> 20 & 15;

	/// <summary>
	/// Indicates the target line of the two cells lie in.
	/// </summary>
	public House Line => Map.SharedLine;

	/// <summary>
	/// Indicates the house that the current conjugate pair lies in.
	/// </summary>
	public HouseMask Houses => Map.SharedHouses;

	/// <summary>
	/// Indicates the cells (the "from" cell and "to" cell).
	/// </summary>
	[HashCodeMember]
	public CellMap Map => [From, To];


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out Candidate fromCand, out Candidate toCand) => (fromCand, toCand) = (From * 9 + Digit, To * 9 + Digit);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Conjugate other) => Map == other.Map && Digit == other.Digit;

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => $"{From.AsCellMap()} == {To.AsCellMap()}({Digit + 1})";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(IFormatProvider? formatProvider)
		=> ToString(GlobalizedConverter.GetConverter(formatProvider as CultureInfo ?? CultureInfo.CurrentUICulture));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString<T>(T converter) where T : CoordinateConverter => converter.ConjugateConverter(this);

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParse(string str, out Conjugate result) => TryParse(str, new RxCyParser(), out result);

	/// <inheritdoc/>
	public static bool TryParse<T>(string str, T parser, out Conjugate result) where T : CoordinateParser
	{
		try
		{
			result = parser.ConjuagteParser(str)[0];
			return true;
		}
		catch (FormatException)
		{
			result = default;
			return false;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Conjugate Parse(string str) => Parse(str, new RxCyParser());

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Conjugate Parse<T>(string str, T parser) where T : CoordinateParser
		=> parser.ConjuagteParser(str) is [var result]
			? result
			: throw new FormatException(ResourceDictionary.ExceptionMessage("MultipleConjugatePairValuesFound"));

	/// <inheritdoc/>
	static bool IParsable<Conjugate>.TryParse(string? s, IFormatProvider? provider, out Conjugate result)
	{
		if (s is null)
		{
			result = default;
			return false;
		}

		return TryParse(s, out result);
	}

	/// <inheritdoc/>
	static Conjugate IParsable<Conjugate>.Parse(string s, IFormatProvider? provider) => Parse(s);
}
