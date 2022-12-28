namespace Sudoku.Solving.Logical.StepSearchers;

/// <summary>
/// Provides with an <b>Alternating Inference Chain</b> step searcher that uses same algorithm with <b>Chaining</b>
/// used by a program called Sudoku Explainer.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Irregular Wings:
/// <list type="bullet">
/// <item>W-Wing (Although it can be searched via <see cref="IIrregularWingStepSearcher"/>)</item>
/// <item>M-Wing</item>
/// <item>Split Wing</item>
/// <item>Local Wing</item>
/// <item>Hybrid Wing</item>
/// <item>Purple Cow</item>
/// </list>
/// </item>
/// <item>Discontinuous Nice Loop</item>
/// <item>Alternating Inference Chain</item>
/// <item>Continuous Nice Loop</item>
/// </list>
/// </summary>
/// <remarks>
/// The type is special: it uses source code from another project called Sudoku Explainer.
/// However unfortunately, I cannot find any sites available of the project.
/// One of the original website is <see href="https://diuf.unifr.ch/pai/people/juillera/Sudoku/Sudoku.html">this link</see> (A broken link).
/// </remarks>
public interface ISudokuExplainerCompatibleChainingStepSearcher : IChainStepSearcher
{
}
