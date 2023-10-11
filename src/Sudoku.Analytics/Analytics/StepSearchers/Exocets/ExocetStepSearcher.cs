using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Junior Exocet</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Junior Exocet</item>
/// <item>Senior Exocet</item>
/// </list>
/// </summary>
[StepSearcher(Technique.JuniorExocet, Technique.SeniorExocet)]
public sealed partial class ExocetStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		return null;
	}
}
