namespace Sudoku.Analytics.Patterning.Chaining;

/// <summary>
/// Represents a forcing chain branch.
/// </summary>
/// <param name="lastNode"><inheritdoc cref="ChainOrLoop(Node, bool, bool)" path="/param[@name='lastNode']"/></param>
public abstract class UnnamedChain(Node lastNode) : ChainOrLoop(lastNode, false, false)
{
	/// <inheritdoc/>
	public sealed override bool IsNamed => false;

	/// <inheritdoc/>
	protected sealed override int LoopIdentity => 1;

	/// <inheritdoc/>
	protected sealed override ReadOnlySpan<Node> ValidNodes => _nodes;
}
