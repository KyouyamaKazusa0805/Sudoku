namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a forcing chains component.
/// </summary>
public interface IForcingChains : IChainOrForcingChains
{
	/// <summary>
	/// Indicates the complexity of the whole pattern.
	/// </summary>
	public abstract int Complexity { get; }

	/// <summary>
	/// Indicates the digits used in this pattern.
	/// </summary>
	public abstract Mask DigitsMask { get; }

	/// <summary>
	/// Indicates the conclusions of the pattern.
	/// </summary>
	public abstract StepConclusions Conclusions { get; }

	/// <summary>
	/// Indicates the complexity of each branch.
	/// </summary>
	public abstract ReadOnlySpan<int> BranchedComplexity { get; }


	/// <summary>
	/// Collect views for the current chain.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="newConclusions">The conclusions.</param>
	/// <param name="supportedRules">The supported rules.</param>
	/// <returns>The views.</returns>
	public abstract View[] GetViews(ref readonly Grid grid, Conclusion[] newConclusions, ChainingRuleCollection supportedRules);
}
