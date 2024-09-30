namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a normal chain or loop.
/// </summary>
/// <param name="lastNode"><inheritdoc cref="ChainOrLoop(Node, bool, bool)" path="/param[@name='lastNode']"/></param>
/// <param name="isLoop"><inheritdoc cref="ChainOrLoop(Node, bool, bool)" path="/param[@name='isLoop']"/></param>
public abstract class NamedChain(Node lastNode, bool isLoop) : ChainOrLoop(lastNode, isLoop, true)
{
	/// <inheritdoc/>
	public sealed override bool IsNamed => true;


	/// <summary>
	/// Try to get a <see cref="ConclusionSet"/> instance that contains all conclusions created by using the current chain.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <returns>A <see cref="ConclusionSet"/> instance. By default the method returns an empty conclusion set.</returns>
	public virtual ConclusionSet GetConclusions(ref readonly Grid grid) => ConclusionSet.Empty;
}
