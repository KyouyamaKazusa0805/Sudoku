using Sudoku.Collections;
using Sudoku.Presentation;

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
		return null;
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
}
