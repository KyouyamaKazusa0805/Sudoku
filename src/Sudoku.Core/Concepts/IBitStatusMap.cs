namespace Sudoku.Concepts;

/// <summary>
/// Extracts a base type that describes state table from elements of <typeparamref name="TSelf"/> type.
/// </summary>
/// <typeparam name="TSelf">The type of the instance that implements this interface type.</typeparam>
/// <typeparam name="TElement">The type of each element.</typeparam>
/// <typeparam name="TEnumerator">The type of the enumerator.</typeparam>
[LargeStructure]
[EqualityOperators(EqualityOperatorsBehavior.MakeVirtual)]
public partial interface IBitStatusMap<TSelf, TElement, TEnumerator> :
	IAdditiveIdentity<TSelf, TSelf>,
	IBitwiseOperators<TSelf, TSelf, TSelf>,
	ICultureFormattable,
	IEquatable<TSelf>,
	IEqualityOperators<TSelf, TSelf, bool>,
	ILogicalOperators<TSelf>,
	IMinMaxValue<TSelf>,
	IModulusOperators<TSelf, TSelf, TSelf>,
	IReadOnlySet<TElement>,
	ISet<TElement>,
	ISimpleParsable<TSelf>,
	ISubtractionOperators<TSelf, TSelf, TSelf>
	where TSelf : unmanaged, IBitStatusMap<TSelf, TElement, TEnumerator>
	where TElement : unmanaged, IBinaryInteger<TElement>
	where TEnumerator : struct, IEnumerator<TElement>
{
	/// <summary>
	/// Indicates the error information describing the case that the number of subsets calculated by methods
	/// <see cref="GetSubsets(int)"/> and <see cref="GetSubsetsAllBelow(int)"/> is too large.
	/// </summary>
	/// <seealso cref="GetSubsets(int)"/>
	/// <seealso cref="GetSubsetsAllBelow(int)"/>
	private protected static readonly string ErrorInfo_SubsetsExceeded = """
		Both cells count and subset size is too large, which may cause potential out of memory exception. 
		This operator will throw this exception to calculate the result, in order to prevent any possible exceptions thrown.
		""".RemoveLineEndings();


	/// <summary>
	/// Indicates the number of the values stored in this collection.
	/// </summary>
	public new abstract int Count { get; }

	/// <summary>
	/// Gets all chunks of the current collection, meaning a list of <see cref="string"/> values that can describe
	/// all cell and candidate indices, grouped with same row/column.
	/// </summary>
	public abstract string[] StringChunks { get; }

	/// <summary>
	/// Indicates the peer intersection of the current instance.
	/// </summary>
	/// <remarks>
	/// A <b>Peer Intersection</b> is a set of cells that all cells from the base collection can be seen.
	/// For more information please visit <see href="http://sudopedia.enjoysudoku.com/Peer.html">this link</see>.
	/// </remarks>
	public abstract TSelf PeerIntersection { get; }

	/// <summary>
	/// Indicates the cell offsets in this collection.
	/// </summary>
	protected internal abstract TElement[] Offsets { get; }

	/// <summary>
	/// Indicates the size of each unit.
	/// </summary>
	protected abstract int Shifting { get; }

	/// <inheritdoc/>
	bool ICollection<TElement>.IsReadOnly => false;

	/// <inheritdoc/>
	int ICollection<TElement>.Count => Count;


	/// <summary>
	/// Indicates the empty instance.
	/// </summary>
	public static abstract TSelf Empty { get; }

	/// <summary>
	/// Indicates the maximum number of elements that the collection can be reached.
	/// </summary>
	protected static abstract TElement MaxCount { get; }


	/// <summary>
	/// Get the offset at the specified position index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>
	/// The offset at the specified position index. If the value is invalid, the return value will be <c>-1</c>.
	/// </returns>
	public abstract TElement this[int index] { get; }


	/// <inheritdoc/>
	static TSelf IAdditiveIdentity<TSelf, TSelf>.AdditiveIdentity => TSelf.Empty;


	/// <summary>
	/// Adds a new offset into the current collection.
	/// </summary>
	/// <param name="offset">An offset to be added.</param>
	public new abstract bool Add(TElement offset);

	/// <summary>
	/// Adds a list of offsets into the current collection.
	/// </summary>
	/// <param name="offsets">Offsets to be added.</param>
	/// <returns>The number of offsets succeeded to be added.</returns>
	public abstract int AddRange(scoped ReadOnlySpan<TElement> offsets);

	/// <summary>
	/// Removes the specified offset from the current collection.
	/// </summary>
	/// <param name="offset">An offset to be removed.</param>
	public new abstract bool Remove(TElement offset);

	/// <summary>
	/// Removes a list of offsets from the current collection.
	/// </summary>
	/// <param name="offsets">Offsets to be removed.</param>
	/// <returns>The number of offsets succeeded to be removed.</returns>
	public abstract int RemoveRange(scoped ReadOnlySpan<TElement> offsets);

	/// <summary>
	/// Clear all bits.
	/// </summary>
	[ExplicitInterfaceImpl(typeof(ICollection<>))]
	public new abstract void Clear();

	/// <summary>
	/// Copies the current instance to the target sequence specified as a reference
	/// to an element of type <typeparamref name="TElement"/>.
	/// </summary>
	/// <param name="sequence">
	/// The reference that points to the first element in a sequence of type <typeparamref name="TElement"/>.
	/// </param>
	/// <param name="length">The length of that array.</param>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="sequence"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	/// Throws when the capacity isn't enough to store all values.
	/// </exception>
	public abstract void CopyTo(scoped ref TElement sequence, int length);

	/// <inheritdoc cref="ICollection{T}.CopyTo(T[], int)"/>
	[ExplicitInterfaceImpl(typeof(ICollection<>))]
	public new sealed void CopyTo(TElement[] array, int arrayIndex) => CopyTo(ref array[arrayIndex], Count - arrayIndex);

	/// <summary>
	/// Iterates on each element in this collection.
	/// </summary>
	/// <param name="action">The visitor that handles for each element in this collection.</param>
	public abstract void ForEach(Action<TElement> action);

	/// <inheritdoc cref="ISet{T}.ExceptWith(IEnumerable{T})"/>
	[ExplicitInterfaceImpl(typeof(ISet<>))]
	public new abstract void ExceptWith(IEnumerable<TElement> other);

	/// <inheritdoc cref="ISet{T}.IntersectWith(IEnumerable{T})"/>
	[ExplicitInterfaceImpl(typeof(ISet<>))]
	public new abstract void IntersectWith(IEnumerable<TElement> other);

	/// <inheritdoc cref="ISet{T}.SymmetricExceptWith(IEnumerable{T})"/>
	[ExplicitInterfaceImpl(typeof(ISet<>))]
	public new abstract void SymmetricExceptWith(IEnumerable<TElement> other);

	/// <inheritdoc cref="ISet{T}.UnionWith(IEnumerable{T})"/>
	[ExplicitInterfaceImpl(typeof(ISet<>))]
	public new abstract void UnionWith(IEnumerable<TElement> other);

	/// <summary>
	/// Determine whether the map contains the specified offset.
	/// </summary>
	/// <param name="offset">The offset.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	[ExplicitInterfaceImpl(typeof(IReadOnlySet<>))]
	public new abstract bool Contains(TElement offset);

	/// <summary>
	/// Try to get the specified index of the offset.
	/// </summary>
	/// <param name="offset">The desired offset.</param>
	/// <returns>The index of the offset.</returns>
	public virtual int IndexOf(TElement offset)
	{
		for (var index = 0; index < Count; index++)
		{
			if (this[index] == offset)
			{
				return index;
			}
		}

		return -1;
	}

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[ExplicitInterfaceImpl(typeof(IEquatable<>))]
	public abstract bool Equals(scoped ref readonly TSelf other);

	/// <summary>
	/// Get all offsets whose bits are set <see langword="true"/>.
	/// </summary>
	/// <returns>An array of offsets.</returns>
	public abstract TElement[] ToArray();

	/// <summary>
	/// Slices the current instance, and get the new instance with some of elements between two indices.
	/// </summary>
	/// <param name="start">The start index.</param>
	/// <param name="count">The number of elements.</param>
	/// <returns>The target instance.</returns>
	public abstract TSelf Slice(int start, int count);

	/// <summary>
	/// Gets the subsets of the current collection via the specified size indicating the number of elements of the each subset.
	/// </summary>
	/// <param name="subsetSize">The size to get.</param>
	/// <returns>
	/// All possible subsets. If:
	/// <list type="table">
	/// <listheader>
	/// <term>Condition</term>
	/// <description>Meaning</description>
	/// </listheader>
	/// <item>
	/// <term><c><paramref name="subsetSize"/> &gt; Count</c></term>
	/// <description>Will return an empty array</description>
	/// </item>
	/// <item>
	/// <term><c><paramref name="subsetSize"/> == Count</c></term>
	/// <description>
	/// Will return an array that contains only one element, same as the current instance.
	/// </description>
	/// </item>
	/// <item>
	/// <term>Other cases</term>
	/// <description>The valid combinations.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <exception cref="NotSupportedException">
	/// Throws when both the count of the current instance and <paramref name="subsetSize"/> are greater than 30.
	/// </exception>
	/// <remarks>
	/// For example, if the current instance is <c>r1c1</c>, <c>r1c2</c> and <c>r1c3</c>
	/// and the argument <paramref name="subsetSize"/> is 2,
	/// the method will return an array of 3 elements given below: <c>r1c12</c>, <c>r1c13</c> and <c>r1c23</c>.
	/// </remarks>
	public abstract ReadOnlySpan<TSelf> GetSubsets(int subsetSize);

	/// <summary>
	/// Equivalent to calling <see cref="GetSubsets(int)"/> with argument <see cref="Count"/>.
	/// </summary>
	/// <returns>All subsets of the current instance.</returns>
	/// <seealso cref="Count"/>
	/// <seealso cref="GetSubsetsAllBelow(int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual ReadOnlySpan<TSelf> GetSubsetsAll() => GetSubsetsAllBelow(Count);

	/// <summary>
	/// Gets all subsets of the current collection via the specified size
	/// indicating the <b>maximum</b> number of elements of the each subset.
	/// </summary>
	/// <param name="limitSubsetSize">The size to get.</param>
	/// <returns>
	/// All possible subsets. If:
	/// <list type="table">
	/// <listheader>
	/// <term>Condition</term>
	/// <description>Meaning</description>
	/// </listheader>
	/// <item>
	/// <term><c><paramref name="limitSubsetSize"/> &gt; Count</c></term>
	/// <description>Will return an empty array</description>
	/// </item>
	/// <item>
	/// <term>Other cases</term>
	/// <description>The valid combinations.</description>
	/// </item>
	/// </list>
	/// </returns>
	public virtual ReadOnlySpan<TSelf> GetSubsetsAllBelow(int limitSubsetSize)
	{
		if (limitSubsetSize == 0 || Count == 0)
		{
			return [];
		}

		var (n, desiredSize) = (Count, 0);
		var length = Math.Min(n, limitSubsetSize);
		for (var i = 1; i <= length; i++)
		{
			var target = PascalTriangle[n - 1][i - 1];
			desiredSize += target;
		}

		var result = new List<TSelf>(desiredSize);
		for (var i = 1; i <= length; i++)
		{
			result.AddRange(GetSubsets(i));
		}

		return result.AsReadOnlySpan();
	}

	/// <summary>
	/// Gets the enumerator of the current instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	public new abstract TEnumerator GetEnumerator();

	/// <inheritdoc cref="RandomSelect(int, Random)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual TSelf RandomSelect(int count) => RandomSelect(count, Random.Shared);

	/// <summary>
	/// Randomly select the specified number of elements in the current collection.
	/// </summary>
	/// <param name="count">The desired number of elements.</param>
	/// <param name="random">The random number generator instance.</param>
	/// <returns>The desired number of elements, as a <typeparamref name="TSelf"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual TSelf RandomSelect(int count, Random random)
	{
		var result = Offsets[..];
		random.Shuffle(result);
		return [.. result[..count]];
	}

	/// <inheritdoc cref="ISet{T}.IsProperSubsetOf(IEnumerable{T})"/>
	[ExplicitInterfaceImpl(typeof(ISet<>))]
	[ExplicitInterfaceImpl(typeof(IReadOnlySet<>))]
	public new sealed bool IsProperSubsetOf(IEnumerable<TElement> other)
	{
		var otherCells = TSelf.Empty;
		foreach (var element in other)
		{
			otherCells.Add(element);
		}

		return (TSelf)this != otherCells && (otherCells & (TSelf)this) == (TSelf)this;
	}

	/// <inheritdoc cref="ISet{T}.IsProperSupersetOf(IEnumerable{T})"/>
	[ExplicitInterfaceImpl(typeof(ISet<>))]
	[ExplicitInterfaceImpl(typeof(IReadOnlySet<>))]
	public new sealed bool IsProperSupersetOf(IEnumerable<TElement> other)
	{
		var otherCells = TSelf.Empty;
		foreach (var element in other)
		{
			otherCells.Add(element);
		}

		return (TSelf)this != otherCells && ((TSelf)this & otherCells) == otherCells;
	}

	/// <inheritdoc cref="ISet{T}.IsSubsetOf(IEnumerable{T})"/>
	[ExplicitInterfaceImpl(typeof(ISet<>))]
	[ExplicitInterfaceImpl(typeof(IReadOnlySet<>))]
	public new sealed bool IsSubsetOf(IEnumerable<TElement> other)
	{
		var otherCells = TSelf.Empty;
		foreach (var element in other)
		{
			otherCells.Add(element);
		}

		return (otherCells & (TSelf)this) == (TSelf)this;
	}

	/// <inheritdoc cref="ISet{T}.IsSupersetOf(IEnumerable{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ExplicitInterfaceImpl(typeof(ISet<>))]
	[ExplicitInterfaceImpl(typeof(IReadOnlySet<>))]
	public new sealed bool IsSupersetOf(IEnumerable<TElement> other)
	{
		var otherCells = TSelf.Empty;
		foreach (var element in other)
		{
			otherCells.Add(element);
		}

		return ((TSelf)this & otherCells) == otherCells;
	}

	/// <inheritdoc cref="ISet{T}.Overlaps(IEnumerable{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ExplicitInterfaceImpl(typeof(ISet<>))]
	[ExplicitInterfaceImpl(typeof(IReadOnlySet<>))]
	public new sealed bool Overlaps(IEnumerable<TElement> other) => !!((TSelf)this & [.. other]);

	/// <inheritdoc cref="ISet{T}.SetEquals(IEnumerable{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ExplicitInterfaceImpl(typeof(ISet<>))]
	[ExplicitInterfaceImpl(typeof(IReadOnlySet<>))]
	public new sealed bool SetEquals(IEnumerable<TElement> other) => (TSelf)this == [.. other];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void ICollection<TElement>.Add(TElement item) => Add(item);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool ISet<TElement>.Add(TElement item) => Add(item);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool ICollection<TElement>.Remove(TElement item) => Remove(item);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TElement>)this).GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
	{
		if (Offsets.Length == 0)
		{
			yield break;
		}

		foreach (var element in Offsets)
		{
			yield return element;
		}
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
	[ExplicitInterfaceImpl(typeof(ILogicalOperators<>))]
	public static abstract bool operator !(scoped in TSelf offsets);

	/// <summary>
	/// Reverse state for all offsets, which means all <see langword="true"/> bits
	/// will be set <see langword="false"/>, and all <see langword="false"/> bits
	/// will be set <see langword="true"/>.
	/// </summary>
	/// <param name="offsets">The instance to negate.</param>
	/// <returns>The negative result.</returns>
	[ExplicitInterfaceImpl(typeof(IBitwiseOperators<,,>))]
	public static abstract TSelf operator ~(scoped in TSelf offsets);

	/// <summary>
	/// Determines whether the specified <typeparamref name="TSelf"/> collection is not empty.
	/// </summary>
	/// <param name="cells">The collection.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ExplicitInterfaceImpl(typeof(ILogicalOperators<>))]
	public static virtual bool operator true(scoped in TSelf cells) => cells.Count != 0;

	/// <summary>
	/// Determines whether the specified <typeparamref name="TSelf"/> collection is empty.
	/// </summary>
	/// <param name="cells">The collection.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ExplicitInterfaceImpl(typeof(ILogicalOperators<>))]
	public static virtual bool operator false(scoped in TSelf cells) => cells.Count == 0;

	/// <summary>
	/// Adds the specified <paramref name="offset"/> to the <paramref name="collection"/>,
	/// and returns the added result.
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <param name="offset">The offset to be added.</param>
	/// <returns>The result collection.</returns>
	public static abstract TSelf operator +(scoped in TSelf collection, TElement offset);

	/// <summary>
	/// Removes the specified <paramref name="offset"/> from the <paramref name="collection"/>,
	/// and returns the removed result.
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <param name="offset">The offset to be removed.</param>
	/// <returns>The result collection.</returns>
	public static abstract TSelf operator -(scoped in TSelf collection, TElement offset);

	/// <summary>
	/// Get a <typeparamref name="TSelf"/> that contains all <paramref name="left"/> instance
	/// but not in <paramref name="right"/> instance.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	public static virtual TSelf operator -(scoped in TSelf left, scoped in TSelf right) => left & ~right;

	/// <summary>
	/// Get the elements that both <paramref name="left"/> and <paramref name="right"/> contain.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	[ExplicitInterfaceImpl(typeof(IBitwiseOperators<,,>))]
	[ExplicitInterfaceImpl(typeof(ILogicalOperators<>))]
	public static abstract TSelf operator &(scoped in TSelf left, scoped in TSelf right);

	/// <summary>
	/// Combine the elements from <paramref name="left"/> and <paramref name="right"/>,
	/// and return the merged result.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	[ExplicitInterfaceImpl(typeof(IBitwiseOperators<,,>))]
	[ExplicitInterfaceImpl(typeof(ILogicalOperators<>))]
	public static abstract TSelf operator |(scoped in TSelf left, scoped in TSelf right);

	/// <summary>
	/// Get the elements that either <paramref name="left"/> or <paramref name="right"/> contains.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	[ExplicitInterfaceImpl(typeof(IBitwiseOperators<,,>))]
	public static abstract TSelf operator ^(scoped in TSelf left, scoped in TSelf right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<TSelf, TSelf, bool>.operator ==(TSelf left, TSelf right) => left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<TSelf, TSelf, bool>.operator !=(TSelf left, TSelf right) => left != right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static TSelf ISubtractionOperators<TSelf, TSelf, TSelf>.operator -(TSelf left, TSelf right) => left - right;

	/// <summary>
	/// Expands the operator to <c><![CDATA[(a & b).PeerIntersection & b]]></c>.
	/// </summary>
	/// <param name="left">The base map.</param>
	/// <param name="right">The template map that the base map to check and cover.</param>
	/// <returns>The result map.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static TSelf IModulusOperators<TSelf, TSelf, TSelf>.operator %(TSelf left, TSelf right) => (left & right).PeerIntersection & right;


	/// <summary>
	/// Converts an element of type <typeparamref name="TElement"/> into a <typeparamref name="TSelf"/> instance,
	/// with only one element - itself.
	/// </summary>
	/// <param name="offset">The offset to be used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static virtual explicit operator TSelf(TElement offset) => [offset];

	/// <summary>
	/// Converts an array of element type <typeparamref name="TElement"/> to a <typeparamref name="TSelf"/> instance.
	/// </summary>
	/// <param name="offsets">An array of element type <typeparamref name="TElement"/>.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static virtual explicit operator TSelf(TElement[] offsets) => [.. offsets];

	/// <summary>
	/// Converts an <see cref="ReadOnlySpan{T}"/> of element type <typeparamref name="TElement"/> to a <typeparamref name="TSelf"/> instance.
	/// </summary>
	/// <param name="offsets">An array of element type <typeparamref name="TElement"/>.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static virtual explicit operator TSelf(scoped ReadOnlySpan<TElement> offsets) => [.. offsets];
}
