namespace Sudoku.Analytics.Construction.Drivers;

internal partial class ChainingDriver
{
	/// <summary>
	/// Finds a list of nodes that can implicitly connects to current node via a forcing chain,
	/// with tracking contradiction checking.
	/// This method won't use cached dictionary.
	/// </summary>
	/// <param name="startNode">The current instance.</param>
	/// <param name="grid">Indicates the interim cached grid.</param>
	/// <param name="chainingRules">Indicates the chaining rules to be used.</param>
	/// <param name="options">Indicates the options used.</param>
	/// <param name="contradiction">
	/// The found contradiction node pairs.
	/// If there's no contradiction here, this argument will keep the value <see langword="null"/>.
	/// </param>
	/// <returns>
	/// A pair of <see cref="HashSet{T}"/> of <see cref="Node"/> instances, indicating all possible nodes
	/// that can implicitly connects to the current node via the whole forcing chain, grouped by their own initial states,
	/// encapsulating with type <see cref="ForcingChainInfo"/>.
	/// </returns>
	/// <seealso cref="StrongLinkDictionary"/>
	/// <seealso cref="WeakLinkDictionary"/>
	/// <seealso cref="HashSet{T}"/>
	/// <seealso cref="Node"/>
	/// <seealso cref="ForcingChainInfo"/>
	private static ForcingChainInfo FindForcingChainsTrackingContradiction(
		Node startNode,
		ref readonly Grid grid,
		ChainingRuleCollection chainingRules,
		StepGathererOptions options,
		out (Node On, Node Off)? contradiction
	)
	{
		(contradiction, var (pendingNodesSupposedOn, pendingNodesSupposedOff)) = (null, (new LinkedList<Node>(), new LinkedList<Node>()));
		(startNode.IsOn ? pendingNodesSupposedOn : pendingNodesSupposedOff).AddLast(startNode);

		var tempGrid = grid;
		var nodesSupposedOn = new HashSet<Node>(ChainingComparers.NodeMapComparer);
		var nodesSupposedOff = new HashSet<Node>(ChainingComparers.NodeMapComparer);
		while (pendingNodesSupposedOn.Count != 0 || pendingNodesSupposedOff.Count != 0)
		{
			if (pendingNodesSupposedOn.Count != 0)
			{
				var currentNode = pendingNodesSupposedOn.RemoveFirstNode();
				if (GetNodesFromOnToOff(currentNode, chainingRules, options, in tempGrid) is var supposedOff and not [])
				{
					foreach (var node in supposedOff)
					{
						var nextNode = node >> currentNode;
						if (nodesSupposedOn.Contains(~nextNode))
						{
							// Contradiction is found.
							contradiction = (~node, node);
							goto ReturnResult;
						}

						if (nodesSupposedOff.Add(nextNode))
						{
							pendingNodesSupposedOff.AddLast(nextNode);
						}
					}
				}
			}
			else
			{
				var currentNode = pendingNodesSupposedOff.RemoveFirstNode();
				tempGrid.Apply(currentNode);

				if (GetNodesFromOffToOn(currentNode, chainingRules, options, in tempGrid) is var supposedOn and not [])
				{
					foreach (var node in supposedOn)
					{
						var nextNode = node >> currentNode;
						if (nodesSupposedOff.Contains(~nextNode))
						{
							// Contradiction is found.
							contradiction = (node, ~node);
							goto ReturnResult;
						}

						if (nodesSupposedOn.Add(nextNode))
						{
							pendingNodesSupposedOn.AddLast(nextNode);
						}
					}
				}
			}
		}

	ReturnResult:
		// Returns the found result.
		return new(nodesSupposedOn, nodesSupposedOff);
	}

	/// <summary>
	/// Try to find nodes that will be considered as "off" state if the specified node is "on" state.
	/// </summary>
	/// <param name="node">The previous node to be checked.</param>
	/// <param name="options">The options.</param>
	/// <param name="chainingRules">The chaining rule collection to be used.</param>
	/// <param name="grid">The grid to be checked.</param>
	/// <returns>A list of nodes found.</returns>
	/// <remarks><i>
	/// This method doesn't use cache on purpose, therefore this method won't be marked type
	/// <see cref="InterceptorMethodCallerAttribute"/>.
	/// </i></remarks>
	/// <seealso cref="InterceptorMethodCallerAttribute"/>
	private static ReadOnlySpan<Node> GetNodesFromOnToOff(Node node, ChainingRuleCollection chainingRules, StepGathererOptions options, ref readonly Grid grid)
	{
		var context = new ChainingRuleNextNodeContext(node, in grid, options);
		foreach (var chainingRule in chainingRules)
		{
			chainingRule.GetWeakLinks(ref context);
		}
		return context.CollectedNodes;
	}

	/// <summary>
	/// Try to find nodes that will be considered as "on" state if the specified node is "off" state.
	/// </summary>
	/// <inheritdoc cref="GetNodesFromOnToOff(Node, ChainingRuleCollection, StepGathererOptions, ref readonly Grid)"/>
	private static ReadOnlySpan<Node> GetNodesFromOffToOn(Node node, ChainingRuleCollection chainingRules, StepGathererOptions options, ref readonly Grid grid)
	{
		var context = new ChainingRuleNextNodeContext(node, in grid, options);
		foreach (var chainingRule in chainingRules)
		{
			chainingRule.GetStrongLinks(ref context);
		}
		return context.CollectedNodes;
	}
}
