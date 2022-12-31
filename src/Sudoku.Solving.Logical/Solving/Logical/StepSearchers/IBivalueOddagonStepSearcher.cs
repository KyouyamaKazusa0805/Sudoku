namespace Sudoku.Solving.Logical.StepSearchers;

/// <summary>
/// <para>
/// Provides with a <b>Bi-value Oddagon</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Bi-value Oddagon Type 2</item>
/// <item>Bi-value Oddagon Type 3</item>
/// </list>
/// </para>
/// <para>
/// In practicing, type 1 and 4 do not exist. A bi-value oddagon type 1 is a remote pair
/// and a type 4 cannot be formed as a stable technique structure.
/// </para>
/// <para>A remote pair is a XY-Chain that only uses two digits.</para>
/// </summary>
public interface IBivalueOddagonStepSearcher : INegativeRankStepSearcher
{
}
