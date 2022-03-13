using Sudoku.Collections;

namespace Sudoku.Test;

/// <summary>
/// Defines a data structure that can automatically inference the strong and weak inferences.
/// </summary>
public readonly partial struct AlternatingInferenceChain :
	ICloneable,
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
	public Conclusion[]? GetConclusions(in Grid grid) =>
		this switch
		{
			{
				_startsWithWeak: true,
				RealChain:
				[
					SoleCandidateNode { Candidate: var c1 },
					..,
					SoleCandidateNode { Candidate: var c2 }
				]
			}
			when grid is var tempGrid =>
			(
				from elimination in !new Candidates { c1, c2 }
				where tempGrid.Exists(elimination) is true
				select new Conclusion(ConclusionType.Elimination, elimination)
			).ToArray(),
			{
				_startsWithWeak: false,
				RealChain: [SoleCandidateNode { Candidate: var c }, ..]
			} => new Conclusion[] { new(ConclusionType.Assignment, c) },
			_ => null
		};

	/// <summary>
	/// Indicates the <see cref="Node"/>s that ignores the first and the last node.
	/// </summary>
	public ImmutableArray<Node> RealChain
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


	/// <inheritdoc cref="ICloneable.Clone"/>
	public AlternatingInferenceChain Clone()
	{
		var resultNodes = new Node[_nodes.Length];
		for (int i = 0; i < _nodes.Length; i++)
		{
			resultNodes[i] = _nodes[i].Clone();
		}

		return new AlternatingInferenceChain(resultNodes, _startsWithWeak);
	}

	/// <inheritdoc cref="object.Equals(object?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) =>
		obj is AlternatingInferenceChain comparer && Equals(comparer);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(AlternatingInferenceChain other) =>
		_startsWithWeak == other._startsWithWeak
			&& RealChain is [var a1, .., var b1] && other.RealChain is [var a2, .., var b2]
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
	public override int GetHashCode() => HashCode.Combine(_nodes[0], _nodes[^1], _startsWithWeak);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		var sb = new StringHandler();
		var realChain = RealChain;
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
	object ICloneable.Clone() => Clone();

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
