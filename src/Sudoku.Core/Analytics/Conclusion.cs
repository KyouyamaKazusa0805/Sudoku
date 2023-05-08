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
/// Indicates the mask that holds the information for the cell, digit and the conclusion type.
/// The bits distribution is like:
/// <code><![CDATA[
/// 16       8       0
///  |-------|-------|
///  |     |---------|
/// 16    10         0
///        |   used  |
/// ]]></code>
/// </param>
[JsonConverter(typeof(JsonConverter))]
public readonly partial struct Conclusion([PrimaryConstructorParameter(MemberKinds.Field)] int mask) :
	IComparable<Conclusion>,
	IComparisonOperators<Conclusion, Conclusion, bool>,
	IEqualityOperators<Conclusion, Conclusion, bool>,
	IEquatable<Conclusion>
{
	/// <summary>
	/// Initializes an instance with a conclusion type and a candidate offset.
	/// </summary>
	/// <param name="type">The conclusion type.</param>
	/// <param name="candidate">The candidate offset.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conclusion(ConclusionType type, Candidate candidate) : this(((int)type << 10) + candidate)
	{
	}

	/// <summary>
	/// Initializes the <see cref="Conclusion"/> instance via the specified cell, digit and the conclusion type.
	/// </summary>
	/// <param name="type">The conclusion type.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conclusion(ConclusionType type, Cell cell, Digit digit) : this(((int)type << 10) + cell * 9 + digit)
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
		get => _mask & (1 << 10) - 1;
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

	private string OutputString => $"{CellsMap[Cell]}{ConclusionType.Notation()}{Digit + 1}";


	[DeconstructionMethod]
	public partial void Deconstruct(out ConclusionType conclusionType, out Candidate candidate);

	[DeconstructionMethod]
	public partial void Deconstruct(out ConclusionType conclusionType, out Cell cell, out Digit digit);

	[GeneratedOverridingMember(GeneratedEqualsBehavior.TypeCheckingAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Conclusion other) => _mask == other._mask;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Conclusion other) => _mask.CompareTo(_mask);

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.SimpleField, "_mask")]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.SimpleMember, nameof(OutputString))]
	public override partial string ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Conclusion left, Conclusion right) => left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Conclusion left, Conclusion right) => !left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >(Conclusion left, Conclusion right) => left.CompareTo(right) > 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >=(Conclusion left, Conclusion right) => left.CompareTo(right) >= 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <(Conclusion left, Conclusion right) => left.CompareTo(right) < 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <=(Conclusion left, Conclusion right) => left.CompareTo(right) <= 0;
}

/// <summary>
/// The file-local type that provides the basic operation for serialization or deserialization for type <see cref="Conclusion"/>.
/// </summary>
file sealed class JsonConverter : JsonConverter<Conclusion>
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
