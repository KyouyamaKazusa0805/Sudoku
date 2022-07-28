namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Normal Fish</b> step searcher. The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Normal fishes:
/// <list type="bullet">
/// <item>X-Wing</item>
/// <item>Swordfish</item>
/// <item>Jellyfish</item>
/// </list>
/// </item>
/// <item>
/// Finned fishes:
/// <list type="bullet">
/// <item>
/// Finned normal fishes:
/// <list type="bullet">
/// <item>Finned X-Wing</item>
/// <item>Finned Swordfish</item>
/// <item>Finned Jellyfish</item>
/// </list>
/// </item>
/// <item>
/// Finned sashimi fishes:
/// <list type="bullet">
/// <item>Sashimi X-Wing</item>
/// <item>Sashimi Swordfish</item>
/// <item>Sashimi Jellyfish</item>
/// </list>
/// </item>
/// </list>
/// </item>
/// </list>
/// </summary>
public interface INormalFishStepSearcher : IFishStepSearcher
{
}
