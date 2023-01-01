namespace Sudoku.Solving.Logical.StepSearchers;

/// <summary>
/// Provides with a <b>Chain</b> step searcher using same algorithm with <b>Chaining</b> used by a program called Sudoku Explainer.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Alternating Inference Chains (Cycles)</item>
/// <item>
/// Forcing Chains:
/// <list type="bullet">
/// <item>
/// Multiple Forcing Chains:
/// <list type="bullet">
/// <item>Cell Forcing Chains</item>
/// <item>Region (House) Forcing Chains</item>
/// </list>
/// </item>
/// <item>
/// Dynamic Forcing Chains:
/// <list type="bullet">
/// <item>Dynamic Cell Forcing Chains</item>
/// <item>Dynamic Region (House) Forcing Chains</item>
/// <item>Dynamic Contradiction Forcing Chains</item>
/// <item>Dynamic Double Forcing Chains</item>
/// </list>
/// </item>
/// </list>
/// </item>
/// </list>
/// </summary>
/// <remarks>
/// The type is special: it uses source code from another project called Sudoku Explainer.
/// However unfortunately, I cannot find any sites available of the project.
/// One of the original website is <see href="https://diuf.unifr.ch/pai/people/juillera/Sudoku/Sudoku.html">this link</see> (A broken link).
/// </remarks>
public interface IChainingStepSearcher : IStepSearcher
{
}
