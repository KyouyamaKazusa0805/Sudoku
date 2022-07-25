namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for template steps.
/// </summary>
public interface ITemplateStepSearcher : IStepSearcher, IStepSearcherRequiresSolution
{
	/// <summary>
	/// Indicates whether the technique searcher only checks template deletes.
	/// </summary>
	public abstract bool TemplateDeleteOnly { get; set; }
}
