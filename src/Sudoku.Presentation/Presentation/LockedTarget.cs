namespace Sudoku.Presentation;

/// <summary>
/// Defines the data structure that stores a set of cells and a digit, indicating the information
/// about the locked candidate node.
/// </summary>
[IsLargeStruct]
[GeneratedOverloadingOperator(GeneratedOperator.EqualityOperators)]
public readonly partial struct LockedTarget : IEquatable<LockedTarget>, IEqualityOperators<LockedTarget, LockedTarget, bool>
{
	/// <summary>
	/// Initializes a <see cref="LockedTarget"/> instance via the specified cell and the specified digit used.
	/// </summary>
	/// <param name="digit">Indicates the digit used.</param>
	/// <param name="cell">Indicates the cell used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public LockedTarget(int digit, int cell) : this(digit, CellsMap[cell])
	{
	}

	/// <summary>
	/// Initializes a <see cref="LockedTarget"/> instance via the specified cells and the specified digit used.
	/// </summary>
	/// <param name="digit">Indicates the digit used.</param>
	/// <param name="cells">Indicates the cells used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public LockedTarget(int digit, scoped in CellMap cells) => (Digit, Cells) = (digit, cells);


	/// <summary>
	/// Indicates whether the number of cells is 1.
	/// </summary>
	[JsonIgnore]
	public bool IsSole => Cells.Count == 1;

	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public int Digit { get; init; }

	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	public CellMap Cells { get; init; }

	/// <summary>
	/// The digit string value.
	/// </summary>
	[DebuggerHidden]
	[GeneratedDisplayName(nameof(Digit))]
	private int DigitString => Digit + 1;


	[DeconstructionMethod]
	public partial void Deconstruct(out CellMap cells, out int digit);

	[GeneratedOverriddingMember(GeneratedEqualsBehavior.TypeCheckingAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(scoped in LockedTarget other) => Digit == other.Digit && Cells == other.Cells;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Cells), nameof(Digit))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(DigitString), nameof(Cells))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool IEquatable<LockedTarget>.Equals(LockedTarget other) => Equals(other);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<LockedTarget, LockedTarget, bool>.operator ==(LockedTarget left, LockedTarget right) => left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<LockedTarget, LockedTarget, bool>.operator !=(LockedTarget left, LockedTarget right) => left != right;
}
