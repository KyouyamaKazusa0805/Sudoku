using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// (<b>Not implemented</b>) Provides with a <b>Senior Exocet</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Senior Exocet</item>
/// </list>
/// </summary>
[StepSearcher(Technique.SeniorExocet, Technique.ComplexSeniorExocet, Technique.SiameseSeniorExocet)]
public sealed partial class SeniorExocetStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates whether the searcher will find advanced eliminations.
	/// </summary>
	[RuntimeIdentifier(RuntimeIdentifier.CheckAdvancedSeniorExocet)]
	public bool CheckAdvanced { get; set; }


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		// TODO: Re-implement SE.
		return null;
	}
}
