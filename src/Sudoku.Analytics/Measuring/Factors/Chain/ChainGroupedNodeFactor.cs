namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes grouped nodes in a chain or a loop.
/// </summary>
public sealed class ChainGroupedNodeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(NormalChainStep.Pattern)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(NormalChainStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula
		=> static args =>
		{
			var result = 0;
			var p = (ChainPattern)args![0]!;
			foreach (var link in p.Links)
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
			return result;
		};
}
