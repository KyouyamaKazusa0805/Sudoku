using Sudoku.Collections;
using Xunit.Abstractions;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;

namespace Sudoku.Test;

internal sealed class Searcher
{
	private const string Separator = ", ";
	private readonly Dictionary<int, HashSet<int>?> _strongInferences = new();
	private readonly Dictionary<int, HashSet<int>?> _weakInferences = new();
	private readonly Dictionary<int, Node> _id2NodeLookup = new();
	private readonly Dictionary<Node, int> _node2IdLookup = new();
	private readonly List<int[]> _foundChains = new();
	private readonly ITestOutputHelper _output;

	public Searcher(ITestOutputHelper output) => _output = output;

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
		foreach (int[] foundChain in _foundChains)
		{
			string result = string.Join(
				" -> ",
				from foundChainId in foundChain select _id2NodeLookup[foundChainId].ToSimpleString()
			);

			_output.WriteLine(result);
		}
	}

	private void GetStrong(Node currentNode, in Grid grid, int candidate)
	{
		byte cell = (byte)(candidate / 9), digit = (byte)(candidate % 9);

		HashSet<int>? list = null;

		// Get bi-location regions.
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

		// Get bi-value cell.
		short candidateMask = grid.GetCandidates(cell);
		if (PopCount((uint)candidateMask) == 2)
		{
			byte theOtherDigit = (byte)Log2((uint)(candidateMask & ~(1 << digit)));
			var nextNode = new SoleCandidateNode(cell, theOtherDigit);
			int nextNodeId = _node2IdLookup[nextNode];
			(list ??= new()).Add(nextNodeId);
		}

		_strongInferences[_node2IdLookup[currentNode]] = list;
	}

	private void GetWeak(Node currentNode, in Grid grid, int candidate)
	{
		byte cell = (byte)(candidate / 9), digit = (byte)(candidate % 9);

		HashSet<int>? list = null;

		foreach (byte anotherCell in (PeerMaps[cell] & grid.CandidatesMap[digit]) - cell)
		{
			var nextNode = new SoleCandidateNode(anotherCell, digit);
			int nextNodeId = _node2IdLookup[nextNode];
			(list ??= new()).Add(nextNodeId);
		}

		foreach (byte anotherDigit in grid.GetCandidates(cell) & ~(1 << digit))
		{
			var nextNode = new SoleCandidateNode(cell, anotherDigit);
			int nextNodeId = _node2IdLookup[nextNode];
			(list ??= new()).Add(nextNodeId);
		}

		_weakInferences[_node2IdLookup[currentNode]] = list;
	}

	private void StartWithWeak()
	{
		var chain = new List<int>();
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

				nextStrong(nextId);

				chain.RemoveAt(chain.Count - 1);
			}

			chain.RemoveAt(chain.Count - 1);
		}


		void nextStrong(int id)
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

				nextWeak(nextId);

				chain.RemoveAt(chain.Count - 1);
			}
		}

		void nextWeak(int id)
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
					int[] finalArray = new int[chain.Count + 1];
					chain.CopyTo(finalArray);
					finalArray[^1] = nextId;

					_foundChains.Add(finalArray);
				}

				if (chain.Contains(nextId))
				{
					continue;
				}

				chain.Add(nextId);

				nextStrong(nextId);

				chain.RemoveAt(chain.Count - 1);
			}
		}
	}

	private void StartWithStrong()
	{
		var chain = new List<int>();
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

				nextWeak(nextId);

				chain.RemoveAt(chain.Count - 1);
			}

			chain.RemoveAt(chain.Count - 1);
		}


		void nextWeak(int id)
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

				nextStrong(nextId);

				chain.RemoveAt(chain.Count - 1);
			}
		}

		void nextStrong(int id)
		{
			if (_strongInferences[id] is not { } nextIds)
			{
				return;
			}

			foreach (int nextId in nextIds)
			{
				if (chain[0] == chain[^1])
				{
					// Found.
					int[] finalArray = new int[chain.Count + 1];
					chain.CopyTo(finalArray);
					finalArray[^1] = nextId;

					_foundChains.Add(finalArray);
				}

				if (chain.Contains(nextId))
				{
					continue;
				}

				chain.Add(nextId);

				nextWeak(nextId);

				chain.RemoveAt(chain.Count - 1);
			}
		}
	}

	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
	private void PrintStringInferences(Dictionary<int, HashSet<int>?> inferences)
	{
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
					sb.Append(Separator);
				}

				sb.RemoveFromEnd(Separator.Length);
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
