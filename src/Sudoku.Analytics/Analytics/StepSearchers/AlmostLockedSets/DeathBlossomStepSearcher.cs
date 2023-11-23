using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.StepSearcherModules;

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Death Blossom</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Death Blossom</item>
/// </list>
/// </summary>
[StepSearcher(Technique.DeathBlossom)]
[StepSearcherRuntimeName("StepSearcherName_DeathBlossomStepSearcher")]
public sealed partial class DeathBlossomStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		_ = AlmostLockedSetsModule.CollectAlmostLockedSets(in grid);

		return null;
	}
}
