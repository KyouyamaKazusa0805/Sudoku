namespace Sudoku.Data;

/// <summary>
/// Provides with a basic collection that is a <see cref="Cells"/> or a <see cref="Candidates"/> collection.
/// </summary>
/// <typeparam name="TCollection">
/// The type. The type should always be <see cref="Cells"/> or <see cref="Candidates"/>.
/// </typeparam>
/// <seealso cref="Cells"/>
/// <seealso cref="Candidates"/>
internal interface ICellsOrCandidates<TCollection>
: IEnumerable<int>
, IValueEquatable<TCollection>
where TCollection : struct, ICellsOrCandidates<TCollection>
{
	/// <summary>
	/// Indicates whether the collection is empty.
	/// </summary>
	bool IsEmpty { get; }

	/// <summary>
	/// Indicates the number of the values stored in this collection.
	/// </summary>
	int Count { get; }

	/// <summary>
	/// Indicates the <typeparamref name="TCollection"/> of intersections.
	/// </summary>
	TCollection PeerIntersection { get; }


	/// <summary>
	/// Get the offset at the specified position index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>
	/// The offset at the specified position index. If the value is invalid, the return value will be <c>-1</c>.
	/// </returns>
	int this[int index] { get; }


	/// <summary>
	/// Copies the current instance to the target array specified as an <see cref="int"/>*.
	/// </summary>
	/// <param name="arr">The pointer that points to an array of type <see cref="int"/>.</param>
	/// <param name="length">The length of that array.</param>
	/// <exception cref="InvalidOperationException">
	/// Throws when the capacity isn't enough to store all values.
	/// </exception>
	unsafe void CopyTo(int* arr, int length);

	/// <summary>
	/// Copies the current instance to the target <see cref="Span{T}"/> instance.
	/// </summary>
	/// <param name="span">
	/// The target <see cref="Span{T}"/> instance.
	/// </param>
	void CopyTo(ref Span<int> span);

	/// <summary>
	/// Determine whether the map contains the specified offset.
	/// </summary>
	/// <param name="offset">The offset.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	bool Contains(int offset);

	/// <summary>
	/// Get all offsets whose bits are set <see langword="true"/>.
	/// </summary>
	/// <returns>An array of offsets.</returns>
	int[] ToArray();

	/// <inheritdoc cref="object.ToString"/>
	string ToString();

	/// <summary>
	/// Set the specified offset as <see langword="true"/> or <see langword="false"/> value.
	/// </summary>
	/// <param name="offset">
	/// The offset. This value can be positive and negative. If 
	/// negative, the offset will be assigned <see langword="false"/>
	/// into the corresponding bit position of its absolute value.
	/// </param>
	void Add(int offset);

	/// <summary>
	/// Set the specified offset as <see langword="true"/> value.
	/// </summary>
	/// <param name="offset">The offset.</param>
	/// <remarks>
	/// Different with <see cref="Add(int)"/>, the method will process negative values,
	/// but this won't.
	/// </remarks>
	/// <seealso cref="Add(int)"/>
	void AddAnyway(int offset);

	/// <summary>
	/// Set the specified offsets as <see langword="true"/> value.
	/// </summary>
	/// <param name="offsets">The offsets to add.</param>
	void AddRange(in ReadOnlySpan<int> offsets);

	/// <summary>
	/// Set the specified offsets as <see langword="true"/> value.
	/// </summary>
	/// <param name="offsets">The offsets to add.</param>
	void AddRange(IEnumerable<int> offsets);

	/// <summary>
	/// Set the specified offset as <see langword="false"/> value.
	/// </summary>
	/// <param name="offset">The offset.</param>
	/// <remarks>
	/// Different with <see cref="Add(int)"/>, this method <b>can't</b> receive the negative value as the parameter.
	/// </remarks>
	/// <seealso cref="Add(int)"/>
	void Remove(int offset);

	/// <summary>
	/// Clear all bits.
	/// </summary>
	void Clear();

	/// <summary>
	/// Converts the current instance to a <see cref="Span{T}"/> of type <see cref="int"/>.
	/// </summary>
	/// <returns>The <see cref="Span{T}"/> of <see cref="int"/> result.</returns>
	Span<int> ToSpan();

	/// <summary>
	/// Converts the current instance to a <see cref="ReadOnlySpan{T}"/> of type <see cref="int"/>.
	/// </summary>
	/// <returns>The <see cref="ReadOnlySpan{T}"/> of <see cref="int"/> result.</returns>
	ReadOnlySpan<int> ToReadOnlySpan();


	/// <summary>
	/// Reverse status for all offsets, which means all <see langword="true"/> bits
	/// will be set <see langword="false"/>, and all <see langword="false"/> bits
	/// will be set <see langword="true"/>.
	/// </summary>
	/// <param name="offsets">The instance to negate.</param>
	/// <returns>The negative result.</returns>
	static abstract TCollection operator ~(in TCollection offsets);

	/// <summary>
	/// The syntactic sugar for <c>!(<paramref name="left"/> - <paramref name="right"/>).IsEmpty</c>.
	/// </summary>
	/// <param name="left">The subtrahend.</param>
	/// <param name="right">The subtractor.</param>
	/// <returns>The <see cref="bool"/> value indicating that.</returns>
	static abstract bool operator >(in TCollection left, in TCollection right);

	/// <summary>
	/// The syntactic sugar for <c>(<paramref name="left"/> - <paramref name="right"/>).IsEmpty</c>.
	/// </summary>
	/// <param name="left">The subtrahend.</param>
	/// <param name="right">The subtractor.</param>
	/// <returns>The <see cref="bool"/> value indicating that.</returns>
	static abstract bool operator <(in TCollection left, in TCollection right);

	/// <summary>
	/// Get a <typeparamref name="TCollection"/> that contains all <paramref name="left"/> instance
	/// but not in <paramref name="right"/> instance.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	static abstract TCollection operator -(in TCollection left, in TCollection right);

	/// <summary>
	/// <para>
	/// Adds the specified <paramref name="offset"/> to the <paramref name="collection"/>,
	/// and returns the added result.
	/// </para>
	/// <para>
	/// The operator is same as the expression <c>new(collection) { cell }</c>, but with optimization.
	/// </para>
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <param name="offset">The offset to be removed.</param>
	/// <returns>The result collection.</returns>
	static abstract TCollection operator +(TCollection collection, int offset);

	/// <summary>
	/// <para>
	/// Removes the specified <paramref name="offset"/> from the <paramref name="collection"/>,
	/// and returns the removed result.
	/// </para>
	/// <para>
	/// The operator is same as the expression <c>new(collection) { ~cell }</c>, but with optimization.
	/// </para>
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <param name="offset">The offset to be removed.</param>
	/// <returns>The result collection.</returns>
	static abstract TCollection operator -(TCollection collection, int offset);

	/// <summary>
	/// Get all cells that two <typeparamref name="TCollection"/> instances both contain.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The intersection result.</returns>
	static abstract TCollection operator &(in TCollection left, in TCollection right);

	/// <summary>
	/// Get all cells from two <typeparamref name="TCollection"/> instances.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The union result.</returns>
	static abstract TCollection operator |(in TCollection left, in TCollection right);

	/// <summary>
	/// Get all cells that only appears once in two <typeparamref name="TCollection"/> instances.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The symmetrical difference result.</returns>
	static abstract TCollection operator ^(in TCollection left, in TCollection right);

	/// <summary>
	/// Simply calls <c><![CDATA[(a & b).PeerIntersection & b]]></c>.
	/// The operator is used for searching for and checking eliminations.
	/// </summary>
	/// <param name="base">The base map.</param>
	/// <param name="template">The template map that the base map to check and cover.</param>
	/// <returns>The result map.</returns>
	/// <seealso cref="PeerIntersection"/>
	static abstract TCollection operator %(in TCollection @base, in TCollection template);


	/// <summary>
	/// Implicit cast from <see cref="int"/>[] to <typeparamref name="TCollection"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	static abstract implicit operator TCollection(int[] offsets);

	/// <summary>
	/// Implicit cast from <see cref="Span{T}"/> to <typeparamref name="TCollection"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	static abstract implicit operator TCollection(in Span<int> offsets);

	/// <summary>
	/// Implicit cast from <see cref="ReadOnlySpan{T}"/> to <typeparamref name="TCollection"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	static abstract implicit operator TCollection(in ReadOnlySpan<int> offsets);

	/// <summary>
	/// Explicit cast from <typeparamref name="TCollection"/> to <see cref="int"/>[].
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	static abstract explicit operator int[](in TCollection offsets);

	/// <summary>
	/// Explicit cast from <typeparamref name="TCollection"/> to <see cref="Span{T}"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	static abstract explicit operator Span<int>(in TCollection offsets);

	/// <summary>
	/// Explicit cast from <typeparamref name="TCollection"/> to <see cref="ReadOnlySpan{T}"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	static abstract explicit operator ReadOnlySpan<int>(in TCollection offsets);
}
