namespace Sudoku.Concepts;

/// <summary>
/// Represents a type that describes for a chain or a loop.
/// </summary>
public interface IChainPattern :
#if false
	IComparable<IChainPattern>,
#endif
	IEnumerable<Node>,
#if false
	IEquatable<IChainPattern>,
#endif
	IFormattable
{
	/// <summary>
	/// Indicates the possible inferences to be used.
	/// </summary>
	protected static readonly Inference[] Inferences = [Inference.Strong, Inference.Weak];


	/// <summary>
	/// Indicates whether the chain pattern uses grouped logic.
	/// </summary>
	public abstract bool IsGrouped { get; }

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
	/// Indicates the head node.
	/// </summary>
	public abstract Node First { get; }

	/// <summary>
	/// Indicates the tail node.
	/// </summary>
	public abstract Node Last { get; }

	/// <summary>
	/// Indicates the backing nodes.
	/// </summary>
	protected abstract Node[] BackingNodes { get; }


	/// <summary>
	/// Gets a <see cref="Node"/> instance at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>The <see cref="Node"/> instance.</returns>
	/// <exception cref="IndexOutOfRangeException">Throws when the argument <paramref name="index"/> is out of range.</exception>
	public abstract Node this[int index] { get; }


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
	public abstract bool Equals([NotNullWhen(true)] IChainPattern? other, NodeComparison nodeComparison, ChainPatternComparison patternComparison);

	/// <inheritdoc cref="object.GetHashCode"/>
	public abstract int GetHashCode();

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

	/// <inheritdoc cref="object.ToString"/>
	public abstract string ToString();

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	public abstract string ToString(IFormatProvider? formatProvider);

	/// <summary>
	/// Slices the collection with the specified start node and its length.
	/// </summary>
	/// <param name="start">The start index.</param>
	/// <param name="length">The number of <see cref="Node"/> instances to slice.</param>
	/// <returns>A <see cref="ReadOnlySpan{T}"/> of <see cref="Node"/> instances returned.</returns>
	public abstract ReadOnlySpan<Node> Slice(int start, int length);

	/// <summary>
	/// Try to get a <see cref="ConclusionSet"/> instance that contains all conclusions created by using the current chain.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <returns>A <see cref="ConclusionSet"/> instance.</returns>
	public abstract ConclusionSet GetConclusions(ref readonly Grid grid);


	/// <summary>
	/// Try to get all possible conclusions via the specified grid and two <see cref="Node"/> instances.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="node1">The first node.</param>
	/// <param name="node2">The second node.</param>
	/// <returns>A sequence of <see cref="Conclusion"/> instances.</returns>
	/// <seealso cref="Conclusion"/>
	protected static sealed ReadOnlySpan<Conclusion> GetConclusions(ref readonly Grid grid, Node node1, Node node2)
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

				var result = new List<Conclusion>();
				if (digit1 == digit2)
				{
					// Same digit.
					foreach (var cell in (cell1.AsCellMap() + cell2).PeerIntersection & candidatesMap[digit1])
					{
						result.Add(new(Elimination, cell, digit1));
					}
					return result.AsReadOnlySpan();
				}

				// Not same digit.
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
