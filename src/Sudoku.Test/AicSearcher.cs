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
	public SearcherNodeTypes NodeTypes { get; set; } = SearcherNodeTypes.SoleDigit | SearcherNodeTypes.SoleCell;


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

		// Gather the basic information on basic nodes.
		int id = 0;
		foreach (int candidate in grid)
		{
			var node = new SoleCandidateNode((byte)(candidate / 9), (byte)(candidate % 9));
			_id2NodeLookup.Add(id, node);
			_node2IdLookup.Add(node, id);
			_strongInferences.Add(id, null);
			_weakInferences.Add(id, null);

			id++;
		}

		// Then checks and searches for the strong and weak inferences by each candidate.
		foreach (int candidate in grid)
		{
			var node = new SoleCandidateNode((byte)(candidate / 9), (byte)(candidate % 9));
			GetStrong(node, grid, candidate);
			GetWeak(node, grid, candidate);
		}

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
	}

	/// <summary>
	/// <para>
	/// Try to get all possible strong inferences for a specified <see cref="Node"/> instance,
	/// and record them to store into the field <see cref="_strongInferences"/>.
	/// </para>
	/// <para>
	/// In addition, the result value is a dictionary that stores a key-value pair,
	/// where the key is an <see cref="int"/> corresponding to the current node, and the value
	/// is an unduplicated collection of IDs corresponding to their own node.
	/// </para>
	/// <para>
	/// All elements in the value of the key-value pair are the possible nodes that can form
	/// the strong inferences with that <see cref="Node"/> instance.
	/// </para>
	/// </summary>
	/// <param name="currentNode">
	/// The node, which is the target node that can form a strong inference with arbitary one
	/// node in the unduplicated collection mentioned above.
	/// </param>
	/// <param name="grid">
	/// The grid that is used for checking whether the two nodes form a strong inference.
	/// </param>
	/// <param name="candidate">The candidate of the node used.</param>
	/// <seealso cref="_strongInferences"/>
	private void GetStrong(Node currentNode, in Grid grid, int candidate)
	{
		byte cell = (byte)(candidate / 9), digit = (byte)(candidate % 9);

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
					int nextNodeId = _node2IdLookup[nextNode];
					(list ??= new()).Add(nextNodeId);
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
				int nextNodeId = _node2IdLookup[nextNode];
				(list ??= new()).Add(nextNodeId);
			}
		}



		_strongInferences[_node2IdLookup[currentNode]] = list;
	}

	/// <summary>
	/// <para>
	/// Try to get all possible weak inferences for a specified <see cref="Node"/> instance,
	/// and record them to store into the field <see cref="_weakInferences"/>.
	/// </para>
	/// <para>
	/// In addition, the result value is a dictionary that stores a key-value pair,
	/// where the key is an <see cref="int"/> corresponding to the current node, and the value
	/// is an unduplicated collection of IDs corresponding to their own node.
	/// </para>
	/// <para>
	/// All elements in the value of the key-value pair are the possible nodes that can form
	/// the weak inferences with that <see cref="Node"/> instance.
	/// </para>
	/// </summary>
	/// <param name="currentNode">
	/// The node, which is the target node that can form a weak inference with arbitary one
	/// node in the unduplicated collection mentioned above.
	/// </param>
	/// <param name="grid">
	/// The grid that is used for checking whether the two nodes form a weak inference.
	/// </param>
	/// <param name="candidate">The candidate of the node used.</param>
	/// <seealso cref="_weakInferences"/>
	private void GetWeak(Node currentNode, in Grid grid, int candidate)
	{
		byte cell = (byte)(candidate / 9), digit = (byte)(candidate % 9);

		HashSet<int>? list = null;

		if (NodeTypes.Flags(SearcherNodeTypes.SoleDigit))
		{
			foreach (byte anotherCell in (PeerMaps[cell] & grid.CandidatesMap[digit]) - cell)
			{
				var nextNode = new SoleCandidateNode(anotherCell, digit);
				int nextNodeId = _node2IdLookup[nextNode];
				(list ??= new()).Add(nextNodeId);
			}
		}

		if (NodeTypes.Flags(SearcherNodeTypes.SoleCell))
		{
			foreach (byte anotherDigit in grid.GetCandidates(cell) & ~(1 << digit))
			{
				var nextNode = new SoleCandidateNode(cell, anotherDigit);
				int nextNodeId = _node2IdLookup[nextNode];
				(list ??= new()).Add(nextNodeId);
			}
		}

		_weakInferences[_node2IdLookup[currentNode]] = list;
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
			if (_strongInferences[id] is not { } nextIds)
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
			if (_weakInferences[id] is not { } nextIds)
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
			if (_weakInferences[id] is not { } nextIds)
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
			if (_strongInferences[id] is not { } nextIds)
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
	/// Prints the strong and weak inferences as the string representation onto the test explorer.
	/// </summary>
	/// <param name="inferences">
	/// The inferences to be output. The value can be either <see cref="_strongInferences"/>
	/// or <see cref="_weakInferences"/>.
	/// </param>
	/// <seealso cref="_strongInferences"/>
	/// <seealso cref="_weakInferences"/>
	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
	[Conditional("DEBUG")]
	private void PrintStringInferences(Dictionary<int, HashSet<int>?> inferences)
	{
		const string separator = ", ";

		var sb = new StringHandler();
		foreach (var (id, nextIds) in inferences)
		{
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
}
