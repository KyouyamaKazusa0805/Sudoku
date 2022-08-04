namespace Sudoku.Solving.Patterns;

/// <summary>
/// Defines an alternating inference chain. The data structure can describe the following techniques:
/// <list type="bullet">
/// <item>
/// Normal chains:
/// <list type="bullet">
/// <item>
/// Irregular Wings
/// <list type="bullet">
/// <item>W-Wing</item>
/// <item>M-Wing</item>
/// <item>Split Wing</item>
/// <item>Local Wing</item>
/// <item>Hybrid Wing</item>
/// <item>Purple Cow</item>
/// </list>
/// </item>
/// <item>Discontinuous Nice Loop</item>
/// <item>Alternating Inference Chain</item>
/// <item>Continuous Nice Loop</item>
/// </list>
/// </item>
/// <item>
/// Grouped chains (which means the nodes are not limited in sole candidates):
/// <list type="bullet">
/// <item>Grouped Wings</item>
/// <item>Grouped Discontinuous Nice Loop</item>
/// <item>Grouped Alternating Inference Chain</item>
/// <item>Grouped Continuous Nice Loop</item>
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
	/// or the first and the last node aren't the same when the argument <paramref name="isStrong"/>
	/// is <see langword="true"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public AlternatingInferenceChain(Node[] nodes, bool isStrong) : base(nodes, InitializeFieldNodeStatus(nodes, isStrong))
	{
		const string lengthNotEnough = $"The length of the argument '{nameof(nodes)}' must be greater than 3.";
		const string nodeNotSameStartsWithStrong = "If the alternating inference chain starts with the strong inference, the first and the last node should be the same.";

		IsStrong = nodes switch
		{
			{ Length: < 3 } => @throw(lengthNotEnough),
			[var a, .., var b] when isStrong && a != b => @throw(nodeNotSameStartsWithStrong),
			_ => isStrong
		};


		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool @throw(string s) => throw new ArgumentException(s, nameof(nodes));
	}


	/// <summary>
	/// Indicates whether the alternating inference chain starts with a strong inference,
	/// and ends with another strong inference.
	/// </summary>
	public bool IsStrong { get; }

#pragma warning disable IDE0055
	/// <summary>
	/// Indicates whether the chain has a collision, encountered between start and end node.
	/// This property should be satisfied at least the chain is a grouped chain, and at least one of
	/// the first and the last node is a grouped node.
	/// </summary>
	public bool HasNodeCollision
		=> this is
		{
			IsStrong: false,
			RealChainNodes:
			[
				{ Cells: var aCells, Digit: var aDigit },
				..,
				{ Cells: var bCells, Digit: var bDigit }
			]
		} && aDigit == bDigit && (aCells | bCells).InOneHouse && (aCells & bCells) is not [];

	/// <summary>
	/// Indicates whether the current chain forms a continuous nice loop.
	/// </summary>
	public bool IsContinuousNiceLoop
		=> this switch
		{
			{
				IsStrong: false,
				RealChainNodes:
				[
					{ Cells: var aCells, Digit: var aDigit },
					..,
					{ Cells: var bCells, Digit: var bDigit }
				]
			} => (aCells, bCells) switch
			{
				([var aCell], [var bCell])
					=> aCell == bCell && aDigit != bDigit || aDigit == bDigit && (aCells | bCells).InOneHouse,
				_ => aDigit == bDigit && (aCells | bCells).InOneHouse && (aCells & bCells) is []
			},
			// The continuous nice loop must starts with weak links due to the design of the current data structure.
			_ => false
		};
#pragma warning restore IDE0055

	/// <summary>
	/// Indicates whether the current chain is a grouped chain.
	/// </summary>
	public bool IsGrouped
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _nodes.Any(static node => node.IsGrouped);
	}

	/// <summary>
	/// Determines whether the current chain is an irregular wing (Chinese: Duan3 Lian4).
	/// </summary>
	public bool IsIrregularWing
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => !IsStrong && RealChainNodes.Length == 5;
	}

	/// <summary>
	/// Determines whether the chain only uses ALS grouped node as the strong inferences.
	/// </summary>
	public bool IsAlmostLockedSetsOnly
	{
		get
		{
#if false
			if (IsStrong)
			{
				return false;
			}

			var nodes = RealChainNodes;
			for (int i = 0; i < nodes.Length; i += 2)
			{
				if (nodes[i].Type != NodeType.AlmostLockedSets)
				{
					return false;
				}
			}

			return true;
#else
			return false;
#endif
		}
	}

	/// <summary>
	/// Determines whether the chain only uses AHS grouped node as the weak inferences.
	/// </summary>
	public bool IsAlmostHiddenSetsOnly
	{
		get
		{
#if false
			if (IsStrong)
			{
				return false;
			}

			var nodes = RealChainNodes;
			for (int i = 1; i < nodes.Length; i += 2)
			{
				if (nodes[i].Type != NodeType.AlmostHiddenSets)
				{
					return false;
				}
			}

			return true;
#else
			return false;
#endif
		}
	}

	/// <inheritdoc/>
	public override int Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => IsStrong ? base.Count : base.Count - 2;
	}

	/// <summary>
	/// Indicates the full chain nodes, containing elimination nodes.
	/// </summary>
	public ImmutableArray<Node> FullChainNodes
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ImmutableArray.Create(_nodes);
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
		get => ImmutableArray.Create(IsStrong ? _nodes : _nodes[1..^1]);
	}


	/// <inheritdoc/>
	public override Node this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => RealChainNodes[index];
	}


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Chain? other)
		=> other is AlternatingInferenceChain comparer
		&& IsStrong == comparer.IsStrong
		&& RealChainNodes is [var a1, .., var b1] && comparer.RealChainNodes is [var a2, .., var b2]
		&& (a1 == a2 && b1 == b2 || a1 == b2 && a2 == b1);

	/// <inheritdoc/>
	public override string ToString()
	{
		scoped var sb = new StringHandler();
		var realChain = RealChainNodes;
		for (int i = 0, length = realChain.Length; i < length; i++)
		{
			sb.Append(realChain[i].ToSimpleString());
			sb.Append(((Inference)(i & 1)).GetIdentifier());
		}

		sb.RemoveFromEnd(4);
		return sb.ToStringAndClear();
	}

	/// <inheritdoc/>
	public override ImmutableArray<Conclusion> GetConclusions(scoped in Grid grid)
		=> ImmutableArray.Create(
			IsStrong switch
			{
				// The chain starts and ends with a same node.
				true => _nodes switch
				{
					// Valid deconstruction.
					[{ Cells: var cells, Digit: var d }, ..] => cells switch
					{
						// The node only uses a single cell.
						[var c] => new Conclusion[] { new(ConclusionType.Assignment, c * 9 + d) },

						// The node uses more than one cell.
						_ => GetEliminationsSingleDigit(!cells & grid.CandidatesMap[d], d)
					},

					// Invalid case.
					_ => null
				},

				// The chain starts and ends with weak inferences.
				_ => IsContinuousNiceLoop switch
				{
					// The chain has formed a continuous nice loop.
					true => GetEliminationsLoop(grid, RealChainNodes),

					// The chain is a normal chain.
					_ => RealChainNodes switch
					{
						// Valid deconstruction.
						[{ Cells: var c1, Digit: var d1 }, .., { Cells: var c2, Digit: var d2 }] => (d1 == d2) switch
						{
							// The chain starts and ends with two different nodes but a same digit.
							true when (!c1 & !c2 & grid.CandidatesMap[d1]) is var elimMap and not []
								=> GetEliminationsSingleDigit(elimMap, d1),

							// The chain starts and ends with two different nodes and different digits.
							_ => (c1, c2) switch
							{
								// The start-point and end-point are both a single cell.
								([var oc1], [var oc2]) => GetEliminationsMultipleDigits(grid, oc1, oc2, d1, d2),

								// The end-point uses multiple cells.
								([var oc], { Count: > 1 }) when grid.Exists(oc, d2) is true
									=> new Conclusion[] { new(ConclusionType.Elimination, oc, d2) },

								// The start-point uses multiple cells.
								({ Count: > 1 }, [var oc]) when grid.Exists(oc, d1) is true
									=> new Conclusion[] { new(ConclusionType.Elimination, oc, d1) },

								// Both start-point and end-point nodes use multiple cells, no conclusions will be found.
								_ => null
							}
						},

						// Invalid case.
						_ => null
					}
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
			current = !current;
		}

		return result;
	}

	/// <summary>
	/// Try to get eliminations, treating the current AIC as a continuous nice loop.
	/// </summary>
	/// <param name="grid">The grid as the candidate reference.</param>
	/// <param name="nodes">The base nodes.</param>
	/// <returns>The conclusions.</returns>
	private static unsafe Conclusion[] GetEliminationsLoop(scoped in Grid grid, ImmutableArray<Node> nodes)
	{
		using scoped var result = new ValueList<Conclusion>(50);
		for (int i = 1; i < nodes.Length; i += 2)
		{
			var node = nodes[i];
			var nextNode = nodes[i + 1 is var nextIndex && nextIndex < nodes.Length ? nextIndex : 0];
			switch (node, nextNode)
			{
				case ({ Cells: [var aCell], Digit: var aDigit }, { Cells: [var bCell], Digit: var bDigit })
				when aCell == bCell:
				{
					// Same cell.
					foreach (int digit in (short)(grid.GetCandidates(aCell) & ~(1 << aDigit | 1 << bDigit)))
					{
						result.AddIfNotContain(new(ConclusionType.Elimination, aCell, digit), &cmp);
					}

					break;
				}
				case ({ Cells: var aCells, Digit: var aDigit }, { Cells: var bCells, Digit: var bDigit })
				when aDigit == bDigit:
				{
					if ((aCells | bCells).CoveredHouses is var houses and not 0)
					{
						// Same house, same digit.
						foreach (int house in houses)
						{
							foreach (int cell in (grid.CandidatesMap[aDigit] & HouseMaps[house]) - (aCells | bCells))
							{
								result.AddIfNotContain(new(ConclusionType.Elimination, cell, aDigit), &cmp);
							}
						}
					}
					else if ((!aCells & !bCells & grid.CandidatesMap[aDigit]) is var elimMap and not [])
					{
						// Same digit but different houses.
						foreach (int cell in elimMap)
						{
							result.AddIfNotContain(new(ConclusionType.Elimination, cell, aDigit), &cmp);
						}
					}

					break;
				}
			}

			// Gets advanced conclusions.
			foreach (var advancedConclusion in node.PotentialConclusionsWith(grid, nextNode))
			{
				result.AddIfNotContain(advancedConclusion, &cmp);
			}
		}

		return result.ToArray();


		static bool cmp(Conclusion a, Conclusion b) => a == b;
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
	private static Conclusion[] GetEliminationsMultipleDigits(scoped in Grid grid, int c1, int c2, byte d1, byte d2)
	{
		using scoped var resultList = new ValueList<Conclusion>(2);
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
	private static Conclusion[] GetEliminationsSingleDigit(scoped in Cells elimMap, byte digit)
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
