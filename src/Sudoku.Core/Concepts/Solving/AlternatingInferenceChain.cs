namespace Sudoku.Concepts.Solving;

/// <summary>
/// Defines an alternating inference chain. The data structure can describe the following techniques:
/// <list type="bullet">
/// <item>
/// Short chains:
/// <list type="bullet">
/// <item>Irregular Wings</item>
/// </list>
/// </item>
/// <item>
/// Normal chains:
/// <list type="bullet">
/// <item>Discontinuous Nice Loop</item>
/// <item>Alternating Inference Chain</item>
/// <!--<item>Continuous Nice Loop</item>-->
/// </list>
/// </item>
/// <item>
/// Grouped chains (which means the nodes are not limited in sole candidates):
/// <list type="bullet">
/// <item>Grouped Discontinuous Nice Loop</item>
/// <item>Grouped Alternating Inference Chain</item>
/// <!--<item>Grouped Continuous Nice Loop</item>-->
/// </list>
/// </item>
/// </list>
/// </summary>
public sealed class AlternatingInferenceChain : Chain
{
	/// <summary>
	/// Initializes an <see cref="AlternatingInferenceChain"/> instance via the specified nodes
	/// and the <see cref="bool"/> value indicating whether the chain starts and ends
	/// with strong inferences.
	/// </summary>
	/// <param name="nodes">The nodes used.</param>
	/// <param name="isStrong">Indicates whether the chain starts and ends with strong inferences.</param>
	/// <exception cref="ArgumentException">
	/// Throws when the length of the argument <paramref name="nodes"/> is less than or equals 3,
	/// or the first and the last node aren't the same when the arguemnt <paramref name="isStrong"/>
	/// is <see langword="true"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public AlternatingInferenceChain(Node[] nodes, bool isStrong) :
		base(nodes, InitializeFieldNodeStatus(nodes, isStrong)) =>
		IsStrong = nodes.Length < 3
			? throw new ArgumentException($"The length of the argument '{nameof(nodes)}' must be greater than 3.", nameof(nodes))
			: isStrong && nodes[0] != nodes[^1]
				? throw new ArgumentException("If the alternating inference chain starts with the strong inference, the first and the last node should be the same.")
				: isStrong;


	/// <summary>
	/// Indicates whether the alternating inference chain starts with a strong inference,
	/// and ends with another strong inference.
	/// </summary>
	public bool IsStrong { get; }

	/// <inheritdoc/>
	public override int Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => IsStrong ? base.Count : base.Count - 2;
	}

	/// <summary>
	/// <para>Indicates the nodes that represents the main chain node data for the current chain.</para>
	/// <para>
	/// The property value will be different if the property <see cref="IsStrong"/> is different.
	/// <list type="table">
	/// <listheader>
	/// <term>The value of the property <see cref="IsStrong"/></term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>All nodes will be returned (i.e. <c>_nodes[..]</c>).</description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>
	/// Returns the sliced array from the nodes being stored in the current instance,
	/// without the first and the last node (i.e. <c>_nodes[1..^1]</c>).
	/// </description>
	/// </item>
	/// </list>
	/// </para>
	/// </summary>
	/// <seealso cref="IsStrong"/>
	public ImmutableArray<Node> RealChainNodes
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ImmutableArray.Create(_nodes[IsStrong ? .. : 1..^1]);
	}


	/// <inheritdoc/>
	public override Node this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => RealChainNodes[index];
	}


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Chain? other) =>
		other is AlternatingInferenceChain comparer
			&& IsStrong == comparer.IsStrong
			&& RealChainNodes is [var a1, .., var b1] && comparer.RealChainNodes is [var a2, .., var b2]
			&& (a1 == a2 && b1 == b2 || a1 == b2 && a2 == b1);

	/// <inheritdoc/>
	public override string ToString()
	{
		var sb = new StringHandler();
		var realChain = RealChainNodes;
		for (int i = 0, length = realChain.Length - 1; i < length; i++)
		{
			sb.Append(realChain[i].ToSimpleString());
			sb.Append(((Inference)(i & 1)).GetIdentifier());
		}

		sb.RemoveFromEnd(4);
		return sb.ToStringAndClear();
	}

	/// <inheritdoc/>
	public override ImmutableArray<Conclusion> GetConclusions(in Grid grid) =>
		ImmutableArray.Create(
			IsStrong switch
			{
				true => _nodes switch
				{
					[{ Cells: var cells, Digit: var d }, ..] => cells switch
					{
						[var c] => new Conclusion[] { new(ConclusionType.Assignment, c * 9 + d) },
						_ => GetEliminationsSingleDigit(!cells & grid.CandidatesMap[d], d)
					},
					_ => null
				},
				_ => RealChainNodes switch
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
				}
			}
		);


	/// <summary>
	/// Try to initializes the field <see cref="Chain._nodesStatus"/>.
	/// </summary>
	/// <param name="nodes">The nodes.</param>
	/// <param name="startsWithStrong">
	/// Indicates whether the chain starts and ends with strong inferences.
	/// </param>
	/// <returns>The array of type <see cref="bool"/>[].</returns>
	/// <seealso cref="Chain._nodesStatus"/>
	private static bool[] InitializeFieldNodeStatus(Node[] nodes, bool startsWithStrong)
	{
		int length = nodes.Length;
		bool current = !startsWithStrong;
		bool[] result = new bool[length];
		for (int i = 0; i < length; i++)
		{
			result[i] = current;
			current ^= true; // Flip.
		}

		return result;
	}

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
}
