namespace Sudoku.Analytics;

/// <summary>
/// Defines a type that can describe a candidate is the correct or wrong digit.
/// </summary>
/// <remarks>
/// Two <see cref="Conclusion"/>s can be compared with each other. If one of those two is an elimination
/// (i.e. holds the value <see cref="Elimination"/> as the type), the instance will be greater;
/// if those two hold same conclusion type, but one of those two holds the global index of the candidate position is greater, it is greater.
/// </remarks>
/// <param name="mask"><inheritdoc cref="_mask" path="/summary"/></param>
public readonly partial struct Conclusion(int mask) :
	IComparable<Conclusion>,
	IComparisonOperators<Conclusion, Conclusion, bool>,
	IEqualityOperators<Conclusion, Conclusion, bool>,
	IEquatable<Conclusion>
{
	/// <summary>
	/// Indicates the mask that holds the information for the cell, digit and the conclusion type.
	/// The bits distribution is like:
	/// <code><![CDATA[
	/// 16       8       0
	///  |-------|-------|
	///  |     |---------|
	/// 16    10         0
	///        |   used  |
	/// ]]></code>
	/// </summary>
	private readonly int _mask = mask;


	/// <summary>
	/// Initializes an instance with a conclusion type and a candidate offset.
	/// </summary>
	/// <param name="type">The conclusion type.</param>
	/// <param name="candidate">The candidate offset.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conclusion(ConclusionType type, int candidate) : this(((int)type << 10) + candidate)
	{
	}

	/// <summary>
	/// Initializes the <see cref="Conclusion"/> instance via the specified cell, digit and the conclusion type.
	/// </summary>
	/// <param name="type">The conclusion type.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conclusion(ConclusionType type, int cell, int digit) : this(((int)type << 10) + cell * 9 + digit)
	{
	}


	/// <summary>
	/// Indicates the cell.
	/// </summary>
	public int Cell
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidate / 9;
	}

	/// <summary>
	/// Indicates the digit.
	/// </summary>
	public int Digit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidate % 9;
	}

	/// <summary>
	/// Indicates the candidate.
	/// </summary>
	public int Candidate
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
	public partial void Deconstruct(out ConclusionType conclusionType, out int candidate);

	[DeconstructionMethod]
	public partial void Deconstruct(out ConclusionType conclusionType, out int cell, out int digit);

	/// <summary>
	/// Put this instance into the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ApplyTo(scoped ref Grid grid)
	{
		switch (ConclusionType)
		{
			case Assignment:
			{
				grid[Cell] = Digit;
				break;
			}
			case Elimination:
			{
				grid[Cell, Digit] = false;
				break;
			}
		}
	}

	[GeneratedOverriddingMember(GeneratedEqualsBehavior.TypeCheckingAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Conclusion other) => _mask == other._mask;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Conclusion other) => _mask.CompareTo(_mask);

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.SimpleField, nameof(_mask))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.SimpleMember, nameof(OutputString))]
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
