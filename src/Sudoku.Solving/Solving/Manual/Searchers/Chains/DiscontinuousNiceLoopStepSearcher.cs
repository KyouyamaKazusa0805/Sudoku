using System.Buffers;
using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Steps;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.Buffer.FastProperties;

namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with an <b>Discontinuous Nice Loop</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Irregular Wings</item>
/// <item>Alternating Inference Chain</item>
/// </list>
/// </summary>
[StepSearcher(Deprecated = true)]
public sealed unsafe class DiscontinuousNiceLoopStepSearcher : IDiscontinuousNiceLoopStepSearcher
{
	/// <summary>
	/// Indicates the max capacity.
	/// </summary>
	private const int MaxCapacity = 3000;


	/// <summary>
	/// Indicates the cache on storing strong inferences. The key is the ID of the chain node,
	/// and the value is the corresponding strong inference endpoints. You can get the raw information
	/// of the chain node via the field <see cref="_idNodeLookup"/>.
	/// </summary>
	private readonly Dictionary<int, ValueLinkedList<int>> _strongInferences = new();

	/// <summary>
	/// Indicates the cache on storing weak inferences. The key is the ID of the chain node,
	/// and the value is the corresponding weak inference endpoints. You can get the raw information
	/// of the chain node via the field <see cref="_idNodeLookup"/>.
	/// </summary>
	/// <seealso cref="_idNodeLookup"/>
	private readonly Dictionary<int, ValueLinkedList<int>> _weakInferences = new();

	/// <summary>
	/// Indicates the lookup table instance that can get the ID value from the specified node.
	/// </summary>
	private readonly Dictionary<ChainNode, int> _nodeIdLookup = new();

	/// <summary>
	/// Indicates the lookup table instance that can get the node instance from the specified ID value.
	/// </summary>
	private readonly Dictionary<int, ChainNode> _idNodeLookup = new();

	/// <summary>
	/// Indicates the possible chains.
	/// </summary>
	private readonly List<ChainNodeBag> _chains = new();

	/// <summary>
	/// Indicates the global index of the cached nodes.
	/// </summary>
	private int _cachedNodesGlobalIndex;

	/// <summary>
	/// Indicates the cached nodes.
	/// </summary>
	private ValueLinkedListNode<int>[] _cachedNodes = null!;


	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(13, DisplayingLevel.B);


	/// <inheritdoc/>
	/// <remarks>
	/// <para>
	/// The main idea of the algorithm is to gather strong and weak inferences before
	/// starting construction of chains.
	/// </para>
	/// <para>
	/// We use the data structure to store all possible nodes, and its strong inferences
	/// and weak inferences. All strong inferences will be formed as a linked list,
	/// and all weak inferences will be also formed as a linked list. For example:
	/// <code>
	/// Dictionary
	///     |
	///     |  / N1 -> N2              : The linked list of strong inferences of node A
	///     |-A 
	///     |  \ N3 -> N4 -> N5 -> N6  : The linked list of weak inferences of node A
	///     |
	///     |
	///     |  / N7 -> N8              : The linked list of strong inferences of node B
	///     |-B
	///     |  \ N9 -> N10 -> N11      : The linked list of weak inferences of node B
	///    ...
	/// </code>
	/// Instead of storing the complete chain node information,
	/// we use their own IDs to be stored into the dictionary,
	/// in order to generalize the relations among chain nodes,
	/// to avoid complex comparisons on chain node equality.
	/// </para>
	/// </remarks>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		_strongInferences.Clear();
		_weakInferences.Clear();
		_nodeIdLookup.Clear();
		_idNodeLookup.Clear();
		_chains.Clear();

		_cachedNodes = ArrayPool<ValueLinkedListNode<int>>.Shared.Rent(MaxCapacity);
		_cachedNodesGlobalIndex = 0;

		// Construct the look up table.
		ConstructLookup(grid);

		// Gets the strong and weak inferences.
		foreach (var (chainNode, id) in _nodeIdLookup)
		{
			ValueLinkedList<int> strongInferences = new(), weakInferences = new();

			// Then find all possible strong and weak inferences.
			GatherStrong_Single(grid, chainNode, ref strongInferences);
			GatherWeak_Single(grid, chainNode, ref weakInferences);
			// TODO: Other cases.

			// Finally, we should add it into the dictionary.
			_strongInferences.Add(id, strongInferences);
			_weakInferences.Add(id, weakInferences);
		}

		// Start iteration on construction of chains.
		// Design notes: Our goal is to find discontinuous nice loops instead of the normal AICs.
		// The discontinuous nice loops can be separated to 2 types:
		// Type 1:
		//     A
		//    / \
		//   B = C
		//
		// Type 2:
		//     A
		//   // \\
		//   B - C
		//
		// Therefore, we shoud check on both cases.
		var chain = new ValueLinkedList<int>();
		ChainingWithWeakStart(ref chain);

		// Checks whether the result collection is not empty.
		if (_chains.Count == 0)
		{
			return null;
		}

		// Iterate on each chain, and store them into the target step.
		foreach (var currentChain in _chains)
		{
			var first = currentChain[0];
			var step = new DiscontinuousNiceLoopStep(
				ImmutableArray.Create(new Conclusion(ConclusionType.Elimination, first.Cell, first.Digit)),
				ImmutableArray.Create<PresentationData>(/*new PresentationData
				{
					Candidates = GetCandidateOffsets(currentChain),
					Links = GetLinks(currentChain)
				}*/),
				currentChain
			);

			if (onlyFindOne)
			{
				return step;
			}

			accumulator.Add(step);
		}

		// Release the memory.
		ArrayPool<ValueLinkedListNode<int>>.Shared.Return(_cachedNodes);

		return null;
	}

	/// <summary>
	/// To construct the lookup table.
	/// </summary>
	/// <param name="grid">The grid used.</param>
	private void ConstructLookup(
		[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")] in Grid grid)
	{
		int globalId = 0;

		// Sole-candidate nodes.
		for (byte digit = 0; digit < 9; digit++)
		{
			foreach (byte cell in CandMaps[digit])
			{
				// Creates the basic data to store the basic information.
				var chainNode = ChainNode.FromSingleCandidate(digit, cell);
				_nodeIdLookup.Add(chainNode, globalId);
				_idNodeLookup.Add(globalId++, chainNode);
			}
		}
	}

	/// <summary>
	/// Gather strong inferences for the sole candidate.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="current">The chain node to be checked.</param>
	/// <param name="strongInferences">
	/// The linked list to store the storng inferences for the specified candidate.
	/// </param>
	private void GatherStrong_Single(in Grid grid, in ChainNode current, ref ValueLinkedList<int> strongInferences)
	{
		_ = current is { Cell: var cell, Digit: var digit };

		// Gets the bi-location regions (i.e. conjugate pairs).
		foreach (var label in Regions)
		{
			short posMask = 0;
			int region = cell.ToRegionIndex(label);
			int[] regionCells = RegionCells[region];
			for (int i = 0; i < 9; i++)
			{
				int currentRegionCell = regionCells[i];
				if (currentRegionCell != cell && grid.Exists(currentRegionCell, digit) is true)
				{
					posMask |= (short)(1 << i);
				}
			}
			if (PopCount((uint)posMask) == 2)
			{
				// Found.
				int theOtherCell = ((RegionMaps[region] & CandMaps[digit]) - cell)[0];
				int nextId = _nodeIdLookup[ChainNode.FromSingleCandidate(digit, (byte)theOtherCell)];

				ref var linkedListNode = ref _cachedNodes[_cachedNodesGlobalIndex];
				linkedListNode = new(nextId);
				strongInferences.Append((ValueLinkedListNode<int>*)Unsafe.AsPointer(ref linkedListNode));

				_cachedNodesGlobalIndex++;
			}
		}

		// Gets the bi-value cells.
		if (BivalueMap.Contains(cell))
		{
			int theOtherDigit = TrailingZeroCount(grid.GetCandidates(cell) & ~(1 << digit));
			int nextId = _nodeIdLookup[ChainNode.FromSingleCandidate((byte)theOtherDigit, cell)];

			ref var linkedListNode = ref _cachedNodes[_cachedNodesGlobalIndex];
			linkedListNode = new(nextId);
			strongInferences.Append((ValueLinkedListNode<int>*)Unsafe.AsPointer(ref linkedListNode));

			_cachedNodesGlobalIndex++;
		}
	}

	/// <summary>
	/// Gather weak inferences for the sole candidate.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="current">The chain node to be checked.</param>
	/// <param name="weakInferences">
	/// The linked list to store the weak inferences for the specified candidate.
	/// </param>
	private void GatherWeak_Single(in Grid grid, in ChainNode current, ref ValueLinkedList<int> weakInferences)
	{
		_ = current is { Cell: var cell, Digit: var digit };

		foreach (var label in Regions)
		{
			int region = cell.ToRegionIndex(label);
			foreach (int anotherCell in (RegionMaps[region] & CandMaps[digit]) - cell)
			{
				int nextId = _nodeIdLookup[ChainNode.FromSingleCandidate(digit, (byte)anotherCell)];

				ref var linkedListNode = ref _cachedNodes[_cachedNodesGlobalIndex];
				linkedListNode = new(nextId);
				weakInferences.Append((ValueLinkedListNode<int>*)Unsafe.AsPointer(ref linkedListNode));

				_cachedNodesGlobalIndex++;
			}
		}

		foreach (int anotherDigit in (short)(grid.GetCandidates(cell) & (short)~(1 << digit)))
		{
			int nextId = _nodeIdLookup[ChainNode.FromSingleCandidate((byte)anotherDigit, cell)];

			ref var linkedListNode = ref _cachedNodes[_cachedNodesGlobalIndex];
			linkedListNode = new(nextId);
			weakInferences.Append((ValueLinkedListNode<int>*)Unsafe.AsPointer(ref linkedListNode));

			_cachedNodesGlobalIndex++;
		}
	}

	/// <summary>
	/// Start the chaining, with weak inference as the next node.
	/// </summary>
	/// <param name="chain">The chain.</param>
	private void ChainingWithWeakStart(ref ValueLinkedList<int> chain)
	{
		foreach (var (id, weakIds) in _weakInferences)
		{
			// Append the start node.
			ref var linkedListNode = ref _cachedNodes[_cachedNodesGlobalIndex];
			linkedListNode = new(id);
			chain.Append((ValueLinkedListNode<int>*)Unsafe.AsPointer(ref linkedListNode));

			foreach (int weakId in weakIds)
			{
				// Append the next node.
				ref var nextLinkedListNode = ref _cachedNodes[_cachedNodesGlobalIndex];
				nextLinkedListNode = new(weakId);
				chain.Append((ValueLinkedListNode<int>*)Unsafe.AsPointer(ref nextLinkedListNode));

				// Iterate on strong inference of this node.
				ChainingWithStrong(ref chain, id);

				// Remove the node.
				_cachedNodesGlobalIndex--;
				chain.RemoveLast();
			}

			// Remove the node.
			_cachedNodesGlobalIndex--;
			chain.Remove(id, &IntegerComparer);
		}
	}

	/// <summary>
	/// Continue to construct the chain via the specified ID as the current node.
	/// </summary>
	/// <param name="chain">The collection that stores the nodes currently.</param>
	/// <param name="id">The ID as the current node.</param>
	private void ChainingWithStrong(ref ValueLinkedList<int> chain, int id)
	{
		foreach (int nextId in _strongInferences[id])
		{
			// Checks whether the ID iterated has already been stored in the chain collection.
			if (chain.Contains(nextId, &IntegerComparer))
			{
				continue;
			}

			// Append the start node.
			ref var linkedListNode = ref _cachedNodes[_cachedNodesGlobalIndex];
			linkedListNode = new(nextId);
			chain.Append((ValueLinkedListNode<int>*)Unsafe.AsPointer(ref linkedListNode));

			// Iterate on the weak inference of this node.
			ChainingWithWeak(ref chain, nextId);

			// Remove the node.
			_cachedNodesGlobalIndex--;
			chain.Remove(nextId, &IntegerComparer);
		}
	}

	/// <summary>
	/// Continue to construct the chain via the specified ID as the current node.
	/// </summary>
	/// <param name="chain">The collection that stores the nodes currently.</param>
	/// <param name="id">The ID as the current node.</param>
	private void ChainingWithWeak(ref ValueLinkedList<int> chain, int id)
	{
		foreach (int nextId in _weakInferences[id])
		{
			// Append the start node.
			ref var linkedListNode = ref _cachedNodes[_cachedNodesGlobalIndex];
			linkedListNode = new(nextId);
			chain.Append((ValueLinkedListNode<int>*)Unsafe.AsPointer(ref linkedListNode));

			// Checks the validity of the chain.
			if (chain.First.Data is var startId && startId == nextId)
			{
				var bag = new ChainNodeBag();
				foreach (int currentId in chain)
				{
					bag.Add(_idNodeLookup[currentId]);
				}

				_chains.Add(bag);
			}

			// Iterate on strong inference of this node.
			ChainingWithStrong(ref chain, nextId);

			// Remove the node.
			_cachedNodesGlobalIndex--;
			chain.Remove(nextId, &IntegerComparer);
		}
	}

	/// <summary>
	/// Get highlight candidate offsets through the specified target node.
	/// </summary>
	/// <param name="target">The target node.</param>
	/// <param name="isSimpleAic">
	/// Indicates whether the current caller is in <see cref="IAlternatingInferenceChainStepSearcher"/>.
	/// The default value is <see langword="false"/>.
	/// </param>
	/// <returns>The candidate offsets.</returns>
	/// <seealso cref="IAlternatingInferenceChainStepSearcher"/>
	private static IList<(int, ColorIdentifier)> GetCandidateOffsets(Node target, bool isSimpleAic = false)
	{
		var result = new List<(int, ColorIdentifier)>();
		var chain = target.WholeChain;
		var (pCandidate, _) = chain.Top;
		if (!isSimpleAic)
		{
			result.Add((pCandidate, (ColorIdentifier)2));
		}

		for (int i = 0, count = chain.Count; i < count; i++)
		{
			if (chain[i].Parents is { } parents)
			{
				foreach (var pr in parents)
				{
					var (prCandidate, prIsOn) = pr;
					if (isSimpleAic && i != count - 2 || !isSimpleAic)
					{
						var pair = (prCandidate, (ColorIdentifier)(prIsOn ? 1 : 0));
						if (!result.Contains(pair))
						{
							result.Add(pair);
						}
					}
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Get the links through the specified target node.
	/// </summary>
	/// <param name="target">The target node.</param>
	/// <param name="showAllLinks">
	/// Indicates whether the current chain will display all chains (even contains the weak links
	/// from the elimination node). The default value is <see langword="false"/>. If you want to
	/// draw the AIC, the elimination weak links don't need drawing.
	/// </param>
	/// <returns>The link.</returns>
	private static IList<(Link, ColorIdentifier)> GetLinks(Node target, bool showAllLinks = false)
	{
		var result = new List<(Link, ColorIdentifier)>();
		var chain = target.WholeChain;
		for (int i = showAllLinks ? 0 : 1, count = chain.Count - (showAllLinks ? 0 : 2); i < count; i++)
		{
			var p = chain[i];
			var (pCandidate, pIsOn) = p;
			var parents = p.Parents;
			for (int j = 0, parentsCount = parents?.Length ?? 0; j < parentsCount; j++)
			{
				var (prCandidate, prIsOn) = parents![j];
				result.Add(
					(
						new(pCandidate, prCandidate, (A: prIsOn, B: pIsOn) switch
						{
							(A: false, B: true) => LinkKind.Strong,
							(A: true, B: false) => LinkKind.Weak,
							_ => LinkKind.Default
						}),
						(ColorIdentifier)0
					)
				);
			}
		}

		return result;
	}

	/// <summary>
	/// Compares two integers.
	/// </summary>
	/// <param name="a">The first integer to compare.</param>
	/// <param name="b">The second integer to compare.</param>
	/// <returns>The <see cref="bool"/> result.</returns>
	private static bool IntegerComparer(int a, int b) => a == b;
}
