namespace Sudoku.Solving.Logical.StepSearchers;

/// <summary>
/// Provides with a <b>Reverse Unique Rectangle</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Reverse Unique Rectangle Type 1</item>
/// <item>Reverse Unique Rectangle Type 2</item>
/// <!--
/// <item>Reverse Unique Rectangle Type 3</item>
/// <item>Reverse Unique Rectangle Type 4</item>
/// -->
/// </list>
/// </summary>
/// <!--
/// Test examples (May or may not be used):
/// 
/// 1) Split Reverse UR pairs
/// ......9...812...6.6.2.3.7.8...4...8....5.6....9...7...3.5.8.1.4.4...123...9......
/// -->
public interface IReverseUniqueRectangleStepSearcher : IReverseBivalueUniversalGraveStepSearcher
{
}
