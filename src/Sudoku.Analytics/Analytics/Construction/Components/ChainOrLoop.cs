namespace Sudoku.Analytics.Construction.Components;

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
	/// Indicates the strong link connector.
	/// </summary>
	public const string StrongLinkConnector = " == ";

	/// <summary>
	/// Indicates the weak link connector.
	/// </summary>
	public const string WeakLinkConnector = " -- ";


	/// <summary>
	/// Indicates the possible inferences to be used.
	/// </summary>
	protected internal static readonly Inference[] Inferences = [Inference.Strong, Inference.Weak];


	/// <summary>
	/// Indicates the nodes to be initialized.
	/// </summary>
	protected readonly Node[] _nodes;


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
				var pool = (isStrong ? StrongLinkDictionary : WeakLinkDictionary).GroupedLinkPool;
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
	protected internal abstract int WeakStartIdentity { get; }

	/// <summary>
	/// Indicates the valid nodes to be used.
	/// </summary>
	protected internal abstract ReadOnlySpan<Node> ValidNodes { get; }

	/// <summary>
	/// Indicates the value on loop checking for link construction usages.
	/// </summary>
	protected abstract int LoopIdentity { get; }

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
	public string ToString(IFormatProvider? formatProvider)
		=> formatProvider switch
		{
			ChainOrLoopFormatInfo f => ChainOrLoopFormatInfo.FormatCoreUnsafeAccessor(f, this),
			_ => ToString(ChainOrLoopFormatInfo.Standard)
		};

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
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => _nodes.GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<Node> IEnumerable<Node>.GetEnumerator() => _nodes.AsEnumerable().GetEnumerator();
}
