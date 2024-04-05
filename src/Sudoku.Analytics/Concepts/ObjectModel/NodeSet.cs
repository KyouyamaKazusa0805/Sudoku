namespace Sudoku.Concepts.ObjectModel;

/// <summary>
/// Defines a <see cref="ChainNode"/> collection using <see cref="HashSet{T}"/> as backing implementation.
/// </summary>
/// <seealso cref="ChainNode"/>
public sealed class NodeSet : HashSet<ChainNode>
{
	/// <summary>
	/// Initializes a <see cref="NodeSet"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public NodeSet() : base(ValueComparison.CreateByEqualityOperator<ChainNode>())
	{
	}

	/// <summary>
	/// Initializes a <see cref="NodeSet"/> instance via the specified <see cref="ChainNode"/> collection to be added.
	/// </summary>
	/// <param name="base">The collection of <see cref="ChainNode"/> instances.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public NodeSet(NodeSet @base) : base(@base, ValueComparison.CreateByEqualityOperator<ChainNode>())
	{
	}


	/// <summary>
	/// Try to get the target <see cref="ChainNode"/> instance whose internal value
	/// (i.e. properties <see cref="ChainNode.Candidate"/> and <see cref="ChainNode.IsOn"/>) are same as
	/// the specified one.
	/// </summary>
	/// <param name="node">The value to be checked.</param>
	/// <returns>
	/// The found value whose value is equal to <paramref name="node"/>; without checking for property <see cref="ChainNode.Parents"/>.
	/// </returns>
	/// <remarks><i>
	/// Please note that this method will return an instance inside the current collection
	/// whose value equals to the specified one; however, property <see cref="ChainNode.Parents"/> may not be equal.
	/// </i></remarks>
	/// <exception cref="InvalidOperationException">Throws when the result is <see langword="null"/>.</exception>
	/// <seealso cref="ChainNode.Candidate"/>
	/// <seealso cref="ChainNode.IsOn"/>
	[IndexerName("Get")]
	public ChainNode this[ChainNode node]
	{
		get
		{
			foreach (var potential in this)
			{
				if (potential == node)
				{
					return potential;
				}
			}

			throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("ChainNodeDataInvalid"));
		}
	}


	/// <summary>
	/// Determines whether a <see cref="NodeSet"/> object contains the specified element,
	/// comparing for properties <see cref="ChainNode.Candidate"/> and <see cref="ChainNode.IsOn"/>.
	/// </summary>
	/// <param name="base">The element to locate in the <see cref="NodeSet"/> object.</param>
	/// <returns>
	/// <see langword="true"/> if the <see cref="NodeSet"/> object contains the specified element; otherwise, <see langword="false"/>.
	/// </returns>
	/// <seealso cref="ChainNode.Candidate"/>
	/// <seealso cref="ChainNode.IsOn"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public new bool Contains(ChainNode @base) => GetNullable(@base) is not null;

	/// <summary>
	/// <para>
	/// Try to get the target <see cref="ChainNode"/> instance whose internal value
	/// (i.e. properties <see cref="ChainNode.Candidate"/> and <see cref="ChainNode.IsOn"/>) are same as
	/// the specified one.
	/// </para>
	/// <para>
	/// Please note that this method will return an instance inside the current collection
	/// whose value equals to the specified one; however, property <see cref="ChainNode.Parents"/> may not be equal.
	/// </para>
	/// </summary>
	/// <param name="base">The value to be checked.</param>
	/// <returns>
	/// <para>
	/// The found value whose value is equal to <paramref name="base"/>; without checking for property <see cref="ChainNode.Parents"/>.
	/// </para>
	/// <para>If none found, <see langword="null"/> will be returned.</para>
	/// </returns>
	/// <seealso cref="ChainNode.Candidate"/>
	/// <seealso cref="ChainNode.IsOn"/>
	public ChainNode? GetNullable(ChainNode @base)
	{
		foreach (var potential in this)
		{
			if (potential == @base)
			{
				return potential;
			}
		}

		return null;
	}


	/// <summary>
	/// Gets the <see cref="ChainNode"/> instances that both <paramref name="left"/> and <paramref name="right"/> contain,
	/// and modifies the argument <paramref name="left"/>, replacing it with <see cref="ChainNode"/>s mentioned above,
	/// then returns it.
	/// </summary>
	/// <param name="left">The first collection to be participated in merging operation.</param>
	/// <param name="right">The second collection to be participated in merging operation.</param>
	/// <returns>Modified collection <paramref name="left"/>.</returns>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="/g/requires-compound-invocation"/>
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static NodeSet operator &(NodeSet left, NodeSet right)
	{
		left.IntersectWith(right);
		return left;
	}

	/// <summary>
	/// Gets the <see cref="ChainNode"/> instances that comes from both collections <paramref name="left"/> and <paramref name="right"/>,
	/// and modifies the argument <paramref name="left"/>, replacing it with <see cref="ChainNode"/>s mentioned above,
	/// then returns it.
	/// </summary>
	/// <param name="left">The first collection to be participated in merging operation.</param>
	/// <param name="right">The second collection to be participated in merging operation.</param>
	/// <returns>Modified collection <paramref name="left"/>.</returns>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="/g/requires-compound-invocation"/>
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static NodeSet operator |(NodeSet left, NodeSet right)
	{
		left.UnionWith(right);
		return left;
	}

	/// <summary>
	/// Gets the <see cref="ChainNode"/> instances that only one collection in <paramref name="left"/> and <paramref name="right"/> contains,
	/// and modifies the argument <paramref name="left"/>, replacing it with <see cref="ChainNode"/>s mentioned above,
	/// then returns it.
	/// </summary>
	/// <param name="left">The first collection to be participated in merging operation.</param>
	/// <param name="right">The second collection to be participated in merging operation.</param>
	/// <returns>Modified collection <paramref name="left"/>.</returns>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="/g/requires-compound-invocation"/>
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static NodeSet operator ^(NodeSet left, NodeSet right)
	{
		left.SymmetricExceptWith(right);
		return left;
	}
}
