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
	protected static (Link, ColorIdentifier)[] GetViewOnLinks(in AlternatingInferenceChain chain)
	{
		var realChainNodes = chain.RealChainNodes;
		var result = new (Link, ColorIdentifier)[realChainNodes.Length];
		for (int i = 0; i < realChainNodes.Length - 1; i++)
		{
			// TODO: I'll re-implement the data structure to support grouped nodes.
			if (
#pragma warning disable IDE0055
				(realChainNodes[i], realChainNodes[i + 1]) is (
					{ Cells: [var aFirst, ..], Digit: var aDigit },
					{ Cells: [var bFirst, ..], Digit: var bDigit }
				)
#pragma warning restore IDE0055
			)
			{
				result[i] = (
					new(aFirst * 9 + aDigit, bFirst * 9 + bDigit, LinkKind.Strong),
					(ColorIdentifier)0
				);
			}
		}

		return result;
	}
}
