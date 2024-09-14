namespace Sudoku.Analytics.Patterning.Chaining;

/// <summary>
/// Represents a chain or a loop.
/// </summary>
[TypeImpl(
	TypeImplFlag.Object_Equals | TypeImplFlag.Object_ToString | TypeImplFlag.AllEqualityComparisonOperators,
	OtherModifiersOnEquals = "sealed",
	ToStringBehavior = ToStringBehavior.MakeAbstract)]
public abstract partial class ChainOrLoop :
	IComparable<ChainOrLoop>,
	IComparisonOperators<ChainOrLoop, ChainOrLoop, bool>,
	IEnumerable<Node>,
	IEquatable<ChainOrLoop>,
	IEqualityOperators<ChainOrLoop, ChainOrLoop, bool>,
	IFormattable,
	IReadOnlyList<Node>,
	IReadOnlyCollection<Node>
{
	/// <summary>
	/// Indicates the possible inferences to be used.
	/// </summary>
	protected static readonly Inference[] Inferences = [Inference.Strong, Inference.Weak];


	/// <summary>
	/// Indicates the nodes to be initialized.
	/// </summary>
	protected readonly Node[] _nodes;

	/// <summary>
	/// Indicates the strong grouped link pool.
	/// </summary>
	protected readonly FrozenDictionary<Link, object> _strongGroupedLinkPool;

	/// <summary>
	/// Indicates the weak grouped link pool.
	/// </summary>
	protected readonly FrozenDictionary<Link, object> _weakGroupedLinkPool;


	/// <summary>
	/// Initializes <see cref="ChainOrLoop"/> data.
	/// </summary>
	/// <param name="lastNode">The last node.</param>
	/// <param name="isLoop">Indicates whether is for loop initialization.</param>
	/// <param name="autoReversingOnComparison">
	/// <para>
	/// Indicates whether the constructor will automatically reverse the chain
	/// if the first node is greater than the last node, in order to make a good look.
	/// </para>
	/// <para>
	/// The default value is <see langword="true"/>. You can also set the value with <see langword="false"/>
	/// if you don't want to make the constructor reverse the whole chain.
	/// </para>
	/// </param>
	protected ChainOrLoop(Node lastNode, bool isLoop, bool autoReversingOnComparison = true)
	{
		_strongGroupedLinkPool = StrongLinkDictionary.GroupedLinkPool;
		_weakGroupedLinkPool = WeakLinkDictionary.GroupedLinkPool;
		var nodes = (List<Node>)[lastNode];
		for (var node = lastNode.Parent!; isLoop ? node != lastNode : node is not null; node = node.Parent!)
		{
			nodes.Add(node >> null);
		}
		_nodes = [.. nodes];

		// Now reverse the chain if worth.
		// If the last node is supposed 'on', it will be a normal elimination-typed chain, and can be reversed.
		// Now we should reverse the whole chain if the first node is greater than the last node in logic.
		if (autoReversingOnComparison && _nodes[^1].IsOn && nodes[1].CompareTo(nodes[^2], NodeComparison.IgnoreIsOn) >= 0)
		{
			Reverse();
		}
	}


	/// <summary>
	/// Indicates whether the current pattern is bound with a technique (i.e. having a technique name).
	/// </summary>
	/// <remarks>
	/// Due to design of this type, the derived types may not be consumed by showing a pattern.
	/// It may be a "segment" of a whole pattern (for example, a branch inside a multiple forcing chains).
	/// In such cases, this property will return <see langword="false"/>, indicating there's no name corresponding to the pattern.
	/// </remarks>
	public abstract bool IsNamed { get; }

	/// <summary>
	/// Indicates whether the chain pattern uses grouped logic.
	/// </summary>
	public bool IsGrouped => ValidNodes.Any(static node => node.IsAdvanced || node.IsGroupedNode);

	/// <summary>
	/// Indicates whether the pattern only uses same digits.
	/// </summary>
	public bool SatisfyXRule
	{
		get
		{
			var digitsMask = (Mask)0;
			foreach (var node in ValidNodes)
			{
				digitsMask |= node.Map.Digits;
			}
			return Mask.IsPow2(digitsMask);
		}
	}

	/// <summary>
	/// Indicates whether the pattern only uses cell strong links.
	/// </summary>
	public bool SatisfyYRule
	{
		get
		{
			foreach (var link in StrongLinks)
			{
				if (link is { FirstNode.Map.Digits: var digits1, SecondNode.Map.Digits: var digits2 } && digits1 == digits2)
				{
					return false;
				}
			}
			return First.Map.Digits == Last.Map.Digits;
		}
	}

	/// <summary>
	/// Indicates whether at least one node in the whole pattern overlaps with a node.
	/// </summary>
	public bool ContainsOverlappedNodes
	{
		get
		{
			foreach (var nodePair in (from node in ValidNodes select node.Map).GetSubsets(2))
			{
				ref readonly var map1 = ref nodePair[0];
				ref readonly var map2 = ref nodePair[1];
				if (map1 & map2)
				{
					return true;
				}
			}
			return false;
		}
	}

	/// <summary>
	/// Indicates the length of the pattern.
	/// </summary>
	public int Length => ValidNodes.Length;

	/// <summary>
	/// Indicates the complexity of the pattern.
	/// </summary>
	/// <remarks>
	/// The value is different with <see cref="Length"/> on a chain starting and ending with itself,
	/// both are by strong links;
	/// however it sometimes is equal to <see cref="Length"/>. It depends on the kind of the chain rule obeys.
	/// For example, a loop has a same complexity and length.
	/// </remarks>
	/// <seealso cref="Length"/>
	public virtual int Complexity => Length;

	/// <summary>
	/// Indicates all digits used in this pattern.
	/// </summary>
	public Mask DigitsMask
	{
		get
		{
			var result = (Mask)0;
			foreach (var node in this)
			{
				result |= node.Map.Digits;
			}
			return result;
		}
	}

	/// <summary>
	/// Indicates the links used.
	/// </summary>
	public ReadOnlySpan<Link> Links
	{
		get
		{
			var span = ValidNodes;
			var resultLength = Length - LoopIdentity;
			var result = new Link[resultLength];
			for (var (linkIndex, i) = (WeakStartIdentity, 0); i < resultLength; linkIndex++, i++)
			{
				var isStrong = Inferences[linkIndex & 1] == Inference.Strong;
				var pool = isStrong ? _strongGroupedLinkPool : _weakGroupedLinkPool;
				pool.TryGetValue(new(span[i], span[(i + 1) % Length], isStrong), out var pattern);
				result[i] = new(span[i], span[(i + 1) % Length], isStrong, pattern);
			}
			return result;
		}
	}

	/// <summary>
	/// Indicates the strong links.
	/// </summary>
	public ReadOnlySpan<Link> StrongLinks
	{
		get
		{
			var result = new List<Link>(ValidNodes.Length >> 1);
			foreach (var link in Links)
			{
				if (link.IsStrong)
				{
					result.Add(link);
				}
			}
			return result.AsReadOnlySpan();
		}
	}

	/// <summary>
	/// Indicates the weak links.
	/// </summary>
	public ReadOnlySpan<Link> WeakLinks
	{
		get
		{
			var result = new List<Link>(Links.Length >> 1);
			foreach (var link in Links)
			{
				if (!link.IsStrong)
				{
					result.Add(link);
				}
			}
			return result.AsReadOnlySpan();
		}
	}

	/// <summary>
	/// Indicates the head node.
	/// </summary>
	public Node First => ValidNodes[0];

	/// <summary>
	/// Indicates the tail node.
	/// </summary>
	public Node Last => ValidNodes[^1];

	/// <summary>
	/// Indicates the value as the start index of the chain link is from whether strong and weak.
	/// </summary>
	protected abstract int WeakStartIdentity { get; }

	/// <summary>
	/// Indicates the value on loop checking for link construction usages.
	/// </summary>
	protected abstract int LoopIdentity { get; }

	/// <summary>
	/// Indicates the valid nodes to be used.
	/// </summary>
	protected abstract ReadOnlySpan<Node> ValidNodes { get; }

	/// <inheritdoc/>
	int IReadOnlyCollection<Node>.Count => Length;


	/// <summary>
	/// Gets a <see cref="Node"/> instance at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>The <see cref="Node"/> instance.</returns>
	/// <exception cref="IndexOutOfRangeException">Throws when the argument <paramref name="index"/> is out of range.</exception>
	public Node this[int index] => ValidNodes[index];


	/// <summary>
	/// Try to reverse the pattern, making all nodes negated its direction connected.
	/// </summary>
	public abstract void Reverse();

	/// <inheritdoc/>
	public bool Equals(ChainOrLoop? other) => Equals(other, NodeComparison.IgnoreIsOn, ChainOrLoopComparison.Undirected);

	/// <summary>
	/// Determine whether two <see cref="Chain"/> or <see cref="Loop"/> instances are same, by using the specified comparison rule.
	/// </summary>
	/// <param name="other">The other instance to be compared.</param>
	/// <param name="nodeComparison">The comparison rule on nodes.</param>
	/// <param name="patternComparison">The comparison rule on the whole chain.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="patternComparison"/> is not defined.
	/// </exception>
	public abstract bool Equals([NotNullWhen(true)] ChainOrLoop? other, NodeComparison nodeComparison, ChainOrLoopComparison patternComparison);

	/// <summary>
	/// Determines whether the current pattern (nodes) overlap with a list of conclusions,
	/// meaning at least one conclusion is used by a node appeared in the pattern.
	/// If so, the chain or loop will become a cannibalistic one.
	/// </summary>
	/// <param name="conclusions">The conclusions to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public bool OverlapsWithConclusions(ConclusionSet conclusions)
	{
		foreach (var conclusion in conclusions)
		{
			foreach (var node in ValidNodes)
			{
				if (node.Map.Contains(conclusion.Candidate))
				{
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// Determines whether the pattern has already used the specified candidate.
	/// </summary>
	/// <param name="candidate">The candidate.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public bool Contains(Candidate candidate)
	{
		foreach (var node in this)
		{
			if (node.Map.Contains(candidate))
			{
				return true;
			}
		}
		return false;
	}

	/// <inheritdoc cref="object.GetHashCode"/>
	/// <remarks>
	/// This method directly calls <see cref="GetHashCode(NodeComparison, ChainOrLoopComparison)"/>
	/// with values <see cref="NodeComparison.IgnoreIsOn"/> and <see cref="ChainOrLoopComparison.Undirected"/>.
	/// </remarks>
	/// <seealso cref="GetHashCode(NodeComparison, ChainOrLoopComparison)"/>
	/// <seealso cref="NodeComparison.IgnoreIsOn"/>
	/// <seealso cref="ChainOrLoopComparison.Undirected"/>
	public sealed override int GetHashCode() => GetHashCode(NodeComparison.IgnoreIsOn, ChainOrLoopComparison.Undirected);

	/// <summary>
	/// Computes hash code based on the current instance.
	/// </summary>
	/// <param name="nodeComparison">The node comparison.</param>
	/// <param name="patternComparison">The pattern comparison.</param>
	/// <returns>An <see cref="int"/> as the result.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="patternComparison"/> is not defined.
	/// </exception>
	public abstract int GetHashCode(NodeComparison nodeComparison, ChainOrLoopComparison patternComparison);

	/// <summary>
	/// Try to find a node satisfying the specified condition, and return its index. If none found, -1 will be returned.
	/// </summary>
	/// <param name="predicate">The condition that a node should satisfy.</param>
	/// <returns>The index of the node satisfied the condition.</returns>
	public int FindIndex(Predicate<Node> predicate)
	{
		for (var i = 0; i < Length; i++)
		{
			if (predicate(this[i]))
			{
				return i;
			}
		}
		return -1;
	}

	/// <summary>
	/// Try to find a node satisfying the specified condition from end, and return its index. If none found, -1 will be returned.
	/// </summary>
	/// <param name="predicate">The condition that a node should satisfy.</param>
	/// <returns>The index of the node satisfied the condition.</returns>
	public int FindLastIndex(Predicate<Node> predicate)
	{
		for (var i = Length - 1; i >= 0; i--)
		{
			if (predicate(this[i]))
			{
				return i;
			}
		}
		return -1;
	}

	/// <inheritdoc/>
	public abstract int CompareTo(ChainOrLoop? other);

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(IFormatProvider? formatProvider) => ToString(null, formatProvider);

	/// <inheritdoc/>
	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		var span = ValidNodes;
		var sb = new StringBuilder();
		for (var (linkIndex, i) = (WeakStartIdentity, 0); i < span.Length; linkIndex++, i++)
		{
			var inference = Inferences[linkIndex & 1];
			sb.Append(span[i].ToString(format, formatProvider));
			if (i != span.Length - 1)
			{
				sb.Append(inference.ConnectingNotation());
			}
		}
		return sb.ToString();
	}

	/// <summary>
	/// Slices the collection with the specified start node and its length.
	/// </summary>
	/// <param name="start">The start index.</param>
	/// <param name="length">The number of <see cref="Node"/> instances to slice.</param>
	/// <returns>A <see cref="ReadOnlySpan{T}"/> of <see cref="Node"/> instances returned.</returns>
	public ReadOnlySpan<Node> Slice(int start, int length) => ValidNodes[start..(start + length)];

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(this);

	/// <summary>
	/// Collect views for the current chain.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="supportedRules">The supported rules.</param>
	/// <param name="alsIndex">Indicates the currently operated ALS index.</param>
	/// <returns>The views.</returns>
	public View[] GetViews(ref readonly Grid grid, ChainingRuleCollection supportedRules, ref int alsIndex)
	{
		var result = (View[])[
			[
				.. v(),
				..
				from link in Links
				let node1 = link.FirstNode
				let node2 = link.SecondNode
				select new ChainLinkViewNode(ColorIdentifier.Normal, node1.Map, node2.Map, link.IsStrong)
			]
		];

		foreach (var supportedRule in supportedRules)
		{
			var context = new ChainingRuleViewNodeContext(in grid, this, result[0]) { CurrentAlmostLockedSetIndex = alsIndex };
			supportedRule.GetViewNodes(ref context);
			alsIndex = context.CurrentAlmostLockedSetIndex;
		}
		return result;


		ReadOnlySpan<CandidateViewNode> v()
		{
			var result = new List<CandidateViewNode>();
			for (var i = 0; i < Length; i++)
			{
				var id = (i & 1) == 0 ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal;
				foreach (var candidate in this[i].Map)
				{
					result.Add(new(id, candidate));
				}
			}
			return result.AsReadOnlySpan();
		}
	}

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => _nodes.GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<Node> IEnumerable<Node>.GetEnumerator() => _nodes.AsEnumerable().GetEnumerator();


	/// <inheritdoc cref="GetConclusions(ref readonly Grid, Node, Node)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected internal static ReadOnlySpan<Conclusion> GetConclusions(ref readonly Grid grid, Link link)
		=> GetConclusions(in grid, link.FirstNode, link.SecondNode);

	/// <summary>
	/// Try to get all possible conclusions via the specified grid and two <see cref="Node"/> instances.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="node1">The first node.</param>
	/// <param name="node2">The second node.</param>
	/// <returns>A sequence of <see cref="Conclusion"/> instances.</returns>
	/// <seealso cref="Conclusion"/>
	protected internal static ReadOnlySpan<Conclusion> GetConclusions(ref readonly Grid grid, Node node1, Node node2)
	{
		var candidatesMap = grid.CandidatesMap;
		if (node1 == ~node2)
		{
			// Two nodes are same, meaning the node must be true. Check whether it is grouped one.
			var digit = node1.Map[0] % 9;
			var map = node1.Map / digit;
			return node1.IsGroupedNode
				? from cell in map.PeerIntersection & candidatesMap[digit] select new Conclusion(Elimination, cell, digit)
				: (Conclusion[])[new(Assignment, node1.Map[0])];
		}

		// Two nodes aren't same. Check for values.
		if ((node1, node2) is not ({ Map: { Digits: var p, Cells: var c1 } m1 }, { Map: { Digits: var q, Cells: var c2 } m2 }))
		{
			return [];
		}

		switch (m1, m2)
		{
			case ([var candidate1], [var candidate2]):
			{
				var (cell1, digit1) = (candidate1 / 9, candidate1 % 9);
				var (cell2, digit2) = (candidate2 / 9, candidate2 % 9);
				if (cell1 == cell2)
				{
					// Same cell.
					return
						from digit in (Mask)(grid.GetCandidates(cell1) & ~(1 << digit1 | 1 << digit2))
						select new Conclusion(Elimination, cell1, digit);
				}
				else if (digit1 == digit2)
				{
					// Same digit.
					return
						from cell in (cell1.AsCellMap() + cell2).PeerIntersection & candidatesMap[digit1]
						select new Conclusion(Elimination, cell, digit1);
				}
				else
				{
					// Otherwise (Different cell and digit).
					var result = new List<Conclusion>(2);
					if ((grid.GetCandidates(cell1) >> digit2 & 1) != 0)
					{
						result.Add(new(Elimination, cell1, digit2));
					}
					if ((grid.GetCandidates(cell2) >> digit1 & 1) != 0)
					{
						result.Add(new(Elimination, cell2, digit1));
					}
					return result.AsReadOnlySpan();
				}
			}
			case var _ when Mask.IsPow2(p) && Mask.IsPow2(q) && p == q:
			{
				var digit = Mask.Log2(p);
				return from cell in (c1 | c2).PeerIntersection & candidatesMap[digit] select new Conclusion(Elimination, cell, digit);
			}
			default:
			{
				return [];
			}
		}
	}
}
