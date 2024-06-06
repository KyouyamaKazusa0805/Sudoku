namespace Sudoku.Concepts;

/// <summary>
/// Represents a chain or a loop.
/// </summary>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.AllOperators, OtherModifiersOnEquals = "sealed")]
public abstract partial class ChainPattern :
	IComparable<ChainPattern>,
	ICoordinateConvertible<ChainPattern>,
	IEnumerable<Node>,
	IEquatable<ChainPattern>,
	IFormattable,
	IReadOnlyList<Node>,
	IReadOnlyCollection<Node>
{
	/// <summary>
	/// Indicates the possible inferences to be used.
	/// </summary>
	private protected static readonly Inference[] Inferences = [Inference.Strong, Inference.Weak];


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
	/// Initializes <see cref="ChainPattern"/> data.
	/// </summary>
	/// <param name="lastNode">The last node.</param>
	/// <param name="isLoop">Indicates whether is for loop initialization.</param>
	/// <param name="strongLinkDictionary">Indicates the strong link dictionary.</param>
	/// <param name="weakLinkDictionary">Indicates the weak link dictionary.</param>
	protected ChainPattern(Node lastNode, bool isLoop, LinkDictionary strongLinkDictionary, LinkDictionary weakLinkDictionary)
	{
		(_strongGroupedLinkPool, _weakGroupedLinkPool) = (strongLinkDictionary.GroupedLinkPool, weakLinkDictionary.GroupedLinkPool);
		var nodes = new List<Node> { lastNode };
		for (var node = lastNode.Parent!; isLoop ? node != lastNode : node is not null; node = node.Parent!)
		{
			nodes.Add(new Node(node, null));
		}
		_nodes = [.. nodes];

		// Reverse the whole chain if the first node is greater than the last node in logic.
		if (nodes[1].CompareTo(nodes[^2], NodeComparison.IgnoreIsOn) >= 0)
		{
			Reverse();
		}
	}


	/// <summary>
	/// Indicates whether the chain pattern uses grouped logic.
	/// </summary>
	public abstract bool IsGrouped { get; }

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
			return IsPow2(digitsMask);
		}
	}

	/// <summary>
	/// Indicates whether the pattern only uses cell strong links.
	/// </summary>
	public bool SatisfyYRule
	{
		get
		{
			foreach (var link in Links)
			{
				switch (link)
				{
					case { IsStrong: false }:
					{
						continue;
					}
					case { FirstNode.Map.Cells: [var cell1], SecondNode.Map.Cells: [var cell2] } when cell1 != cell2:
					{
						return false;
					}
				}
			}
			return true;
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
	public abstract int Length { get; }

	/// <summary>
	/// Indicates the complexity of the pattern.
	/// The value is different with <see cref="Length"/> on a chain starting and ending with itself, both are by strong links.
	/// </summary>
	public abstract int Complexity { get; }

	/// <summary>
	/// Indicates the links used.
	/// </summary>
	public abstract ReadOnlySpan<Link> Links { get; }

	/// <summary>
	/// Indicates the head node.
	/// </summary>
	public Node First => ValidNodes[0];

	/// <summary>
	/// Indicates the tail node.
	/// </summary>
	public Node Last => ValidNodes[^1];

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
	public abstract Node this[int index] { get; }


	/// <summary>
	/// Try to reverse the pattern, making all nodes negated its direction connected.
	/// </summary>
	public abstract void Reverse();

	/// <inheritdoc/>
	public abstract bool Equals(ChainPattern? other);

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
	public abstract bool Equals([NotNullWhen(true)] ChainPattern? other, NodeComparison nodeComparison, ChainPatternComparison patternComparison);

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
	/// This method directly calls <see cref="GetHashCode(NodeComparison, ChainPatternComparison)"/>
	/// with values <see cref="NodeComparison.IgnoreIsOn"/> and <see cref="ChainPatternComparison.Undirected"/>.
	/// </remarks>
	/// <seealso cref="GetHashCode(NodeComparison, ChainPatternComparison)"/>
	/// <seealso cref="NodeComparison.IgnoreIsOn"/>
	/// <seealso cref="ChainPatternComparison.Undirected"/>
	public sealed override int GetHashCode() => GetHashCode(NodeComparison.IgnoreIsOn, ChainPatternComparison.Undirected);

	/// <summary>
	/// Computes hash code based on the current instance.
	/// </summary>
	/// <param name="nodeComparison">The node comparison.</param>
	/// <param name="patternComparison">The pattern comparison.</param>
	/// <returns>An <see cref="int"/> as the result.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="patternComparison"/> is not defined.
	/// </exception>
	public abstract int GetHashCode(NodeComparison nodeComparison, ChainPatternComparison patternComparison);

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
	public abstract int CompareTo(ChainPattern? other);

	/// <inheritdoc cref="object.ToString"/>
	public abstract override string ToString();

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(IFormatProvider? formatProvider) => ToString(null, formatProvider);

	/// <inheritdoc/>
	public abstract string ToString(string? format, IFormatProvider? formatProvider);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString<T>(T converter) where T : CoordinateConverter => ToString(null, converter);

	/// <summary>
	/// Returns a string that represents the current object.
	/// </summary>
	/// <typeparam name="T">The type of the converter.</typeparam>
	/// <param name="format">The format text.</param>
	/// <param name="converter">The converter instance.</param>
	/// <returns>A string that represents the current object.</returns>
	public abstract string ToString<T>(string? format, T converter) where T : CoordinateConverter;

	/// <summary>
	/// Slices the collection with the specified start node and its length.
	/// </summary>
	/// <param name="start">The start index.</param>
	/// <param name="length">The number of <see cref="Node"/> instances to slice.</param>
	/// <returns>A <see cref="ReadOnlySpan{T}"/> of <see cref="Node"/> instances returned.</returns>
	public abstract ReadOnlySpan<Node> Slice(int start, int length);

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(this);

	/// <summary>
	/// Try to get a <see cref="ConclusionSet"/> instance that contains all conclusions created by using the current chain.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <returns>A <see cref="ConclusionSet"/> instance.</returns>
	public abstract ConclusionSet GetConclusions(ref readonly Grid grid);

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => _nodes.GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<Node> IEnumerable<Node>.GetEnumerator() => ((IEnumerable<Node>)_nodes).GetEnumerator();


	/// <summary>
	/// Try to get all possible conclusions via the specified grid and two <see cref="Node"/> instances.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="node1">The first node.</param>
	/// <param name="node2">The second node.</param>
	/// <returns>A sequence of <see cref="Conclusion"/> instances.</returns>
	/// <seealso cref="Conclusion"/>
	private protected static ReadOnlySpan<Conclusion> GetConclusions(ref readonly Grid grid, Node node1, Node node2)
	{
		var candidatesMap = grid.CandidatesMap;
		if (node1 == ~node2)
		{
			// Two nodes are same, meaning the node must be true. Check whether it is grouped one.
			var digit = node1.Map[0] % 9;
			var map = Subview.ReduceCandidateByDigit(in node1.Map, digit);
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
						from digit in (Mask)(grid.GetCandidates(cell1) & (Mask)~(1 << digit1 | 1 << digit2))
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
			case var _ when IsPow2(p) && IsPow2(q) && p == q:
			{
				var digit = Log2((uint)p);
				return from cell in (c1 | c2).PeerIntersection & candidatesMap[digit] select new Conclusion(Elimination, cell, digit);
			}
			default:
			{
				return [];
			}
		}
	}
}
