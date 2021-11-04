namespace Sudoku.Solving.Manual.Searchers.LastResorts;

/// <summary>
/// Defines a step searcher that searches for template steps.
/// </summary>
public unsafe interface ITemplateStepSearcher : IStepSearcher
{
	/// <summary>
	/// Indicates whether the technique searcher only checks template deletes.
	/// </summary>
	bool TemplateDeleteOnly { get; set; }

	/// <summary>
	/// Indicates the reference of the solution sudoku grid.
	/// </summary>
	/// <remarks>
	/// Please note that the property will be received a pointer value that points to a <see cref="Grid"/>,
	/// but we recommend you should keep the inner value and only assign once.
	/// </remarks>
	Grid* Solution { get; set; }
}
