namespace Sudoku.Concepts;

/// <summary>
/// Extracts a base type that describes status table from elements of <typeparamref name="TSelf"/> type.
/// </summary>
/// <typeparam name="TSelf">The type of the instance that implements this interface type.</typeparam>
public interface IBitStatusMap<TSelf> :
	IAdditiveIdentity<TSelf, TSelf>,
	IBitwiseOperators<TSelf, TSelf, TSelf>,
	ICollection<int>,
	IEnumerable,
	IEnumerable<int>,
	IEquatable<TSelf>,
	IEqualityOperators<TSelf, TSelf, bool>,
	IFormattable,
	IMinMaxValue<TSelf>,
	IModulusOperators<TSelf, TSelf, TSelf>,
	IParsable<TSelf>,
	IReadOnlyCollection<int>,
	IReadOnlyList<int>,
	IReadOnlySet<int>,
	ISet<int>,
	ISelectClauseProvider<int>,
	ISimpleFormattable,
	ISimpleParsable<TSelf>,
	ISubtractionOperators<TSelf, TSelf, TSelf>
	where TSelf : unmanaged, IBitStatusMap<TSelf>
{
	/// <summary>
	/// Indicates the size of each unit.
	/// </summary>
	int Shifting { get; }

	/// <summary>
	/// Indicates the number of the values stored in this collection.
	/// </summary>
	new int Count { get; }

	/// <summary>
	/// Gets all chunks of the current collection, meaning a list of <see cref="string"/> values that can describe
	/// all cell and candidate indices, grouped with same row/column.
	/// </summary>
	string[] StringChunks { get; }

	/// <summary>
	/// Indicates the peer intersection of the current instance.
	/// </summary>
	/// <remarks>
	/// A <b>Peer Intersection</b> is a set of cells that all cells from the base collection can be seen.
	/// For more information please visit <see href="https://sunnieshine.github.io/Sudoku/terms/peer">this link</see>.
	/// </remarks>
	TSelf PeerIntersection { get; }

	/// <summary>
	/// Indicates the cell offsets in this collection.
	/// </summary>
	protected abstract int[] Offsets { get; }

	/// <inheritdoc/>
	bool ICollection<int>.IsReadOnly => false;

	/// <inheritdoc/>
	int ICollection<int>.Count => Count;


	/// <summary>
	/// Indicates the empty instance.
	/// </summary>
	static abstract TSelf Empty { get; }


	/// <summary>
	/// Get the offset at the specified position index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>
	/// The offset at the specified position index. If the value is invalid, the return value will be <c>-1</c>.
	/// </returns>
	new int this[int index] { get; }

	/// <inheritdoc/>
	int IReadOnlyList<int>.this[int index] => this[index];


	/// <inheritdoc/>
	static TSelf IAdditiveIdentity<TSelf, TSelf>.AdditiveIdentity => TSelf.Empty;


	/// <summary>
	/// Adds a new offset into the current collection.
	/// </summary>
	/// <param name="offset">The offset.</param>
	new void Add(int offset);

	/// <summary>
	/// Set the specified offset as <see langword="true"/> value, with range check.
	/// </summary>
	/// <param name="offset">The offset.</param>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the specified offset is invalid.</exception>
	void AddChecked(int offset);

	/// <summary>
	/// Set the specified offsets as <see langword="true"/> value.
	/// </summary>
	/// <param name="offsets">The offsets to add.</param>
	sealed void AddRange(scoped ReadOnlySpan<int> offsets)
	{
		foreach (var cell in offsets)
		{
			Add(cell);
		}
	}

	/// <inheritdoc cref="AddRange(ReadOnlySpan{int})"/>
	void AddRange(IEnumerable<int> offsets);

	/// <inheritdoc cref="AddRange(ReadOnlySpan{int})"/>
	/// <remarks>
	/// Different with the method <see cref="AddRange(IEnumerable{int})"/>, this method
	/// also checks for the validity of each offsets.
	/// If the value is below 0 or greater than 80 (for cell offsets) or below 0 or greater than 728 (for candidate offsets),
	/// this method will throw an exception to report about this.
	/// </remarks>
	/// <exception cref="InvalidOperationException">Throws when found at least one cell offset invalid.</exception>
	void AddRangeChecked(IEnumerable<int> offsets);

	/// <summary>
	/// Set the specified offset as <see langword="false"/> value.
	/// </summary>
	/// <param name="offset">The offset.</param>
	new void Remove(int offset);

	/// <summary>
	/// Set the specified offset as <see langword="false"/> value, with range check.
	/// </summary>
	/// <param name="offset">The offset.</param>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the specified offset is invalid.</exception>
	void RemoveChecked(int offset);

	/// <summary>
	/// Set the specified offsets as <see langword="false"/> value.
	/// </summary>
	/// <param name="offsets">The offsets to remove.</param>
	sealed void RemoveRange(scoped ReadOnlySpan<int> offsets)
	{
		foreach (var cell in offsets)
		{
			Remove(cell);
		}
	}

	/// <inheritdoc cref="RemoveRange(ReadOnlySpan{int})"/>
	/// <remarks>
	/// Different with the method <see cref="RemoveRange(IEnumerable{int})"/>, this method
	/// also checks for the validity of each offsets.
	/// If the value is below 0 or greater than 80 (for cell offsets) or below 0 or greater than 728 (for candidate offsets),
	/// this method will throw an exception to report about this.
	/// </remarks>
	/// <exception cref="InvalidOperationException">Throws when found at least one cell offset invalid.</exception>
	void RemoveRange(IEnumerable<int> offsets);

	/// <inheritdoc cref="RemoveRange(ReadOnlySpan{int})"/>
	/// <remarks>
	/// Different with the method <see cref="AddRange(IEnumerable{int})"/>, this method
	/// also checks for the validity of each offsets.
	/// If the value is below 0 or greater than 80 (for cell offsets) or below 0 or greater than 728 (for candidate offsets),
	/// this method will throw an exception to report about this.
	/// </remarks>
	/// <exception cref="InvalidOperationException">Throws when found at least one cell offset invalid.</exception>
	void RemoveRangeChecked(IEnumerable<int> offsets);

	/// <summary>
	/// Clear all bits.
	/// </summary>
	new void Clear();

	/// <summary>
	/// Copies the current instance to the target array specified as an <see cref="int"/>*.
	/// </summary>
	/// <param name="arr">The pointer that points to an array of type <see cref="int"/>.</param>
	/// <param name="length">The length of that array.</param>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="arr"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	/// Throws when the capacity isn't enough to store all values.
	/// </exception>
	unsafe void CopyTo(int* arr, int length);

	/// <inheritdoc cref="ICollection{T}.CopyTo(T[], int)"/>
	new unsafe void CopyTo(int[] array, int arrayIndex)
	{
		fixed (int* pArray = array)
		{
			CopyTo(pArray + arrayIndex, Count - arrayIndex);
		}
	}

	/// <summary>
	/// Copies the current instance to the target <see cref="Span{T}"/> instance.
	/// </summary>
	/// <param name="span">
	/// The target <see cref="Span{T}"/> instance.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	sealed unsafe void CopyTo(scoped Span<int> span)
	{
		fixed (int* ptr = span)
		{
			CopyTo(ptr, span.Length);
		}
	}

	/// <summary>
	/// Iterates on each element in this collection.
	/// </summary>
	/// <param name="action">The visitor that handles for each element in this collection.</param>
	void ForEach(Action<int> action);

	/// <inheritdoc cref="ISet{T}.ExceptWith(IEnumerable{T})"/>
	new void ExceptWith(IEnumerable<int> other);

	/// <inheritdoc cref="ISet{T}.IntersectWith(IEnumerable{T})"/>
	new void IntersectWith(IEnumerable<int> other);

	/// <inheritdoc cref="ISet{T}.SymmetricExceptWith(IEnumerable{T})"/>
	new void SymmetricExceptWith(IEnumerable<int> other);

	/// <inheritdoc cref="ISet{T}.UnionWith(IEnumerable{T})"/>
	new void UnionWith(IEnumerable<int> other);

	/// <summary>
	/// Determine whether the map contains the specified offset.
	/// </summary>
	/// <param name="offset">The offset.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	new bool Contains(int offset);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	bool Equals(scoped in TSelf other);

	/// <summary>
	/// Get all offsets whose bits are set <see langword="true"/>.
	/// </summary>
	/// <returns>An array of offsets.</returns>
	int[] ToArray();

	/// <summary>
	/// Slices the current instance, and get the new instance with some of elements between two indices.
	/// </summary>
	/// <param name="start">The start index.</param>
	/// <param name="count">The number of elements.</param>
	/// <returns>The target instance.</returns>
	TSelf Slice(int start, int count);

	/// <summary>
	/// Gets the enumerator of the current instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	new OneDimensionalArrayEnumerator<int> GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void ICollection<int>.Clear() => Clear();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void ICollection<int>.CopyTo(int[] array, int arrayIndex) => CopyTo(array, arrayIndex);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void ISet<int>.ExceptWith(IEnumerable<int> other) => ExceptWith(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void ISet<int>.IntersectWith(IEnumerable<int> other) => IntersectWith(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void ISet<int>.SymmetricExceptWith(IEnumerable<int> other) => SymmetricExceptWith(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void ISet<int>.UnionWith(IEnumerable<int> other) => UnionWith(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool ISet<int>.Add(int item)
	{
		Add(item);
		return true;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool ICollection<int>.Remove(int item)
	{
		Remove(item);
		return true;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool IReadOnlySet<int>.Contains(int item) => Contains(item);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool IEquatable<TSelf>.Equals(TSelf other) => Equals(other);

	#region Not fully tested
	/// <inheritdoc cref="ISet{T}.IsProperSubsetOf(IEnumerable{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	new sealed bool IsProperSubsetOf(IEnumerable<int> other)
	{
		var otherCells = TSelf.Empty + other;
		return (TSelf)this != otherCells && (otherCells & (TSelf)this) == (TSelf)this;
	}

	/// <inheritdoc cref="ISet{T}.IsProperSupersetOf(IEnumerable{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	new sealed bool IsProperSupersetOf(IEnumerable<int> other)
	{
		var otherCells = TSelf.Empty + other;
		return (TSelf)this != otherCells && ((TSelf)this & otherCells) == otherCells;
	}

	/// <inheritdoc cref="ISet{T}.IsSubsetOf(IEnumerable{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	new sealed bool IsSubsetOf(IEnumerable<int> other) => ((TSelf.Empty + other) & (TSelf)this) == (TSelf)this;

	/// <inheritdoc cref="ISet{T}.IsSupersetOf(IEnumerable{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	new sealed bool IsSupersetOf(IEnumerable<int> other)
	{
		var otherCells = TSelf.Empty + other;
		return ((TSelf)this & otherCells) == otherCells;
	}

	/// <inheritdoc cref="ISet{T}.Overlaps(IEnumerable{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	new sealed bool Overlaps(IEnumerable<int> other) => ((TSelf)this & (TSelf.Empty + other)) is not [];

	/// <inheritdoc cref="ISet{T}.SetEquals(IEnumerable{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	new sealed bool SetEquals(IEnumerable<int> other) => (TSelf)this == TSelf.Empty + other;
	#endregion

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool ISet<int>.IsProperSubsetOf(IEnumerable<int> other) => IsProperSubsetOf(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool ISet<int>.IsProperSupersetOf(IEnumerable<int> other) => IsProperSupersetOf(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool ISet<int>.IsSubsetOf(IEnumerable<int> other) => IsSubsetOf(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool ISet<int>.IsSupersetOf(IEnumerable<int> other) => IsSupersetOf(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool ISet<int>.Overlaps(IEnumerable<int> other) => Overlaps(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool ISet<int>.SetEquals(IEnumerable<int> other) => SetEquals(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool IReadOnlySet<int>.IsProperSubsetOf(IEnumerable<int> other) => IsProperSubsetOf(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool IReadOnlySet<int>.IsProperSupersetOf(IEnumerable<int> other) => IsProperSupersetOf(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool IReadOnlySet<int>.IsSubsetOf(IEnumerable<int> other) => IsSubsetOf(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool IReadOnlySet<int>.IsSupersetOf(IEnumerable<int> other) => IsSupersetOf(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool IReadOnlySet<int>.Overlaps(IEnumerable<int> other) => Overlaps(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool IReadOnlySet<int>.SetEquals(IEnumerable<int> other) => SetEquals(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<int>)this).GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<int> IEnumerable<int>.GetEnumerator()
	{
		var collection = new List<int>();
		foreach (var element in this)
		{
			collection.Add(element);
		}

		return collection.GetEnumerator();
	}

	/// <inheritdoc/>
	IEnumerable<TResult> ISelectClauseProvider<int>.Select<TResult>(Func<int, TResult> selector)
	{
		var result = new TResult[Count];
		var i = 0;
		foreach (var cell in this)
		{
			result[i++] = selector(cell);
		}

		return ImmutableArray.Create(result);
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static TSelf IParsable<TSelf>.Parse(string s, IFormatProvider? provider) => TSelf.Parse(s);


	/// <summary>
	/// Determines whether the current collection is empty.
	/// </summary>
	/// <param name="offsets">The cells to be checked.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	/// <remarks>
	/// The type of the current collection supports using <see cref="bool"/>-like expression to determine whether the collection is not empty,
	/// for example:
	/// <code><![CDATA[
	/// if (collection)
	///     // ...
	/// ]]></code>
	/// The statement <c>collection</c> will be expanded to <c>collection.Count != 0</c>. Therefore, the negation operator <c>!</c>
	/// will invert the result of above expression. This is why I use <see langword="operator"/> <c>!</c> to determine on this.
	/// </remarks>
	static abstract bool operator !(scoped in TSelf offsets);

	/// <summary>
	/// Reverse status for all offsets, which means all <see langword="true"/> bits
	/// will be set <see langword="false"/>, and all <see langword="false"/> bits
	/// will be set <see langword="true"/>.
	/// </summary>
	/// <param name="offsets">The instance to negate.</param>
	/// <returns>The negative result.</returns>
	static abstract TSelf operator ~(scoped in TSelf offsets);

	/// <summary>
	/// Determines whether the specified <typeparamref name="TSelf"/> collection is not empty.
	/// </summary>
	/// <param name="cells">The collection.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static virtual bool operator true(scoped in TSelf cells) => cells.Count != 0;

	/// <summary>
	/// Determines whether the specified <typeparamref name="TSelf"/> collection is empty.
	/// </summary>
	/// <param name="cells">The collection.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static virtual bool operator false(scoped in TSelf cells) => cells.Count == 0;

	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static virtual bool operator ==(scoped in TSelf left, scoped in TSelf right) => left.Equals(right);

	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static virtual bool operator !=(scoped in TSelf left, scoped in TSelf right) => !(left == right);

	/// <summary>
	/// Adds the specified <paramref name="offset"/> to the <paramref name="collection"/>,
	/// and returns the added result.
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <param name="offset">The offset to be added.</param>
	/// <returns>The result collection.</returns>
	static abstract TSelf operator +(scoped in TSelf collection, int offset);

	/// <summary>
	/// Adds the specified <paramref name="offset"/> to the <paramref name="collection"/>,
	/// and returns the added result.
	/// This operator will check the validity of the argument <paramref name="offset"/>.
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <param name="offset">The offset to be added.</param>
	/// <returns>The result collection.</returns>
	static abstract TSelf operator checked +(scoped in TSelf collection, int offset);

	/// <summary>
	/// Adds the specified list of <paramref name="offsets"/> to the <paramref name="collection"/>,
	/// and returns the added result.
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <param name="offsets">A list of cells to be added.</param>
	/// <returns>The result collection.</returns>
	static abstract TSelf operator +(scoped in TSelf collection, IEnumerable<int> offsets);

	/// <summary>
	/// Adds the specified list of <paramref name="offsets"/> to the <paramref name="collection"/>,
	/// and returns the added result.
	/// This operator will check the validity of the argument <paramref name="offsets"/>.
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <param name="offsets">A list of cells to be added.</param>
	/// <returns>The result collection.</returns>
	static abstract TSelf operator checked +(scoped in TSelf collection, IEnumerable<int> offsets);

	/// <summary>
	/// Removes the specified <paramref name="offset"/> from the <paramref name="collection"/>,
	/// and returns the removed result.
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <param name="offset">The offset to be removed.</param>
	/// <returns>The result collection.</returns>
	static abstract TSelf operator -(scoped in TSelf collection, int offset);

	/// <summary>
	/// Removes the specified <paramref name="offset"/> from the <paramref name="collection"/>,
	/// and returns the removed result. This operator will check the validity of the argument <paramref name="offset"/>.
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <param name="offset">The offset to be removed.</param>
	/// <returns>The result collection.</returns>
	static abstract TSelf operator checked -(scoped in TSelf collection, int offset);

	/// <summary>
	/// Get a <typeparamref name="TSelf"/> that contains all <paramref name="collection"/> instance
	/// but not in <paramref name="offsets"/> instance.
	/// </summary>
	/// <param name="collection">The left instance.</param>
	/// <param name="offsets">The right instance.</param>
	/// <returns>The result.</returns>
	static abstract TSelf operator -(scoped in TSelf collection, IEnumerable<int> offsets);

	/// <summary>
	/// Get a <typeparamref name="TSelf"/> that contains all <paramref name="collection"/> instance
	/// but not in <paramref name="offsets"/> instance.
	/// This operator will check the validity of each element in the argument <paramref name="offsets"/>.
	/// </summary>
	/// <param name="collection">The left instance.</param>
	/// <param name="offsets">The right instance.</param>
	/// <returns>The result.</returns>
	static abstract TSelf operator checked -(scoped in TSelf collection, IEnumerable<int> offsets);

	/// <summary>
	/// Get a <typeparamref name="TSelf"/> that contains all <paramref name="left"/> instance
	/// but not in <paramref name="right"/> instance.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	static virtual TSelf operator -(scoped in TSelf left, scoped in TSelf right) => left & ~right;

	/// <summary>
	/// Get the elements that both <paramref name="left"/> and <paramref name="right"/> contain.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	static abstract TSelf operator &(scoped in TSelf left, scoped in TSelf right);

	/// <summary>
	/// Combine the elements from <paramref name="left"/> and <paramref name="right"/>,
	/// and return the merged result.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	static abstract TSelf operator |(scoped in TSelf left, scoped in TSelf right);

	/// <summary>
	/// Get the elements that either <paramref name="left"/> or <paramref name="right"/> contains.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	static abstract TSelf operator ^(scoped in TSelf left, scoped in TSelf right);

	/// <summary>
	/// Gets the subsets of the current collection via the specified size indicating the number of elements of the each subset.
	/// </summary>
	/// <param name="offsets">Indicates the base template cells.</param>
	/// <param name="subsetSize">The size to get.</param>
	/// <returns>
	/// All possible subsets. If:
	/// <list type="table">
	/// <listheader>
	/// <term>Condition</term>
	/// <description>Meaning</description>
	/// </listheader>
	/// <item>
	/// <term><c><paramref name="subsetSize"/> &gt; <paramref name="offsets"/>.Count</c></term>
	/// <description>Will return an empty array</description>
	/// </item>
	/// <item>
	/// <term><c><paramref name="subsetSize"/> == <paramref name="offsets"/>.Count</c></term>
	/// <description>
	/// Will return an array that contains only one element, same as the argument <paramref name="offsets"/>.
	/// </description>
	/// </item>
	/// <item>
	/// <term>Other cases</term>
	/// <description>The valid combinations.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <exception cref="NotSupportedException">
	/// Throws when both <paramref name="offsets"/> count and <paramref name="subsetSize"/> are greater than 30.
	/// </exception>
	/// <remarks>
	/// For example, if the argument <paramref name="offsets"/> is <c>r1c1</c>, <c>r1c2</c> and <c>r1c3</c>
	/// and the argument <paramref name="subsetSize"/> is 2, the expression <c><![CDATA[cells & 2]]></c>
	/// will be an array of 3 elements given below: <c>r1c12</c>, <c>r1c13</c> and <c>r1c23</c>.
	/// </remarks>
	static abstract TSelf[] operator &(scoped in TSelf offsets, int subsetSize);

	/// <summary>
	/// Gets all subsets of the current collection via the specified size
	/// indicating the <b>maximum</b> number of elements of the each subset.
	/// </summary>
	/// <param name="offsets">Indicates the base template cells.</param>
	/// <param name="subsetSize">The size to get.</param>
	/// <returns>
	/// All possible subsets. If:
	/// <list type="table">
	/// <listheader>
	/// <term>Condition</term>
	/// <description>Meaning</description>
	/// </listheader>
	/// <item>
	/// <term><c><paramref name="subsetSize"/> &gt; <paramref name="offsets"/>.Count</c></term>
	/// <description>Will return an empty array</description>
	/// </item>
	/// <item>
	/// <term>Other cases</term>
	/// <description>The valid combinations.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <remarks>
	/// For example, the expression <c>cells | 3</c> is equivalent to all possible cases
	/// coming from <c><![CDATA[cells & 1]]></c>,
	/// <c><![CDATA[cells & 2]]></c> and <c><![CDATA[cells & 3]]></c>.
	/// </remarks>
	static virtual TSelf[] operator |(scoped in TSelf offsets, int subsetSize)
	{
		if (subsetSize == 0 || offsets is [])
		{
			return Array.Empty<TSelf>();
		}

		var n = offsets.Count;
		var desiredSize = 0;
		var length = Min(n, subsetSize);
		for (var i = 1; i <= length; i++)
		{
			var target = Combinatorial[n - 1, i - 1];
			desiredSize += target;
		}

		var result = new List<TSelf>(desiredSize);
		for (var i = 1; i <= length; i++)
		{
			result.AddRange(offsets & i);
		}

		return result.ToArray();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<TSelf, TSelf, bool>.operator ==(TSelf left, TSelf right) => left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<TSelf, TSelf, bool>.operator !=(TSelf left, TSelf right) => left != right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static TSelf IBitwiseOperators<TSelf, TSelf, TSelf>.operator ~(TSelf value) => ~value;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static TSelf ISubtractionOperators<TSelf, TSelf, TSelf>.operator -(TSelf left, TSelf right) => left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static TSelf IBitwiseOperators<TSelf, TSelf, TSelf>.operator &(TSelf left, TSelf right) => left & right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static TSelf IBitwiseOperators<TSelf, TSelf, TSelf>.operator |(TSelf left, TSelf right) => left | right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static TSelf IBitwiseOperators<TSelf, TSelf, TSelf>.operator ^(TSelf left, TSelf right) => left ^ right;

	/// <summary>
	/// Expands the operator to <c><![CDATA[(a & b).PeerIntersection & b]]></c>.
	/// </summary>
	/// <param name="left">The base map.</param>
	/// <param name="right">The template map that the base map to check and cover.</param>
	/// <returns>The result map.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static TSelf IModulusOperators<TSelf, TSelf, TSelf>.operator %(TSelf left, TSelf right) => (left & right).PeerIntersection & right;


	/// <summary>
	/// Implicit cast from <typeparamref name="TSelf"/> to <see cref="int"/>[].
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	static virtual implicit operator int[](scoped in TSelf offsets) => offsets.ToArray();

	/// <summary>
	/// Implicit cast from <see cref="Span{T}"/> to <typeparamref name="TSelf"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	static virtual implicit operator TSelf(scoped Span<int> offsets)
	{
		var result = TSelf.Empty;
		foreach (var offset in offsets)
		{
			result.Add(offset);
		}

		return result;
	}

	/// <summary>
	/// Implicit cast from <see cref="ReadOnlySpan{T}"/> to <typeparamref name="TSelf"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	static virtual implicit operator TSelf(scoped ReadOnlySpan<int> offsets)
	{
		var result = TSelf.Empty;
		foreach (var offset in offsets)
		{
			result.Add(offset);
		}

		return result;
	}

	/// <summary>
	/// Explicit cast from <see cref="int"/>[] to <typeparamref name="TSelf"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	static virtual explicit operator TSelf(int[] offsets)
	{
		var result = TSelf.Empty;
		foreach (var offset in offsets)
		{
			result.Add(offset);
		}

		return result;
	}

	/// <summary>
	/// Explicit cast from <see cref="int"/>[] to <typeparamref name="TSelf"/>, with cell range check.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when a certain element in the argument <paramref name="offsets"/>
	/// is not a valid value to represent a cell offset.
	/// </exception>
	static virtual explicit operator checked TSelf(int[] offsets)
	{
		var result = TSelf.Empty;
		foreach (var offset in offsets)
		{
			result.AddChecked(offset);
		}

		return result;
	}

	/// <summary>
	/// Explicit cast from <typeparamref name="TSelf"/> to <see cref="Span{T}"/> of element type <see cref="int"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	static virtual explicit operator Span<int>(scoped in TSelf offsets) => offsets.ToArray();

	/// <summary>
	/// Explicit cast from <typeparamref name="TSelf"/> to <see cref="ReadOnlySpan{T}"/> of element type <see cref="int"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	static virtual explicit operator ReadOnlySpan<int>(scoped in TSelf offsets) => offsets.ToArray();
}
