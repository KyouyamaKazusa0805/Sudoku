namespace Sudoku.Concepts;

/// <summary>
/// Represents a normal chain or loop.
/// </summary>
/// <param name="lastNode"><inheritdoc cref="ChainOrLoop(Node, bool, bool)" path="/param[@name='lastNode']"/></param>
/// <param name="isLoop"><inheritdoc cref="ChainOrLoop(Node, bool, bool)" path="/param[@name='isLoop']"/></param>
/// <param name="autoReversingOnComparison">
/// <inheritdoc cref="ChainOrLoop(Node, bool, bool)" path="/param[@name='autoReversingOnComparison']"/>
/// </param>
public abstract class NamedChain(Node lastNode, bool isLoop, bool autoReversingOnComparison = true) :
	ChainOrLoop(lastNode, isLoop, autoReversingOnComparison)
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
