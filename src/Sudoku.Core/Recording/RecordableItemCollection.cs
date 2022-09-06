namespace Sudoku.Recording;

/// <summary>
/// Defines a collection of <see cref="RecordableItem"/>s.
/// </summary>
/// <seealso cref="RecordableItem"/>
public readonly struct RecordableItemCollection :
	IReadOnlyCollection<RecordableItem>,
	IReadOnlyList<RecordableItem>,
	IEquatable<RecordableItemCollection>,
	IEqualityOperators<RecordableItemCollection, RecordableItemCollection, bool>,
	IEnumerable<RecordableItem>
{
	/// <summary>
	/// Indicates the inner collection.
	/// </summary>
	private readonly ImmutableArray<RecordableItem> _recordables;


	/// <summary>
	/// Initializes a <see cref="RecordableItemCollection"/> instance
	/// via the specified collection of <see cref="RecordableItem"/> instances.
	/// </summary>
	/// <param name="recordables">The collection of <see cref="RecordableItem"/> instances.</param>
	/// <param name="startTimestamp">Indicates the timestamp that starts the stopwatch to record steps.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private RecordableItemCollection(IEnumerable<RecordableItem> recordables, long startTimestamp)
		=> (_recordables, StartTimestamp) = (ImmutableArray.CreateRange(recordables), startTimestamp);


	/// <summary>
	/// Indicates the number of steps that the current collection being stored.
	/// </summary>
	public int Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _recordables.Length;
	}

	/// <summary>
	/// Indicates the start timestamp. This value records the time information that is ahead of the first step recorded.
	/// </summary>
	public long StartTimestamp { get; }

	/// <summary>
	/// Indicates the total elapsed time that starts the <see cref="StartTimestamp"/> and ends with the final step's
	/// timestamp.
	/// </summary>
	/// <seealso cref="StartTimestamp"/>
	/// <seealso cref="RecordableItem.Timestamp"/>
	public TimeSpan ElapsedTime
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new(_recordables[^1].Timestamp - StartTimestamp);
	}


	/// <summary>
	/// Gets a read-only reference to the element at the specified <paramref name="index"/>
	/// in the read-only collection.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>
	/// A read-only reference to the element at the specified <paramref name="index"/> in the read-only collection.
	/// </returns>
	public ref readonly RecordableItem this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ref _recordables.ItemRef(index);
	}

	/// <inheritdoc/>
	RecordableItem IReadOnlyList<RecordableItem>.this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this[index];
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj)
		=> obj is RecordableItemCollection comparer && Equals(comparer);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(scoped in RecordableItemCollection other) => _recordables == other._recordables;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => _recordables.GetHashCode();

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ImmutableArray<RecordableItem>.Enumerator GetEnumerator() => _recordables.GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool IEquatable<RecordableItemCollection>.Equals(RecordableItemCollection other) => Equals(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_recordables).GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<RecordableItem> IEnumerable<RecordableItem>.GetEnumerator()
		=> ((IEnumerable<RecordableItem>)_recordables).GetEnumerator();


	/// <summary>
	/// Creates a new <see cref="RecordableItemCollection"/> instance
	/// via the specified collection of <see cref="RecordableItem"/> instances.
	/// </summary>
	/// <param name="recordables">The collection of <see cref="RecordableItem"/> instances.</param>
	/// <param name="startTimestamp">The start timestamp value that starts recording each step.</param>
	/// <returns>A <see cref="RecordableItemCollection"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static RecordableItemCollection Create(IEnumerable<RecordableItem> recordables, long startTimestamp)
		=> new(recordables, startTimestamp);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(scoped in RecordableItemCollection left, scoped in RecordableItemCollection right)
		=> left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(scoped in RecordableItemCollection left, scoped in RecordableItemCollection right)
		=> !(left == right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<RecordableItemCollection, RecordableItemCollection, bool>.operator ==(RecordableItemCollection left, RecordableItemCollection right)
		=> left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<RecordableItemCollection, RecordableItemCollection, bool>.operator !=(RecordableItemCollection left, RecordableItemCollection right)
		=> left != right;
}
