namespace Sudoku.Concepts.Solving;

/// <summary>
/// Defines a chain.
/// </summary>
public abstract class Chain :
	IEquatable<Chain>,
	IEnumerable<Node>,
	IReadOnlyCollection<Node>,
	IReadOnlyList<Node>
#if FEATURE_GENERIC_MATH
	,
	IEqualityOperators<Chain, Chain>
#endif
{
	/// <summary>
	/// Indicates the status of those nodes.
	/// </summary>
	protected readonly bool[] _nodesStatus;

	/// <summary>
	/// Indicates the nodes that is used in the current chain.
	/// Generally, the first element and the last element are same.
	/// </summary>
	protected readonly Node[] _nodes;


	/// <summary>
	/// Initializes a instance whose type is derived from <see cref="Chain"/>,
	/// using the specified nodes and their own status respectively.
	/// </summary>
	/// <param name="nodes">The nodes.</param>
	/// <param name="nodesStatus">The nodes' status.</param>
	/// <exception cref="ArgumentException">
	/// Throws when the arguments <paramref name="nodes"/> and <paramref name="nodesStatus"/>
	/// don't hold the same number of elements.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private protected Chain(Node[] nodes, bool[] nodesStatus) =>
		(_nodes, _nodesStatus) =
			nodes.Length == nodesStatus.Length
				? (nodes, nodesStatus)
				: throw new ArgumentException("Two arguments must keep the same number of elements.");


	/// <inheritdoc/>
	public virtual int Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _nodes.Length - 1;
	}


	/// <inheritdoc/>
	public virtual Node this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _nodes[index];
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override bool Equals([NotNullWhen(true)] object? obj) => Equals(obj as Chain);

	/// <inheritdoc/>
	public abstract bool Equals([NotNullWhen(true)] Chain? other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override int GetHashCode()
	{
		var hashCodeHandler = new HashCode();
		hashCodeHandler.Add(GetType());
		for (int i = 0, length = _nodes.Length - 1; i < length; i++)
		{
			hashCodeHandler.Add(_nodes[i]);
			hashCodeHandler.Add(_nodesStatus[i]);
		}

		return hashCodeHandler.ToHashCode();
	}

	/// <inheritdoc/>
	public abstract override string ToString();

	/// <summary>
	/// Gets the conclusions of the current chain.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The conclusions of the current chain.</returns>
	/// <remarks>
	/// <para><b>TODO</b>: Append extra eliminations that requires the extended elimination rules.</para>
	/// <para>
	/// e.g.
	/// <code>
	/// a. 25 | d. 28
	/// b. 25 | e. 38
	/// c. 3+ | f. 5+
	/// </code>
	/// Chain: <c>a(5) == d(8) -- e(8 == 3) => b != 5</c>, where the cell <c>c</c> and <c>f</c>
	/// are filled with modifiable values.
	/// </para>
	/// </remarks>
	public abstract ImmutableArray<Conclusion> GetConclusions(in Grid grid);

	/// <summary>
	/// Creates an array that stores the values with each element of a pair, where:
	/// <list type="table">
	/// <item>
	/// <term>The first tuple element</term>
	/// <description>The node itself.</description>
	/// </item>
	/// <item>
	/// <term>The second tuple element</term>
	/// <description>The status of the current node, indicating whether the node is on currently.</description>
	/// </item>
	/// </list>
	/// </summary>
	/// <returns>An array that stores the values with each element of a pair.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (Node Node, bool IsOn)[] ToRawArray()
	{
		var result = new (Node, bool)[_nodes.Length];
		for (int i = 0, length = _nodes.Length; i < length; i++)
		{
			result[i] = (_nodes[i], _nodesStatus[i]);
		}

		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IEnumerator<Node> GetEnumerator() => ((IEnumerable<Node>)_nodes).GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


	/// <summary>
	/// Determines whether two <see cref="Chain"/>s are same.
	/// </summary>
	/// <param name="left">The left instance to be compared.</param>
	/// <param name="right">The right instance to be compared.</param>
	/// <returns>A <see cref="bool"/> value indicating the result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Chain left, Chain right) => left.Equals(right);

	/// <summary>
	/// Determines whether two <see cref="Chain"/>s are not same.
	/// </summary>
	/// <param name="left">The left instance to be compared.</param>
	/// <param name="right">The right instance to be compared.</param>
	/// <returns>A <see cref="bool"/> value indicating the result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Chain left, Chain right) => !(left == right);
}
