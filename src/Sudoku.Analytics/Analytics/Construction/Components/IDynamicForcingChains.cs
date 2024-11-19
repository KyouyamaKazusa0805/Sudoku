namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a dynamic forcing chains.
/// </summary>
public interface IDynamicForcingChains : IForcingChains
{
	/// <summary>
	/// Indicates whether the chain is dynamic.
	/// </summary>
	public abstract bool IsDynamic { get; }
}
