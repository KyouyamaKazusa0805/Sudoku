namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents named chain patterns:
/// <list type="bullet">
/// <item>Alternating inference chain (corresponds to type <see cref="AlternatingInferenceChain"/>)</item>
/// <item>Continuous nice loop (corresponds to type <see cref="ContinuousNiceLoop"/>)</item>
/// </list>
/// </summary>
/// <param name="lastNode"><inheritdoc cref="Chain(Node, bool, bool)" path="/param[@name='lastNode']"/></param>
/// <param name="isLoop"><inheritdoc cref="Chain(Node, bool, bool)" path="/param[@name='isLoop']"/></param>
/// <seealso cref="AlternatingInferenceChain"/>
/// <seealso cref="ContinuousNiceLoop"/>
public abstract class NamedChain(Node lastNode, bool isLoop) : Chain(lastNode, isLoop, true)
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
