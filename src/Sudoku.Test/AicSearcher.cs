#undef OUTPUT_INFERENCES
#define GET_ELIMINATIONS

using Sudoku.Collections;
using Xunit.Abstractions;

namespace Sudoku.Test;

internal sealed partial class AicSearcher
{
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
	/// Indicates the maximum length of the chain searched for. If the chain length is greater than
	/// the value, the result will be ignored.
	/// </summary>
	public int MaximumLength { get; set; } = 10;

	/// <summary>
	/// Indicates the extended nodes to be searched for. Please note that the type of the property
	/// is an enumeration type with bit-fields attribute, which means you can add multiple choices
	/// into the value.
	/// </summary>
	public SearcherNodeTypes NodeTypes { get; set; } =
		SearcherNodeTypes.SoleDigit | SearcherNodeTypes.SoleCell
			| SearcherNodeTypes.LockedCandidates
			| SearcherNodeTypes.LockedSet | SearcherNodeTypes.HiddenSet;


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
		GatherStrongAndWeak_AlmostLockedSet(grid);

#if OUTPUT_INFERENCES && GET_ELIMINATIONS
#error Cannot set both symbols 'OUTPUT_INFERENCES' and 'GET_ELIMINATIONS'.
#elif OUTPUT_INFERENCES && !GET_ELIMINATIONS
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
#elif GET_ELIMINATIONS && !OUTPUT_INFERENCES
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
#else
#error You must set either the symbol 'OUTPUT_INFERENCES' or the symbol 'GET_ELIMINATIONS'.
#endif
	}

	/// <summary>
	/// To print the whole chain via the ID. The method is only used for calling by the debugger.
	/// </summary>
	/// <param name="chainIds">The IDs.</param>
#if DEBUG && GET_ELIMINATIONS
	private string PrintChainData(int[] chainIds) =>
		string.Join(" -> ", from id in chainIds select _id2NodeLookup[id].ToSimpleString());
#endif
}
