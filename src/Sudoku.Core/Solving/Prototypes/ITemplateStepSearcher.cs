namespace Sudoku.Solving.Prototypes;

/// <summary>
/// Provides with a <b>Template</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Template Set</item>
/// <item>Template Delete</item>
/// </list>
/// </summary>
public interface ITemplateStepSearcher : IStepSearcher, IStepSearcherRequiresSolution
{
	/// <summary>
	/// Indicates whether the technique searcher only checks template deletes.
	/// </summary>
	public abstract bool TemplateDeleteOnly { get; set; }
}
