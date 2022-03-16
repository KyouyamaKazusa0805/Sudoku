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


	/// <summary>
	/// Indicates the possible eliminations or assignments that can be concluded through the current chain.
	/// </summary>
	/// <param name="grid">The grid as the candidate template.</param>
	/// <remarks><i>
	/// <para>TODO: Append extra eliminations that requires the extended elimination rules.</para>
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
	/// </i></remarks>
	public Conclusion[]? GetConclusions(in Grid grid) =>
		_startsWithWeak switch
		{
			true => RealChainNodes switch
			{
				[{ Cells: var c1, Digit: var d1 }, .., { Cells: var c2, Digit: var d2 }] => (d1 == d2) switch
				{
					true => (!c1 & !c2 & grid.CandidatesMap[d1]) switch
					{
						{ Count: not 0 } elimMap => GetEliminationsSingleDigit(elimMap, d1),
						_ => null
					},
					_ => (c1, c2) switch
					{
#pragma warning disable IDE0055
						([var oc1], [var oc2]) => GetEliminationsMultipleDigits(grid, oc1, oc2, d1, d2),
						([var oc], { Count: > 1 }) => grid.Exists(oc, d2) switch
						{
							true => new Conclusion[] { new(ConclusionType.Elimination, oc, d2) },
							_ => null
						},
#pragma warning restore IDE0055
						({ Count: > 1 }, [var oc]) => grid.Exists(oc, d1) switch
						{
							true => new Conclusion[] { new(ConclusionType.Elimination, oc, d1) },
							_ => null
						},
						_ => null
					}
				},
				_ => null
			},
			_ => _nodes switch
			{
				[{ Cells: var cells, Digit: var d }, ..] => cells switch
				{
					[var c] => new Conclusion[] { new(ConclusionType.Assignment, c * 9 + d) },
					_ => GetEliminationsSingleDigit(!cells & grid.CandidatesMap[d], d)
				},
				_ => null
			}
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => _nodes.GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<Node> IEnumerable<Node>.GetEnumerator() => ((IEnumerable<Node>)_nodes).GetEnumerator();


	/// <summary>
	/// Try to get eliminations via different digits.
	/// </summary>
	/// <param name="grid">The grid as the candidate reference.</param>
	/// <param name="c1">The only cell in the first node.</param>
	/// <param name="c2">The only cell in the second node.</param>
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <returns>The conclusions.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Conclusion[] GetEliminationsMultipleDigits(in Grid grid, int c1, int c2, byte d1, byte d2)
	{
		using var resultList = new ValueList<Conclusion>(2);
		if (grid.Exists(c1, d2) is true)
		{
			resultList.Add(new(ConclusionType.Elimination, c1, d2));
		}
		if (grid.Exists(c2, d1) is true)
		{
			resultList.Add(new(ConclusionType.Elimination, c2, d1));
		}

		return resultList.ToArray();
	}

	/// <summary>
	/// Try to get eliminations via the digit and the elimination cells.
	/// </summary>
	/// <param name="elimMap">The elimination cells.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The conclusions.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Conclusion[] GetEliminationsSingleDigit(in Cells elimMap, byte digit)
	{
		int i = 0;
		var result = new Conclusion[elimMap.Count];
		foreach (int cell in elimMap)
		{
			result[i++] = new(ConclusionType.Elimination, cell * 9 + digit);
		}

		return result;
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(AlternatingInferenceChain left, AlternatingInferenceChain right) =>
		left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(AlternatingInferenceChain left, AlternatingInferenceChain right) =>
		!(left == right);
}
