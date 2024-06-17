namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes grouped nodes in a multiple forcing chains.
/// </summary>
public sealed class MultipleForcingChainsGroupedNodeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(MultipleForcingChainsStep.Pattern)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(MultipleForcingChainsStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula
		=> static args =>
		{
			var result = 0;
			foreach (var branch in ((MultipleForcingChains)args![0]!).Values)
			{
				foreach (var link in branch.Links)
				{
					result += link.GroupedLinkPattern switch
					{
						AlmostLockedSet => 2,
						AlmostHiddenSet => 3,
						UniqueRectangle => 4,
						Fish => 6,
						XyzWing => 8,
						null when link.FirstNode.IsGroupedNode || link.SecondNode.IsGroupedNode => 1,
						_ => 0
					};
				}
			}
			return result;
		};
}
