namespace Sudoku.Data;

/// <summary>
/// Defines a collection that only used for <see cref="Cells"/> or <see cref="Candidates"/>.
/// </summary>
/// <typeparam name="TCollection">The type.</typeparam>
/// <seealso cref="Cells"/>
/// <seealso cref="Candidates"/>
public unsafe interface ICellsOrCandidate<TCollection> : IEnumerable<int>, IValueEquatable<TCollection>
	where TCollection : struct, ICellsOrCandidate<TCollection>
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
	void CopyTo(int* arr, int length);

	/// <summary>
	/// Copies the current instance to the target <see cref="Span{T}"/> instance.
	/// </summary>
	/// <param name="span">
	/// The target <see cref="Span{T}"/> instance.
	/// </param>
	void CopyTo(ref Span<int> span);

	/// <summary>
	/// Determine whether the map contains the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	bool Contains(int cell);

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
	/// Get a <typeparamref name="TCollection"/> that contains all <paramref name="left"/> instance
	/// but not in <paramref name="right"/> instance.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	static abstract TCollection operator -(in TCollection left, in TCollection right);

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
}
