namespace Sudoku.Concepts;

/// <summary>
/// Defines a type that can describe a candidate is the correct or wrong digit.
/// </summary>
/// <param name="mask">
/// The field uses the mask table of length 81 to indicate the state and all possible candidates
/// holding for each cell. Each mask uses a <see cref="Mask"/> value, but only uses 11 of 16 bits.
/// <code>
/// | 16  15  14  13  12  11  10| 9   8   7   6   5   4   3   2   1   0 |
/// |-----------------------|---|---------------------------------------|
/// |   |   |   |   |   |   | 1 | 0 | 1 | 0 | 1 | 0 | 1 | 0 | 1 | 0 | 1 |
/// '-----------------------|---|---------------------------------------'
///                          \_/ \_____________________________________/
///                          (2)                   (1)
/// </code>
/// Where (1) is for candidate offset value (from 0 to 728), and (2) is for the conclusion type (assignment or elimination).
/// Please note that the part (2) only use one bit because the target value can only be assignment (0) or elimination (1), but the real type
/// <see cref="ConclusionType"/> uses <see cref="byte"/> as its underlying numeric type because C# cannot set "A bit"
/// to be the underlying type. The narrowest type is <see cref="byte"/>.
/// </param>
/// <remarks>
/// Two <see cref="Mask"/> values can be compared with each other. If one of those two is an elimination
/// (i.e. holds the value <see cref="Elimination"/> as the type), the instance will be greater;
/// if those two hold same conclusion type, but one of those two holds the global index of the candidate position is greater, it is greater.
/// </remarks>
[JsonConverter(typeof(Converter))]
[TypeImpl(TypeImplFlag.AllObjectMethods | TypeImplFlag.EqualityOperators)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly partial struct Conclusion([PrimaryConstructorParameter(MemberKinds.Field), HashCodeMember] Mask mask) :
	IComparable<Conclusion>,
	IEqualityOperators<Conclusion, Conclusion, bool>,
	IEquatable<Conclusion>,
	IFormattable,
	IParsable<Conclusion>
{
	/// <summary>
	/// The internal parsers.
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static readonly CoordinateParser[] Parsers = [new RxCyParser(), new K9Parser()];


	/// <summary>
	/// Initializes an instance with a conclusion type and a candidate offset.
	/// </summary>
	/// <param name="type">The conclusion type.</param>
	/// <param name="candidate">The candidate offset.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conclusion(ConclusionType type, Candidate candidate) : this((Mask)(((int)type << 10) + candidate))
	{
	}

	/// <summary>
	/// Initializes the <see cref="Conclusion"/> instance via the specified cell, digit and the conclusion type.
	/// </summary>
	/// <param name="type">The conclusion type.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conclusion(ConclusionType type, Cell cell, Digit digit) : this((Mask)(((int)type << 10) + cell * 9 + digit))
	{
	}


	/// <summary>
	/// Indicates the cell the current instance uses.
	/// </summary>
	public Cell Cell
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidate / 9;
	}

	/// <summary>
	/// Indicates the digit the current instance uses.
	/// </summary>
	public Digit Digit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidate % 9;
	}

	/// <summary>
	/// Indicates the candidate the current instance uses.
	/// </summary>
	public Candidate Candidate
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _mask & 1023;
	}

	/// <summary>
	/// Indicates the conclusion type of the current instance.
	/// If the type is <see cref="Assignment"/>, this conclusion will be set value (Set a digit into a cell);
	/// otherwise, a candidate will be removed.
	/// </summary>
	public ConclusionType ConclusionType
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ConclusionType)(_mask >> 10 & 1);
	}


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out ConclusionType conclusionType, out Candidate candidate)
		=> (conclusionType, candidate) = (ConclusionType, Candidate);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out ConclusionType conclusionType, out Cell cell, out Digit digit)
		=> ((conclusionType, _), cell, digit) = (this, Cell, Digit);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Conclusion other) => _mask == other._mask;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Conclusion other) => _mask.CompareTo(_mask);

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(IFormatProvider? formatProvider)
		=> CoordinateConverter.GetConverter(formatProvider).ConclusionConverter(this);

	/// <summary>
	/// Try to get a new <see cref="Conclusion"/> instance which is symmetric with the current instance, with the specified symmetric type.
	/// </summary>
	/// <param name="symmetricType">The symmetric type to be checked.</param>
	/// <param name="mappingDigit">The other mapping digit.</param>
	/// <returns>The other symmetric <see cref="Conclusion"/> value.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="symmetricType"/> contains multiple (greater than 2) cells
	/// symmetric with the current cell and digit.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conclusion GetSymmetricConclusion(SymmetricType symmetricType, Digit mappingDigit)
		=> symmetricType.GetAxisDimension() switch
		{
			0 or 1 => symmetricType.GetCellsInSymmetryAxis().Contains(Cell)
				? new(ConclusionType, Cell, mappingDigit == -1 ? Digit : mappingDigit)
				: new(ConclusionType, (symmetricType.GetCells(Cell) - Cell)[0], mappingDigit == -1 ? Digit : mappingDigit),
			_ => throw new ArgumentOutOfRangeException(nameof(symmetricType))
		};

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);


	/// <inheritdoc cref="IParsable{TSelf}.TryParse(string?, IFormatProvider?, out TSelf)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParse(string str, out Conclusion result) => TryParse(str, null, out result);

	/// <inheritdoc/>
	public static bool TryParse(string? s, IFormatProvider? provider, out Conclusion result)
	{
		if (s is null)
		{
			result = default;
			return false;
		}
		return TryParse(s, out result);
	}

	/// <inheritdoc cref="IParsable{TSelf}.Parse(string, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Conclusion Parse(string str) => Parse(str, null);

	/// <inheritdoc/>
	public static Conclusion Parse(string s, IFormatProvider? provider)
	{
		switch (provider)
		{
			default:
			case null:
			{
				foreach (var parser in Parsers)
				{
					if (parser.ConclusionParser(s) is [var result])
					{
						return result;
					}
				}
				throw new FormatException(SR.ExceptionMessage("StringValueInvalidToBeParsed"));
			}
			case CultureInfo c:
			{
				return CoordinateParser.GetParser(c).ConclusionParser(s)[0];
			}
			case CoordinateParser c:
			{
				return c.ConclusionParser(s)[0];
			}
		}
	}


	/// <summary>
	/// Negates the current conclusion instance, changing the conclusion type from <see cref="Assignment"/> to <see cref="Elimination"/>,
	/// or from <see cref="Elimination"/> to <see cref="Assignment"/>.
	/// </summary>
	/// <param name="self">The current conclusion instance to be negated.</param>
	/// <returns>The negation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Conclusion operator ~(Conclusion self) => new((ConclusionType)(1 & (byte)~self.ConclusionType), self.Candidate);
}

/// <summary>
/// Represents a JSON converter for type <see cref="Conclusion"/>.
/// </summary>
/// <seealso cref="Conclusion"/>
file sealed class Converter : JsonConverter<Conclusion>
{
	/// <inheritdoc/>
	public override bool HandleNull => false;


	/// <inheritdoc/>
	public override Conclusion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> Conclusion.Parse(reader.GetString() ?? string.Empty);

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Conclusion value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString());
}
