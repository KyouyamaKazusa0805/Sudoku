#define OUTPUT_INFERENCES
#pragma warning disable IDE0051, IDE0079

using Sudoku.Collections;
using Xunit.Abstractions;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;

namespace Sudoku.Test;

internal sealed class AicSearcher
{
	/// <summary>
	/// Indicates the field that stores the temporary strong inferences during the searching.
	/// </summary>
	/// <remarks>
	/// The value uses a <see cref="Dictionary{TKey, TValue}"/> to store the table of strong inferences, where:
	/// <list type="table">
	/// <listheader>
	/// <term>Item</term>
	/// <description>Meaning</description>
	/// </listheader>
	/// <item>
	/// <term>Key</term>
	/// <description>The ID of a node.</description>
	/// </item>
	/// <item>
	/// <term>Value</term>
	/// <description>
	/// All possible IDs that corresponds to their own node respectively,
	/// one of which can form a strong inference with the <b>Key</b> node.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	/// <seealso cref="Dictionary{TKey, TValue}"/>
	private readonly Dictionary<int, HashSet<int>?> _strongInferences = new();

	/// <summary>
	/// Indicates the field that stores the temporary weak inferences during the searching.
	/// </summary>
	/// <remarks>
	/// The value uses a <see cref="Dictionary{TKey, TValue}"/> to store the table of weak inferences, where:
	/// <list type="table">
	/// <listheader>
	/// <term>Item</term>
	/// <description>Meaning</description>
	/// </listheader>
	/// <item>
	/// <term>Key</term>
	/// <description>The ID of a node.</description>
	/// </item>
	/// <item>
	/// <term>Value</term>
	/// <description>
	/// All possible IDs that corresponds to their own node respectively,
	/// one of which can form a weak inference with the <b>Key</b> node.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	/// <seealso cref="Dictionary{TKey, TValue}"/>
	private readonly Dictionary<int, HashSet<int>?> _weakInferences = new();

	/// <summary>
	/// Indicates the lookup table that can get the target <see cref="Node"/> instance
	/// via the corresponding ID value.
	/// </summary>
	/// <seealso cref="Node"/>
	private readonly Dictionary<int, Node> _id2NodeLookup = new();

	/// <summary>
	/// Indicates the lookup table that can get the target ID value
	/// via the corresponding <see cref="Node"/> instance.
	/// </summary>
	/// <seealso cref="Node"/>
	private readonly Dictionary<Node, int> _node2IdLookup = new();

	/// <summary>
	/// Indicates all possible found chains, that stores the IDs of the each node.
	/// </summary>
	private readonly List<(int[] ChainIds, bool StartsWithWeakInference)> _foundChains = new();

	/// <summary>
	/// Indicates the output instance that can allow displaying the customized items onto the test explorer.
	/// </summary>
	private readonly ITestOutputHelper _output;

	/// <summary>
	/// Indicates the global ID value.
	/// </summary>
	private int _globalId;


	/// <summary>
	/// Initializes a <see cref="AicSearcher"/> instance via the <see cref="ITestOutputHelper"/> instance
	/// to allow displaying the customized items onto the test explorer.
	/// </summary>
	/// <param name="output">
	/// The <see cref="ITestOutputHelper"/> instance
	/// that allows displaying the customized items onto the test explorer.
	/// </param>
	/// <seealso cref="ITestOutputHelper"/>
	public AicSearcher(ITestOutputHelper output) => _output = output;


	/// <summary>
	/// Indicates the extended nodes to be searched for. Please note that the type of the property
	/// is an enumeration type with bit-fields attribute, which means you can add multiple choices
	/// into the value.
	/// </summary>
	public SearcherNodeTypes NodeTypes { get; set; } =
		SearcherNodeTypes.SoleDigit | SearcherNodeTypes.SoleCell
			| SearcherNodeTypes.LockedCandidates;


	/// <summary>
	/// Get all possible chain steps.
	/// </summary>
	/// <param name="grid">The grid used.</param>
	public void GetAll(in Grid grid)
	{
		// Clear all possible lists.
		_strongInferences.Clear();
		_weakInferences.Clear();
		_id2NodeLookup.Clear();
		_node2IdLookup.Clear();
		_foundChains.Clear();

		// Gather strong and weak links.
		GatherStrongAndWeak_Sole(grid);
		GatherStrongAndWeak_LockedCandidates(grid);

#if OUTPUT_INFERENCES
		// Display the inferences found.
		printInferences(_strongInferences);
		printInferences(_weakInferences);


		void printInferences(Dictionary<int, HashSet<int>?> inferences)
		{
			const string separator = ", ";

			var sb = new StringHandler();
			foreach (var (id, nextIds) in inferences)
			{
				if (!_id2NodeLookup.ContainsKey(id))
				{
					continue;
				}

				sb.Append("Node ");
				sb.Append(_id2NodeLookup[id].ToSimpleString());
				sb.Append(": ");

				if (nextIds is not null)
				{
					foreach (int nextId in nextIds)
					{
						sb.Append(_id2NodeLookup[nextId].ToSimpleString());
						sb.Append(separator);
					}

					sb.RemoveFromEnd(separator.Length);
				}
				else
				{
					sb.Append("<null>");
				}

				sb.AppendLine();
			}

			_output.WriteLine(sb.ToStringAndClear());
		}
#else
		// Construct chains.
		StartWithWeak();
		StartWithStrong();

		// Output the result.
		var tempList = new Dictionary<AlternatingInferenceChain, Conclusion[]>();
		foreach (var (nids, startsWithWeak) in _foundChains)
		{
			var chain = new AlternatingInferenceChain(from nid in nids select _id2NodeLookup[nid], startsWithWeak);
			if (chain.GetConclusions(grid) is { Length: not 0 } conclusions && !tempList.ContainsKey(chain))
			{
				tempList.Add(chain, conclusions);
			}
		}

		foreach (var (chain, conclusions) in tempList)
		{
			_output.WriteLine($"{chain} => {new ConclusionCollection(conclusions).ToString()}");
		}
#endif
	}

	/// <summary>
	/// Gather the strong and weak inferences on sole candidate nodes.
	/// </summary>
	/// <param name="grid">The grid.</param>
	private void GatherStrongAndWeak_Sole(in Grid grid)
	{
		// Sole candidate -> Sole candidate.
		foreach (int candidate in grid)
		{
			byte cell = (byte)(candidate / 9), digit = (byte)(candidate % 9);
			var node = new SoleCandidateNode((byte)(candidate / 9), (byte)(candidate % 9));
			getStrong(grid);
			getWeak(grid);


			void getStrong(in Grid grid)
			{
				HashSet<int>? list = null;

				// Get bi-location regions.
				if (NodeTypes.Flags(SearcherNodeTypes.SoleDigit))
				{
					foreach (var label in Regions)
					{
						int region = cell.ToRegionIndex(label);
						var posCells = (RegionMaps[region] & grid.CandidatesMap[digit]) - cell;
						if (posCells is [var posCell])
						{
							var nextNode = new SoleCandidateNode((byte)posCell, digit);
							AddNode(nextNode, ref list);
						}
					}
				}

				// Get bi-value cell.
				if (NodeTypes.Flags(SearcherNodeTypes.SoleCell))
				{
					short candidateMask = grid.GetCandidates(cell);
					if (PopCount((uint)candidateMask) == 2)
					{
						byte theOtherDigit = (byte)Log2((uint)(candidateMask & ~(1 << digit)));
						var nextNode = new SoleCandidateNode(cell, theOtherDigit);
						AddNode(nextNode, ref list);
					}
				}

				AssignOrUpdateHashSet(list, node, _strongInferences);
			}

			void getWeak(in Grid grid)
			{
				HashSet<int>? list = null;

				if (NodeTypes.Flags(SearcherNodeTypes.SoleDigit))
				{
					foreach (byte anotherCell in (PeerMaps[cell] & grid.CandidatesMap[digit]) - cell)
					{
						var nextNode = new SoleCandidateNode(anotherCell, digit);
						AddNode(nextNode, ref list);
					}
				}

				if (NodeTypes.Flags(SearcherNodeTypes.SoleCell))
				{
					foreach (byte anotherDigit in grid.GetCandidates(cell) & ~(1 << digit))
					{
						var nextNode = new SoleCandidateNode(cell, anotherDigit);
						AddNode(nextNode, ref list);
					}
				}

				AssignOrUpdateHashSet(list, node, _weakInferences);
			}
		}
	}

	/// <summary>
	/// Gather the strong and weak inferences on locked candidates nodes.
	/// </summary>
	/// <param name="grid">The grid.</param>
	private void GatherStrongAndWeak_LockedCandidates(in Grid grid)
	{
		// Sole candidate -> Locked candidates.
		foreach (int candidate in grid)
		{
			byte cell = (byte)(candidate / 9), digit = (byte)(candidate % 9);
			var node = new SoleCandidateNode(cell, digit);
			getStrong(grid);
			getWeak(grid);


			void getStrong(in Grid grid)
			{
				HashSet<int>? list = null;

				if (NodeTypes.Flags(SearcherNodeTypes.LockedCandidates))
				{
					foreach (var label in Regions)
					{
						int region = cell.ToRegionIndex(label);
						var otherCells = RegionMaps[region] & grid.CandidatesMap[digit] - cell;
						if (
							otherCells is not
							{
								Count: > 1 and <= 3,
								CoveredLine: not Constants.InvalidFirstSet,
								CoveredRegions: var coveredRegions
							}
						)
						{
							// Optimization:
							// 1) If the number of all other cells in the current region
							// is greater than 3, the region doesn't hold a valid strong inference
							// from the current candidate to a locked candidates,
							// because a locked candidates node at most use 3 cells.
							// 2) If all other cells don't lie in a same row or column, those cells
							// can still not form a locked candidates node.
							continue;
						}

						if (TrailingZeroCount(coveredRegions) >= 9)
						{
							// The cells must be in a same block.
							continue;
						}

						var nextNode = new LockedCandidatesNode(digit, otherCells);
						AddNode(nextNode, ref list);
					}
				}

				AssignOrUpdateHashSet(list, node, _strongInferences);
			}

			void getWeak(in Grid grid)
			{
				HashSet<int>? list = null;
				using var possibleList = new ValueList<Cells>(4);
				var triplets = (stackalloc Cells[3]);

				if (NodeTypes.Flags(SearcherNodeTypes.LockedCandidates))
				{
					foreach (var label in Regions)
					{
						int region = cell.ToRegionIndex(label);
						var otherCells = RegionMaps[region] & grid.CandidatesMap[digit] - cell;
						if (otherCells.Count <= 1)
						{
							continue;
						}

						// Okay. Now we get a set of cells.
						// Now we should gather all possible covered rows and columns.
						if (label == Region.Block)
						{
							int block = cell.ToRegionIndex(Region.Block);
							foreach (int subRegions in otherCells.RowMask << 9 | otherCells.ColumnMask << 18)
							{
								var intersectionMap = RegionMaps[block] & RegionMaps[subRegions];
								var subOtherCells = grid.CandidatesMap[digit] & intersectionMap - cell;
								if (subOtherCells.Count is not (var count and not (0 or 1)))
								{
									continue;
								}

								possibleList.Clear();
								if (count == 2)
								{
									possibleList.Add(subOtherCells);
								}
								else// if (count == 3)
								{
									var combinations = subOtherCells & 2;
									possibleList.Add(combinations[0]);
									possibleList.Add(combinations[1]);
									possibleList.Add(combinations[2]);
									possibleList.Add(subOtherCells);
								}

								foreach (var cellsCombination in possibleList)
								{
									var nextNode = new LockedCandidatesNode(digit, cellsCombination);
									AddNode(nextNode, ref list);
								}
							}
						}
						else
						{
							triplets.Clear();
							triplets[0] = RegionMaps[region][0..3];
							triplets[1] = RegionMaps[region][3..6];
							triplets[2] = RegionMaps[region][6..9];

							foreach (ref readonly var triplet in triplets)
							{
								var subOtherCells = triplet & otherCells;
								if (subOtherCells.Count is not (var count and not (0 or 1)))
								{
									continue;
								}

								possibleList.Clear();
								if (count == 2)
								{
									possibleList.Add(subOtherCells);
								}
								else// if (count == 3)
								{
									var combinations = subOtherCells & 2;
									possibleList.Add(combinations[0]);
									possibleList.Add(combinations[1]);
									possibleList.Add(combinations[2]);
									possibleList.Add(subOtherCells);
								}

								foreach (var cellsCombination in possibleList)
								{
									var nextNode = new LockedCandidatesNode(digit, cellsCombination);
									AddNode(nextNode, ref list);
								}
							}
						}
					}
				}

				AssignOrUpdateHashSet(list, node, _weakInferences);
			}
		}

		// Locked candidates -> Sole candidate.


		// Locked candidates -> Locked candidates.
	}

	/// <summary>
	/// Start to construct the chain, with the weak inference as the beginning node.
	/// </summary>
	private void StartWithWeak()
	{
		var chain = new Bag<int>();
		foreach (var (id, nextIds) in _weakInferences)
		{
			if (nextIds is null)
			{
				continue;
			}

			chain.Add(id);

			foreach (int nextId in nextIds)
			{
				chain.Add(nextId);

				nextStrong(ref chain, nextId);

				chain.Remove();
			}

			chain.Remove();
		}

		chain.Dispose();


		void nextStrong(ref Bag<int> chain, int id)
		{
			if (!_strongInferences.TryGetValue(id, out var nextIds) || nextIds is null)
			{
				return;
			}

			foreach (int nextId in nextIds)
			{
				if (chain.Contains(nextId))
				{
					continue;
				}

				chain.Add(nextId);

				nextWeak(ref chain, nextId);

				chain.Remove();
			}
		}

		void nextWeak(ref Bag<int> chain, int id)
		{
			if (!_weakInferences.TryGetValue(id, out var nextIds) || nextIds is null)
			{
				return;
			}

			foreach (int nextId in nextIds)
			{
				if (chain[0] == nextId)
				{
					// Found.
					int[] finalArray = chain.ImmutelyAdd(nextId).ToArray();

					_foundChains.Add((finalArray, true));

					return;
				}

				if (chain.Contains(nextId))
				{
					continue;
				}

				chain.Add(nextId);

				nextStrong(ref chain, nextId);

				chain.Remove();
			}
		}
	}

	/// <summary>
	/// Start to construct the chain, with the strong inference as the beginning node.
	/// </summary>
	private void StartWithStrong()
	{
		var chain = new Bag<int>();
		foreach (var (id, nextIds) in _strongInferences)
		{
			if (nextIds is null)
			{
				continue;
			}

			chain.Add(id);

			foreach (int nextId in nextIds)
			{
				chain.Add(nextId);

				nextWeak(ref chain, nextId);

				chain.Remove();
			}

			chain.Remove();
		}

		chain.Dispose();


		void nextWeak(ref Bag<int> chain, int id)
		{
			if (!_weakInferences.TryGetValue(id, out var nextIds) || nextIds is null)
			{
				return;
			}

			foreach (int nextId in nextIds)
			{
				if (chain.Contains(nextId))
				{
					continue;
				}

				chain.Add(nextId);

				nextStrong(ref chain, nextId);

				chain.Remove();
			}
		}

		void nextStrong(ref Bag<int> chain, int id)
		{
			if (!_strongInferences.TryGetValue(id, out var nextIds) || nextIds is null)
			{
				return;
			}

			foreach (int nextId in nextIds)
			{
				if (chain[0] == nextId)
				{
					// Found.
					int[] finalArray = chain.ImmutelyAdd(nextId).ToArray();

					_foundChains.Add((finalArray, false));

					return;
				}

				if (chain.Contains(nextId))
				{
					continue;
				}

				chain.Add(nextId);

				nextWeak(ref chain, nextId);

				chain.Remove();
			}
		}
	}

	/// <summary>
	/// Adds the specified node into the collection, or just get the ID value.
	/// </summary>
	/// <param name="nextNode">The next node.</param>
	/// <param name="list">The list.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void AddNode(Node nextNode, [NotNull] ref HashSet<int>? list)
	{
		if (_node2IdLookup.TryGetValue(nextNode, out int nextNodeId))
		{
			(list ??= new()).Add(nextNodeId);
		}
		else
		{
			_id2NodeLookup.Add(_globalId, nextNode);
			_node2IdLookup.Add(nextNode, _globalId);
			(list ??= new()).Add(_globalId++);
		}
	}

	/// <summary>
	/// To assign the recorded IDs into the dictionary.
	/// </summary>
	/// <param name="list">The list of IDs.</param>
	/// <param name="currentNode">The current node.</param>
	/// <param name="inferences">Indicates what dictionary the hash set is assigned to.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void AssignOrUpdateHashSet(
		HashSet<int>? list, Node currentNode, Dictionary<int, HashSet<int>?> inferences)
	{
		if (list is not null)
		{
			if (_node2IdLookup.TryGetValue(currentNode, out int currentNodeId))
			{
				if (list is not null)
				{
					if (inferences.ContainsKey(currentNodeId))
					{
						inferences[currentNodeId]!.AddRange(list);
					}
					else
					{
						inferences.Add(currentNodeId, list);
					}
				}
			}
			else
			{
				inferences.Add(_globalId++, list);
			}
		}
	}
}
