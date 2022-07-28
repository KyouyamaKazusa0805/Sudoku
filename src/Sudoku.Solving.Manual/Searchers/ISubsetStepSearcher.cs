namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Subset</b> step searcher. The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Locked Subsets:
/// <list type="bullet">
/// <item>Locked Pair</item>
/// <item>Locked Triple</item>
/// </list>
/// </item>
/// <item>
/// Semi-Locked Subsets:
/// <list type="bullet">
/// <item>Naked Pair (+)</item>
/// <item>Naked Triple (+)</item>
/// <item>Naked Quadruple (+)</item>
/// </list>
/// </item>
/// <item>
/// Normal Naked Subsets:
/// <list type="bullet">
/// <item>Naked Pair</item>
/// <item>Naked Triple</item>
/// <item>Naked Quadruple</item>
/// </list>
/// </item>
/// <item>
/// Hidden Subsets:
/// <list type="bullet">
/// <item>Hidden Pair</item>
/// <item>Hidden Triple</item>
/// <item>Hidden Quadruple</item>
/// </list>
/// </item>
/// </list>
/// </summary>
public interface ISubsetStepSearcher : IStepSearcher
{
	/// <summary>
	/// Indicates the maximum size of the searcher can search for.
	/// </summary>
	public abstract int MaxSize { get; set; }
}
