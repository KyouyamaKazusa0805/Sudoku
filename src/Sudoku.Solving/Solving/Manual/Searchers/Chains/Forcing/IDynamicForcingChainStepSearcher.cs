namespace Sudoku.Solving.Manual.Searchers.Chains.Forcing;

/// <summary>
/// Defines a step searcher that searches for dynamic forcing chain steps.
/// </summary>
public interface IDynamicForcingChainStepSearcher : IForcingChainStepSearcher
{
	/// <summary>
	/// Indicates whether the step searcher will search for dynamic forcing chains.
	/// </summary>
	bool IsDynamic { get; set; }

	/// <summary>
	/// Indicates the level of the dynamic case. The dynamic level contains 6 possible values, which is corresponding
	/// to the digit 0, 1, 2, 3, 4 and 5.
	/// <list type="table">
	/// <item>
	/// <term>Level 0</term>
	/// <description>The dynamic case is disabled. The chain isn't a dynamic chain at all.</description>
	/// </item>
	/// <item>
	/// <term>Level 1</term>
	/// <description>The chain is a normal dynamic forcing chains.</description>
	/// </item>
	/// <item>
	/// <term>Level 2</term>
	/// <description>
	/// The chain is a dynamic forcing chains with grouped nodes, such as an X-Wing.
	/// (i.e. <b>Dynamic Forcing Chains + Generalized Structure</b>)
	/// </description>
	/// </item>
	/// <item>
	/// <term>Level 3</term>
	/// <description>
	/// The chain is a dynamic forcing chains with normal AICs as grouped nodes.
	/// (i.e. <b>Dynamic Forcing Chains + Alternating Inference Chain</b>)
	/// </description>
	/// </item>
	/// <item>
	/// <term>Level 4</term>
	/// <description>
	/// The chain is a dynamic forcing chains with normal forcing chains as grouped nodes.
	/// (i.e. <b>Dynamic Forcing Chains + Forcing Chains</b>)
	/// </description>
	/// </item>
	/// <item>
	/// <term>Level 5</term>
	/// <description>
	/// The chain is a dynamic forcing chains with dynamic forcing chains as grouped nodes.
	/// (i.e. <b>Dynamic Forcing Chains + Dynamic Forcing Chains</b>)
	/// </description>
	/// </item>
	/// </list>
	/// </summary>
	byte Level { get; set; }
}