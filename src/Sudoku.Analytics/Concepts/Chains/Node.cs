namespace Sudoku.Concepts;

/// <summary>
/// Represents a chain node.
/// </summary>
/// <param name="map">Indicates the backing map.</param>
/// <param name="isOn">Indicates whether the node is on.</param>
/// <param name="isAdvanced">
/// Indicates whether the node is advanced one. Please note that the property won't participate comparison rules.
/// </param>
[TypeImpl(TypeImplFlag.AllObjectMethods | TypeImplFlag.AllOperators)]
public sealed partial class Node(
	[PrimaryConstructorParameter(MemberKinds.Field), HashCodeMember] ref readonly CandidateMap map,
	[PrimaryConstructorParameter, HashCodeMember] bool isOn,
	[PrimaryConstructorParameter] bool isAdvanced
) :
	IComparable<Node>,
	IComparisonOperators<Node, Node, bool>,
	IEquatable<Node>,
	IEqualityOperators<Node, Node, bool>,
	IFormattable
{
	/// <summary>
	/// Indicates the map format string.
	/// </summary>
	private const string MapFormatString = "m";

	/// <summary>
	/// Indicates the property <see cref="IsOn"/> format string.
	/// </summary>
	private const string IsOnFormatString = "s";


	/// <summary>
	/// Initializes a <see cref="Node"/> instance via the specified candidate.
	/// </summary>
	/// <param name="candidate">A candidate.</param>
	/// <param name="isOn">Indicates whether the node is on.</param>
	/// <param name="isAdvanced">Indicates whether the node is advanced.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Node(Candidate candidate, bool isOn, bool isAdvanced) : this(candidate.AsCandidateMap(), isOn, isAdvanced)
	{
	}

	/// <summary>
	/// Initializes a <see cref="Node"/> instance via the specified cell and digit.
	/// </summary>
	/// <param name="cell">A cell.</param>
	/// <param name="digit">A digit.</param>
	/// <param name="isOn">Indicates whether the node is on.</param>
	/// <param name="isAdvanced">Indicates whether the node is advanced.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Node(Cell cell, Digit digit, bool isOn, bool isAdvanced) : this(cell * 9 + digit, isOn, isAdvanced)
	{
	}

	/// <summary>
	/// Copies and creates a <see cref="Node"/> instance from argument <paramref name="base"/>,
	/// and appends its parent node.
	/// </summary>
	/// <param name="base">The data provider.</param>
	/// <param name="parent">The parent node.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Node(Node @base, Node? parent) : this(in @base._map, @base.IsOn, @base.IsAdvanced) => Parent = parent;

	/// <summary>
	/// Copies and creates a <see cref="Node"/> instance from argument <paramref name="base"/>,
	/// and appends its parent node, and modify <see cref="IsOn"/> property value.
	/// </summary>
	/// <param name="base">The data provider.</param>
	/// <param name="isOn">Indicates whether the node is on.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Node(Node @base, bool isOn) : this(in @base._map, isOn, @base.IsAdvanced) => Parent = @base.Parent;


	/// <summary>
	/// Indicates whether the node is a grouped node.
	/// </summary>
	public bool IsGroupedNode => _map.Count >= 2;

	/// <summary>
	/// Indicates the length of ancestors.
	/// </summary>
	public int AncestorsLength
	{
		get
		{
			var result = 0;
			for (var node = this; node is not null; node = node.Parent)
			{
				result++;
			}
			return result;
		}
	}

	/// <summary>
	/// Indicates the map of candidates the node uses.
	/// </summary>
	public ref readonly CandidateMap Map => ref _map;

	/// <summary>
	/// Indicates the parent node. The value doesn't participate in equality comparison.
	/// </summary>
	public Node? Parent { get; set; }

	/// <summary>
	/// The backing comparing value on <see cref="IsOn"/> property.
	/// </summary>
	/// <seealso cref="IsOn"/>
	private int IsOnPropertyValue => IsOn ? 1 : 0;


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out bool isGroupedNode, out CandidateMap map) => (isGroupedNode, map) = (IsGroupedNode, _map);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out bool isGroupedNode, out CandidateMap map, out Node? parent)
		=> ((isGroupedNode, map), parent) = (this, Parent);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] Node? other) => Equals(other, NodeComparison.IncludeIsOn);

	/// <summary>
	/// Compares with two <see cref="Node"/> instances, based on the specified comparison rule.
	/// </summary>
	/// <param name="other">The other instance to be compared.</param>
	/// <param name="comparison">The comparison rule.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="comparison"/> is not defined.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] Node? other, NodeComparison comparison)
		=> other is not null
		&& comparison switch
		{
			NodeComparison.IncludeIsOn => _map == other._map && IsOn == other.IsOn,
			NodeComparison.IgnoreIsOn => _map == other._map,
			_ => throw new ArgumentOutOfRangeException(nameof(comparison))
		};

	/// <summary>
	/// Determines whether the current node is an ancestor of the specified node. 
	/// </summary>
	/// <param name="childNode">The node to be checked.</param>
	/// <param name="nodeComparison">The comparison rule on nodes.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public bool IsAncestorOf(Node childNode, NodeComparison nodeComparison)
	{
		for (var node = childNode; node is not null; node = node.Parent)
		{
			if (Equals(node, nodeComparison))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Calculates the hash code on the current instance.
	/// </summary>
	/// <param name="comparison">The comparison rule.</param>
	/// <returns>An <see cref="int"/> value as the result.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="comparison"/> is not defined.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetHashCode(NodeComparison comparison)
		=> comparison switch
		{
			NodeComparison.IncludeIsOn => HashCode.Combine(_map.GetHashCode(), IsOn),
			NodeComparison.IgnoreIsOn => HashCode.Combine(_map.GetHashCode()),
			_ => throw new ArgumentOutOfRangeException(nameof(comparison))
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Node? other) => CompareTo(other, NodeComparison.IgnoreIsOn);

	/// <inheritdoc cref="CompareTo(Node?)"/>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="comparison"/> is not defined.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Node? other, NodeComparison comparison)
		=> other is null
			? -1
			: comparison switch
			{
				NodeComparison.IncludeIsOn => IsOnPropertyValue.CompareTo(other.IsOnPropertyValue) switch
				{
					var r and not 0 => r,
					_ => _map.CompareTo(in other._map)
				},
				NodeComparison.IgnoreIsOn => _map.CompareTo(in other._map),
				_ => throw new ArgumentOutOfRangeException(nameof(comparison))
			};

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(IFormatProvider? formatProvider)
	{
		var converter = CoordinateConverter.GetConverter(formatProvider);
		return $"{converter.CandidateConverter(_map)}: {IsOn}";
	}

	/// <inheritdoc/>
	/// <remarks>
	/// Format description:
	/// <list type="table">
	/// <listheader>
	/// <term>Format character</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term><c>m</c></term>
	/// <description>The map text. For example, <c>r1c23(4)</c></description>
	/// </item>
	/// <item>
	/// <term><c>S</c> and <c>s</c></term>
	/// <description>
	/// The <see cref="IsOn"/> property value (<see langword="true"/> or <see langword="false"/>).
	/// If the character <c>s</c> is upper-cased, the result text will be upper-cased on initial letter.
	/// </description>
	/// </item>
	/// </list>
	/// For example, format value <c>"m: S"</c> will be replaced with value <c>"r1c23(4): True"</c>.
	/// </remarks>
	public string ToString(string? format, IFormatProvider? formatProvider)
		=> (format ?? $"{MapFormatString}: {IsOnFormatString}")
			.Replace(MapFormatString, _map.ToString(formatProvider))
			.Replace(IsOnFormatString, IsOn.ToString().ToLower());

	/// <summary>
	/// Creates a copy of the current instance.
	/// </summary>
	/// <returns>A cloned instance whose internal values are same as the current instance, independent.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Node Clone() => new(in _map, IsOn, IsAdvanced) { Parent = Parent };

	/// <summary>
	/// <para>
	/// Find all possible <see cref="ChainOrLoop"/> patterns starting with the current node,
	/// and to make an confliction with itself.
	/// </para>
	/// <para>
	/// This method will return <see langword="null"/> if <paramref name="onlyFindOne"/> is <see langword="false"/>,
	/// or the method cannot find a valid chain that forms a confliction with the current node.
	/// </para>
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="onlyFindOne">Indicates whether the method only find one valid <see cref="ChainOrLoop"/> and return.</param>
	/// <param name="result">
	/// A collection that stores all possible found <see cref="ChainOrLoop"/> patterns
	/// if <paramref name="onlyFindOne"/> is <see langword="false"/>.
	/// </param>
	/// <returns>The first found <see cref="ChainOrLoop"/> pattern.</returns>
	/// <seealso cref="ChainOrLoop"/>
	internal ChainOrLoop? FindChains(ref readonly Grid grid, bool onlyFindOne, SortedSet<ChainOrLoop> result)
	{
		var pendingNodesSupposedOn = new LinkedList<Node>();
		var pendingNodesSupposedOff = new LinkedList<Node>();
		(IsOn ? pendingNodesSupposedOff : pendingNodesSupposedOn).AddLast(this);

		var visitedNodesSupposedOn = new HashSet<Node>(CachedChainingComparers.NodeMapComparer);
		var visitedNodesSupposedOff = new HashSet<Node>(CachedChainingComparers.NodeMapComparer);
		_ = (visitedNodesSupposedOn.Add(this), visitedNodesSupposedOff.Add(this));

		while (pendingNodesSupposedOn.Count != 0 || pendingNodesSupposedOff.Count != 0)
		{
			while (pendingNodesSupposedOn.Count != 0)
			{
				var currentNode = pendingNodesSupposedOn.RemoveFirstNode();
				if (WeakLinkDictionary.TryGetValue(currentNode, out var nodesSupposedOff))
				{
					foreach (var nodeSupposedOff in nodesSupposedOff)
					{
						var nextNode = new Node(nodeSupposedOff, currentNode);

						////////////////////////////////////////////
						// Continuous Nice Loop 3) Strong -> Weak //
						////////////////////////////////////////////
						if (nodeSupposedOff == this && nextNode.AncestorsLength >= 4)
						{
							var loop = new Loop(nextNode);
							if (!loop.GetConclusions(in grid).IsWorthFor(in grid))
							{
								goto Next;
							}

							if (onlyFindOne)
							{
								return loop;
							}

							result.Add(loop);
						Next:;
						}

						/////////////////////////////////////////////////
						// Discontinuous Nice Loop 2) Strong -> Strong //
						/////////////////////////////////////////////////
						if (nodeSupposedOff == ~this)
						{
							var chain = new Chain(nextNode);
							if (!chain.GetConclusions(in grid).IsWorthFor(in grid))
							{
								goto Next;
							}

							if (onlyFindOne)
							{
								return chain;
							}
							result.Add(chain);
						Next:;
						}

						// This step will filter duplicate nodes in order not to make a internal loop on chains.
						// The second argument must be 'NodeComparison.IgnoreIsOn' because we should explicitly ignore them
						// no matter what state the node is.
						// This will fix issue #673:
						//   * https://github.com/SunnieShine/Sudoku/issues/673
						// Counter-example:
						//   4.+3.6+85...+57.....8+89.5...3..7..+8+6.2.23..94.+8..+84.....15..6..8+7+3+3..+871.5.+7+68.....2:114 124 324 425 427 627 943 366 667 967 272 273 495 497
						if (!nodeSupposedOff.IsAncestorOf(currentNode, NodeComparison.IgnoreIsOn)
							&& visitedNodesSupposedOff.Add(nodeSupposedOff))
						{
							pendingNodesSupposedOff.AddLast(nextNode);
						}
					}
				}
			}
			while (pendingNodesSupposedOff.Count != 0)
			{
				var currentNode = pendingNodesSupposedOff.RemoveFirstNode();
				if (StrongLinkDictionary.TryGetValue(currentNode, out var nodesSupposedOn))
				{
					foreach (var nodeSupposedOn in nodesSupposedOn)
					{
						var nextNode = new Node(nodeSupposedOn, currentNode);

						/////////////////////////////////////////////
						// Discontinuous Nice Loop 1) Weak -> Weak //
						/////////////////////////////////////////////
						if (nodeSupposedOn == ~this)
						{
							var chain = new Chain(nextNode);
							if (!chain.GetConclusions(in grid).IsWorthFor(in grid))
							{
								goto Next;
							}

							if (onlyFindOne)
							{
								return chain;
							}
							result.Add(chain);
						Next:;
						}

						if (!nodeSupposedOn.IsAncestorOf(currentNode, NodeComparison.IgnoreIsOn)
							&& visitedNodesSupposedOn.Add(nodeSupposedOn))
						{
							pendingNodesSupposedOn.AddLast(nextNode);
						}
					}
				}
			}
		}
		return null;
	}

	/// <summary>
	/// <para>Finds a list of nodes that can implicitly connects to current node via a forcing chain.</para>
	/// <para>This method only uses cached fields <see cref="StrongLinkDictionary"/> and <see cref="WeakLinkDictionary"/>.</para>
	/// </summary>
	/// <returns>
	/// Returns a pair of <see cref="HashSet{T}"/> of <see cref="Node"/> instances, indicating all possible nodes
	/// that can implicitly connects to the current node via the whole forcing chain, grouped by their own initial states.
	/// </returns>
	/// <seealso cref="StrongLinkDictionary"/>
	/// <seealso cref="WeakLinkDictionary"/>
	internal (HashSet<Node> OnNodes, HashSet<Node> OffNodes) FindForcingChains()
	{
		var (pendingNodesSupposedOn, pendingNodesSupposedOff) = (new LinkedList<Node>(), new LinkedList<Node>());
		(IsOn ? pendingNodesSupposedOn : pendingNodesSupposedOff).AddLast(this);

		var nodesSupposedOn = new HashSet<Node>(CachedChainingComparers.NodeMapComparer);
		var nodesSupposedOff = new HashSet<Node>(CachedChainingComparers.NodeMapComparer);
		while (pendingNodesSupposedOn.Count != 0 || pendingNodesSupposedOff.Count != 0)
		{
			if (pendingNodesSupposedOn.Count != 0)
			{
				var currentNode = pendingNodesSupposedOn.RemoveFirstNode();
				if (WeakLinkDictionary.TryGetValue(currentNode, out var supposedOff))
				{
					foreach (var node in supposedOff)
					{
						var nextNode = new Node(node, currentNode);
						if (nodesSupposedOn.Contains(~nextNode))
						{
							// Contradiction is found.
							goto ReturnResult;
						}

						if (nodesSupposedOff.Add(nextNode))
						{
							pendingNodesSupposedOff.AddLast(nextNode);
						}
					}
				}
			}
			else
			{
				var currentNode = pendingNodesSupposedOff.RemoveFirstNode();
				if (StrongLinkDictionary.TryGetValue(currentNode, out var supposedOn))
				{
					foreach (var node in supposedOn)
					{
						var nextNode = new Node(node, currentNode);
						if (nodesSupposedOff.Contains(~nextNode))
						{
							// Contradiction is found.
							goto ReturnResult;
						}

						if (nodesSupposedOn.Add(nextNode))
						{
							pendingNodesSupposedOn.AddLast(nextNode);
						}
					}
				}
			}
		}

	ReturnResult:
		// Returns the found result.
		return (nodesSupposedOn, nodesSupposedOff);
	}


	/// <summary>
	/// Negates the node with <see cref="IsOn"/> property value.
	/// </summary>
	/// <param name="value">The current node.</param>
	/// <returns>The node negated.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Node operator ~(Node value) => new(value, !value.IsOn);
}
