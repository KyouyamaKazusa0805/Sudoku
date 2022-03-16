using Sudoku.Collections;

namespace Sudoku.Test;

/// <summary>
/// Defines a data structure that can automatically inference the strong and weak inferences.
/// </summary>
public readonly partial struct AlternatingInferenceChain :
	IEnumerable<Node>,
	IEquatable<AlternatingInferenceChain>,
	IReadOnlyCollection<Node>,
	IReadOnlyList<Node>
#if FEATURE_GENERIC_MATH
	,
	IEqualityOperators<AlternatingInferenceChain, AlternatingInferenceChain>
#endif
{
	/// <summary>
	/// Indicates whether the chain starts with the weak inference.
	/// </summary>
	private readonly bool _startsWithWeak;

	/// <summary>
	/// Indicates the inner chain nodes stored.
	/// </summary>
	private readonly Node[] _nodes;


	/// <summary>
	/// Initializes an <see cref="AlternatingInferenceChain"/> instance via the specified nodes.
	/// </summary>
	/// <param name="nodes">The nodes.</param>
	/// <param name="startsWithWeakInference">
	/// Indicates whether the chain starts with the weak inference. The default value is <see langword="false"/>.
	/// </param>
	/// <exception cref="ArgumentException">
	/// Throws when the length of the argument <paramref name="nodes"/> is less than 3,
	/// or the first and the last node in the argument <paramref name="nodes"/> aren't same
	/// if the argument <paramref name="startsWithWeakInference"/> is <see langword="false"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public AlternatingInferenceChain(Node[] nodes, bool startsWithWeakInference = false)
	{
		if (nodes.Length < 3)
		{
			throw new ArgumentException(
				$"The length of the argument '{nameof(nodes)}' must be greater than 3.",
				nameof(nodes)
			);
		}

		if (!startsWithWeakInference && nodes[0] != nodes[^1])
		{
			throw new ArgumentException(
				"If the alternating inference chain starts with the strong inference, " +
				"the first and the last node should be the same."
			);
		}

		_nodes = nodes;
		_startsWithWeak = startsWithWeakInference;
	}


	/// <summary>
	/// Indicates the number of <see cref="Node"/>s stored in this collection.
	/// </summary>
	public int Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _nodes.Length;
	}

	/// <summary>
	/// Indicates the possible eliminations or assignments that can be concluded through the current chain.
	/// </summary>
	/// <param name="grid">The grid as the candidate template.</param>
	public Conclusion[]? GetConclusions(in Grid grid)
	{
		if (!_startsWithWeak)
		{
			// If '_startsWithWeak' is false, the chain is the special AIC
			// that the head and tail is a same node.
			// If the head (or tail) node is a sole candidate, we can conclude that
			// the sole candidate is true; otherwise, the structure of the node is true.
			byte d = _nodes[0].Digit;
			if (_nodes[0].Cells is var cells && cells is [var c])
			{
				return new Conclusion[] { new(ConclusionType.Assignment, c * 9 + d) };
			}

			return getEliminationsSingleDigit(!cells & grid.CandidatesMap[d], d);
		}

		if (RealChainNodes is not [{ Cells: var c1, Digit: var d1 }, .., { Cells: var c2, Digit: var d2 }])
		{
			// If the 'RealChainNodes' is invalid, just return null.
			return null;
		}

		if (d1 == d2)
		{
			// If 'd1' is equal to 'd2', the head and tail node uses the same digit.
			// Therefore, we can get the eliminations more easily.
			var elimMap = !c1 & !c2 & grid.CandidatesMap[d1];
			if (elimMap is [])
			{
				return null;
			}

			return getEliminationsSingleDigit(elimMap, d1);
		}

		switch ((c1, c2))
		{
			case ( [var oc1], [var oc2]):
			{
				return getEliminationsMultipleDigits(grid, oc1, oc2, d1, d2);
			}
			case ( [var oc], { Count: > 1 }) when grid.Exists(oc, d2) is true:
			{
				return new Conclusion[] { new(ConclusionType.Elimination, oc, d2) };
			}
			case ({ Count: > 1 }, [var oc]) when grid.Exists(oc, d1) is true:
			{
				return new Conclusion[] { new(ConclusionType.Elimination, oc, d1) };
			}
			default:
			{
				return null;
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static Conclusion[] getEliminationsSingleDigit(in Cells elimMap, byte d)
		{
			int i = 0;
			var result = new Conclusion[elimMap.Count];
			foreach (int cell in elimMap)
			{
				result[i++] = new(ConclusionType.Elimination, cell * 9 + d);
			}

			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static Conclusion[] getEliminationsMultipleDigits(in Grid grid, int oc1, int oc2, byte d1, byte d2)
		{
			using var resultList = new ValueList<Conclusion>(2);
			if (grid.Exists(oc1, d2) is true)
			{
				resultList.Add(new(ConclusionType.Elimination, oc1, d2));
			}
			if (grid.Exists(oc2, d1) is true)
			{
				resultList.Add(new(ConclusionType.Elimination, oc2, d1));
			}

			return resultList.ToArray();
		}
	}

	/// <summary>
	/// Indicates the <see cref="Node"/>s that ignores the first and the last node.
	/// </summary>
	public ImmutableArray<Node> RealChainNodes
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ImmutableArray.Create(_startsWithWeak ? _nodes[1..^1] : _nodes);
	}


	/// <summary>
	/// Gets the result <see cref="Node"/> instance at the specified index.
	/// </summary>
	/// <param name="index">The desired index value.</param>
	/// <returns>The result <see cref="Node"/> instance.</returns>
	public Node this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _nodes[index];
	}


	/// <inheritdoc cref="object.Equals(object?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) =>
		obj is AlternatingInferenceChain comparer && Equals(comparer);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(AlternatingInferenceChain other) =>
		_startsWithWeak == other._startsWithWeak
			&& RealChainNodes is [var a1, .., var b1] && other.RealChainNodes is [var a2, .., var b2]
			&& (a1 == a2 && b1 == b2 || a1 == b2 && a2 == b1);

	/// <summary>
	/// Indicates whether the current object has the same value and the same length of the chain
	/// with the other specified chain.
	/// </summary>
	/// <param name="other">The other specified chain to compare.</param>
	/// <returns>
	/// <see langword="true"/> if the current object is equal to <paramref name="other"/> parameter;
	/// otherwise, <see langword="false"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ExactlyEquals(AlternatingInferenceChain other) =>
		Equals(other) && _nodes.Length == other._nodes.Length;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(RealChainNodes[0], RealChainNodes[^1], _startsWithWeak);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		var sb = new StringHandler();
		var realChain = RealChainNodes;
		for (int i = 0, length = realChain.Length; i < length; i++)
		{
			sb.Append(realChain[i].ToSimpleString());
			sb.Append((i & 1) == 0 ? " == " : " -- ");
		}

		sb.RemoveFromEnd(4);

		return sb.ToStringAndClear();
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(_nodes);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => _nodes.GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<Node> IEnumerable<Node>.GetEnumerator() => ((IEnumerable<Node>)_nodes).GetEnumerator();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(AlternatingInferenceChain left, AlternatingInferenceChain right) =>
		left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(AlternatingInferenceChain left, AlternatingInferenceChain right) =>
		!(left == right);
}
