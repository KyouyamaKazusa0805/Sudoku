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
	private readonly ITestOutputHelper _output;
	private int _globalId = 0;

	public Searcher(ITestOutputHelper output) => _output = output;

	public void GetAll(in Grid grid)
	{
		_strongInferences.Clear();
		_weakInferences.Clear();
		_id2NodeLookup.Clear();
		_node2IdLookup.Clear();

		foreach (int candidate in grid)
		{
			var node = new SoleCandidateNode((byte)(candidate / 9), (byte)(candidate % 9));
			_id2NodeLookup.Add(_globalId, node);
			_node2IdLookup.Add(node, _globalId);
			_strongInferences.Add(_globalId, null);
			_weakInferences.Add(_globalId, null);

			_globalId++;
		}

		foreach (int candidate in grid)
		{
			var node = new SoleCandidateNode((byte)(candidate / 9), (byte)(candidate % 9));
			GetStrong(node, grid, candidate);
			GetWeak(node, grid, candidate);
		}

		// Tests.
		PrintStringInferences(_strongInferences);
		PrintStringInferences(_weakInferences);
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
