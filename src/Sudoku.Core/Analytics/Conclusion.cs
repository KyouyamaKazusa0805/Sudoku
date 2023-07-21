namespace Sudoku.Analytics;

/// <summary>
/// Defines a type that can describe a candidate is the correct or wrong digit.
/// </summary>
/// <remarks>
/// Two <see cref="Conclusion"/>s can be compared with each other. If one of those two is an elimination
/// (i.e. holds the value <see cref="Elimination"/> as the type), the instance will be greater;
/// if those two hold same conclusion type, but one of those two holds the global index of the candidate position is greater, it is greater.
/// </remarks>
/// <param name="mask">
/// The field uses the mask table of length 81 to indicate the status and all possible candidates
/// holding for each cell. Each mask uses a <see cref="Mask"/> value, but only uses 11 of 16 bits.
/// <code>
///  | 16  15  14  13  12  11  10| 9   8   7   6   5   4   3   2   1   0 |
///  |-----------------------|---|---------------------------------------|
///  |   |   |   |   |   |   | 1 | 0 | 1 | 0 | 1 | 0 | 1 | 0 | 1 | 0 | 1 |
///  |-----------------------|---|---------------------------------------|
///                           \_/ \_____________________________________/
///                           (2)                   (1)
/// </code>
/// Where (1) is for candidate offset value (from 0 to 728), and (2) is for the conclusion type (assignment or elimination).
/// Please note that the part (2) only use one bit because the target value can only be assignment (0) or elimination (1), but the real type
/// <see cref="Analytics.ConclusionType"/> uses <see cref="byte"/> as its underlying numeric type because C# cannot set "A bit"
/// to be the underlying type. The narrowest type is <see cref="byte"/>.
/// </param>
[JsonConverter(typeof(Converter))]
[Equals]
[GetHashCode]
[ToString]
[EqualityOperators]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly partial struct Conclusion([PrimaryConstructorParameter(MemberKinds.Field), HashCodeMember] Mask mask) :
	IComparable<Conclusion>,
	IEqualityOperators<Conclusion, Conclusion, bool>,
	IEquatable<Conclusion>
{
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
	/// Indicates the cell.
	/// </summary>
	public Cell Cell
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidate / 9;
	}

	/// <summary>
	/// Indicates the digit.
	/// </summary>
	public Digit Digit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidate % 9;
	}

	/// <summary>
	/// Indicates the candidate.
	/// </summary>
	public Candidate Candidate
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _mask & 1023;
	}

	/// <summary>
	/// The conclusion type to control the action of applying.
	/// If the type is <see cref="Assignment"/>, this conclusion will be set value (Set a digit into a cell);
	/// otherwise, a candidate will be removed.
	/// </summary>
	public ConclusionType ConclusionType
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ConclusionType)(_mask >> 10 & 1);
	}

	[StringMember]
	private string OutputString => $"{CellsMap[Cell]}{ConclusionType.Notation()}{Digit + 1}";


	[DeconstructionMethod]
	public partial void Deconstruct(out ConclusionType conclusionType, out Candidate candidate);

	[DeconstructionMethod]
	public partial void Deconstruct(out ConclusionType conclusionType, out Cell cell, out Digit digit);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Conclusion other) => _mask == other._mask;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Conclusion other) => _mask.CompareTo(_mask);


	/// <summary>
	/// Negates the current conclusion instance, changing the conclusion type from <see cref="Assignment"/> to <see cref="Elimination"/>,
	/// or from <see cref="Elimination"/> to <see cref="Assignment"/>.
	/// </summary>
	/// <param name="current">The current conclusion instance to be negated.</param>
	/// <returns>The negation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Conclusion operator ~(Conclusion current)
		=> new(current.ConclusionType == Assignment ? Elimination : Assignment, current.Candidate);
}

/// <summary>
/// The file-local type that provides the basic operation for serialization or deserialization for type <see cref="Conclusion"/>.
/// </summary>
file sealed class Converter : JsonConverter<Conclusion>
{
	/// <inheritdoc/>
	public override Conclusion Read(scoped ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.GetString() switch
		{
			[_, var r, _, var c, .. { Length: 3 }, var d] => new(Assignment, (r - '1') * 9 + (c - '1'), d - '1'),
			[_, var r, _, var c, .. { Length: 4 }, var d] => new(Elimination, (r - '1') * 9 + (c - '1'), d - '1'),
			_ => throw new JsonException()
		};

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Conclusion value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString());
}
