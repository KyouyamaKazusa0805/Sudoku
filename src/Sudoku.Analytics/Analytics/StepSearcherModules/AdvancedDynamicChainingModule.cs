namespace Sudoku.Analytics.StepSearcherModules;

/// <summary>
/// Represents the advanced dynamic chaining module.
/// </summary>
[Obsolete]
internal sealed class AdvancedDynamicChainingModule : ChainingModule
{
	/// <inheritdoc cref="ChainingModule.OnAdvanced{T}(T, NodeList, NodeList, NodeSet, ref readonly Grid, ref readonly Grid)"/>
	public static void OnAdvanced(
		AdvancedMultipleChainingStepSearcher stepSearcher,
		NodeList pendingOn,
		NodeList pendingOff,
		NodeSet toOff,
		ref readonly Grid grid,
		ref readonly Grid original
	)
	{
		if (pendingOn.Count == 0 && pendingOff.Count == 0 && stepSearcher.DynamicNestingLevel > 0)
		{
			foreach (var pOff in GetAdvancedPotentials(stepSearcher, in grid, in original, toOff))
			{
				if (toOff.Add(pOff))
				{
					// Not processed yet.
					pendingOff.AddLast(pOff);
				}
			}
		}
	}

	/// <summary>
	/// Get all non-trivial implications (involving fished, naked/hidden sets, etc).
	/// </summary>
	/// <param name="stepSearcher">The step searcher.</param>
	/// <param name="grid">Indicates the current grid state.</param>
	/// <param name="original">Indicates the original grid state.</param>
	/// <param name="offPotentials">The candidates marked "off".</param>
	/// <returns>Found <see cref="ChainNode"/> instances.</returns>
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
	private static NodeList GetAdvancedPotentials(
		AdvancedMultipleChainingStepSearcher stepSearcher,
		ref readonly Grid grid,
		ref readonly Grid original,
		NodeSet offPotentials
	)
	{
		stepSearcher._otherStepSearchers ??= [
			(1, [new LockedCandidatesStepSearcher(), new LockedSubsetStepSearcher(), new NormalFishStepSearcher(), new NormalSubsetStepSearcher()]),
#if false
			(2, [new NonMultipleChainingStepSearcher()]),
#endif
			(3, [new MultipleChainingStepSearcher { AllowMultiple = true }]),
			(4, [new MultipleChainingStepSearcher { AllowDynamic = true, AllowMultiple = true }]),
			(5, [new AdvancedMultipleChainingStepSearcher { DynamicNestingLevel = stepSearcher.DynamicNestingLevel - 3 }])
		];

		var result = new NodeList();
		return result;
	}
}
