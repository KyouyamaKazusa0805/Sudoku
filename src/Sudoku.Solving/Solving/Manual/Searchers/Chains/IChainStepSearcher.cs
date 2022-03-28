using Sudoku.Presentation;
using Sudoku.Solving.Collections;

namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for chain steps.
/// </summary>
public interface IChainStepSearcher : IStepSearcher
{
	/// <summary>
	/// Creates an array of the presentation data of links
	/// via the specified instance of type <see cref="AlternatingInferenceChain"/>.
	/// </summary>
	/// <param name="chain">The chain.</param>
	/// <returns>An array of presentation data of links.</returns>
	protected static LinkViewNode[] GetViewOnLinks(in AlternatingInferenceChain chain)
	{
		var realChainNodes = chain.RealChainNodes;
		var result = new LinkViewNode[realChainNodes.Length];
		for (int i = 0; i < realChainNodes.Length - 1; i++)
		{
			if (realChainNodes[i] is { Cells: var aCells, Digit: var aDigit }
				&& realChainNodes[i + 1] is { Cells: var bCells, Digit: var bDigit })
			{
				result[i] = new(0, new(aDigit, aCells), new(bDigit, bCells), LinkKind.Strong);
			}
		}

		return result;
	}
}
