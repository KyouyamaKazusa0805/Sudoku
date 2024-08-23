namespace Sudoku.Runtime.MeasuringServices.Factors;

/// <summary>
/// Represents a factor that describes grouped nodes in a blossom loop.
/// </summary>
public sealed class BlossomLoopGroupedNodeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(BlossomLoopStep.Pattern)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(BlossomLoopStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula
		=> static args =>
		{
			var result = 0;
			foreach (var branch in ((BlossomLoop)args![0]!).Values)
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
