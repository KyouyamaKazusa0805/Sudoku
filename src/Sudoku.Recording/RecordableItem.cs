namespace Sudoku.Recording;

/// <summary>
/// Defines a recordable item. This type is used for recording a user's behavior on finishing a sudoku puzzle.
/// </summary>
[IsLargeStruct]
public readonly struct RecordableItem :
	IEquatable<RecordableItem>,
	IEqualityOperators<RecordableItem, RecordableItem, bool>
{
	/// <summary>
	/// Indicates the inner mask.
	/// </summary>
	/// <remarks>
	/// This field only uses 11 bits. The lower 10 bits to describe the candidate the user operates,
	/// and the 11th bit from lower means which kind of behavior that a user want to operate that candidate
	/// (set or delete). Other bits is reserved for later use. The sketch is like:
	/// <code>
	/// 16   1110        0
	///  |xxxx||         |
	///  |-------|-------|
	/// 16       8       0
	/// </code>
	/// </remarks>
	private readonly short _mask;


	/// <summary>
	/// Initializes a <see cref="RecordableItem"/> instance via the specified recordable type and candidate used.
	/// </summary>
	/// <param name="type">
	/// The recordable type. Indicates which kind of behavior that user want to operate that candidate.
	/// </param>
	/// <param name="candidate">The candidate used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RecordableItem(RecordableItemType type, int candidate) : this((short)((byte)type << 11 | candidate))
	{
	}

	/// <summary>
	/// Initializes a <see cref="RecordableItem"/> instance via the specified recordable type, cell and digit used.
	/// </summary>
	/// <param name="type">
	/// The recordable type. Indicates which kind of behavior that user want to operate that candidate.
	/// </param>
	/// <param name="cell">The cell used.</param>
	/// <param name="digit">The digit used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RecordableItem(RecordableItemType type, int cell, int digit) : this(type, cell * 9 + digit)
	{
	}

	/// <summary>
	/// Initializes a <see cref="RecordableItem"/> instance via the mask.
	/// </summary>
	/// <param name="mask">The inner mask.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private RecordableItem(short mask) => (Timestamp, _mask) = (Stopwatch.GetTimestamp(), mask);


	/// <summary>
	/// Indicates the type of the recording item.
	/// </summary>
	public RecordableItemType Type
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (RecordableItemType)(_mask >> 11);
	}

	/// <summary>
	/// Indicates the cell used.
	/// </summary>
	public int Cell
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidate / 9;
	}

	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public int Digit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidate % 9;
	}

	/// <summary>
	/// Indicates the candidate used. The property value is equivalent
	/// to expression <c><see cref="Cell"/> * 9 + <see cref="Digit"/></c>.
	/// </summary>
	/// <seealso cref="Cell"/>
	/// <seealso cref="Digit"/>
	public int Candidate
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _mask & 1023;
	}

	/// <summary>
	/// Indicates the current timestamp.
	/// </summary>
	public long Timestamp { get; }


	/// <inheritdoc cref="object.Equals(object?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is RecordableItem comparer && Equals(comparer);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(scoped in RecordableItem other) => _mask == other._mask && Timestamp == other.Timestamp;

	/// <inheritdoc cref="object.GetHashCode"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(_mask, Timestamp);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool IEquatable<RecordableItem>.Equals(RecordableItem other) => Equals(other);


	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(scoped in RecordableItem left, scoped in RecordableItem right) => left.Equals(right);

	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(scoped in RecordableItem left, scoped in RecordableItem right) => !(left == right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<RecordableItem, RecordableItem, bool>.operator ==(RecordableItem left, RecordableItem right)
		=> left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<RecordableItem, RecordableItem, bool>.operator !=(RecordableItem left, RecordableItem right)
		=> left != right;
}
