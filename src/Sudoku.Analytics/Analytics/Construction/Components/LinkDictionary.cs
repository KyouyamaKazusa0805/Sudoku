namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a list of <see cref="Node"/> relations on strong or weak links.
/// </summary>
/// <seealso cref="Node"/>
public sealed class LinkDictionary : Dictionary<Node, HashSet<Node>>, IComponent
{
	/// <summary>
	/// The backing pool storing link and patterns.
	/// </summary>
	private readonly Dictionary<Link, Pattern> _groupedLinkPool = new(
		EqualityComparer<Link>.Create(
			static (left, right) => (left, right) switch
			{
				(null, null) => true,
				(not null, not null) => left.Equals(right, LinkComparison.Directed),
				_ => false
			},
			static obj => obj.GetHashCode(LinkComparison.Directed)
		)
	);


	/// <summary>
	/// Indicates the pool of grouped links and its corresponding pattern.
	/// </summary>
	public FrozenDictionary<Link, Pattern> GroupedLinkPool => _groupedLinkPool.ToFrozenDictionary();

	/// <inheritdoc/>
	ComponentType IComponent.Type => ComponentType.ChainLinkDictionary;


	/// <summary>
	/// Clears the collection.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public new void Clear()
	{
		base.Clear();
		_groupedLinkPool.Clear();
	}

	/// <summary>
	/// Try to append grouped links from the other dictionary. This method will merge links without any distinctive checks.
	/// </summary>
	/// <param name="other">The other dictionary.</param>
	public void Merge(LinkDictionary other)
	{
		foreach (var (k, v) in other)
		{
			if (!TryAdd(k, v))
			{
				this[k].UnionWith(v);
			}
		}
		foreach (var (k, v) in other._groupedLinkPool)
		{
			_groupedLinkPool.TryAdd(k, v);
		}
	}

	/// <summary>
	/// Add a link to the current collection with both entries on nodes of the link used.
	/// </summary>
	/// <param name="node1">Indicates the first node to be added.</param>
	/// <param name="node2">Indicates the second node to be added.</param>
	/// <param name="isStrong">Indicates the grouped link pattern is a strong link.</param>
	/// <param name="pattern">The advanced pattern to be used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddEntry(Node node1, Node node2, bool isStrong = false, Pattern? pattern = null)
	{
		if (!TryAdd(node1, [node2]))
		{
			this[node1].Add(node2);
		}
		if (!TryAdd(node2, [node1]))
		{
			this[node2].Add(node1);
		}
		if (pattern is not null)
		{
			// Add pattern into pool.
			// We may not add its reversed version because the pool dictionary is compared under undirected rule.
			_groupedLinkPool.TryAdd(new(node1, node2, isStrong), pattern);
		}

		var (node3, node4) = (~node1, ~node2);
		if (!TryAdd(node3, [node4]))
		{
			this[node3].Add(node4);
		}
		if (!TryAdd(node4, [node3]))
		{
			this[node4].Add(node3);
		}
		if (pattern is not null)
		{
			_groupedLinkPool.TryAdd(new(node3, node4, isStrong), pattern);
		}
	}
}
