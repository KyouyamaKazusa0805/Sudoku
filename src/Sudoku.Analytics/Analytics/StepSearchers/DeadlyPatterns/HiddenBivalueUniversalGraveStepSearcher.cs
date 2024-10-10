namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Hidden Bi-value Universal Grave</b> (Bi-local Universal Grave) step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Bi-local Universal Grave Type 1</item>
/// <item>Bi-local Universal Grave Type 2</item>
/// <item>Bi-local Universal Grave Type 3</item>
/// <item>Bi-local Universal Grave Type 4</item>
/// </list>
/// </summary>
[StepSearcher(
	"StepSearcherName_HiddenBivalueUniversalGraveStepSearcher",
	Technique.HiddenBivalueUniversalGraveType1, Technique.HiddenBivalueUniversalGraveType2,
	Technique.HiddenBivalueUniversalGraveType3, Technique.HiddenBivalueUniversalGraveType4,
	SupportedSudokuTypes = SudokuType.Standard,
	SupportAnalyzingMultipleSolutionsPuzzle = false)]
public sealed partial class HiddenBivalueUniversalGraveStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(ref StepAnalysisContext context)
	{
		// Test examples:
		// +3+1+5+9+64+782+279+38+1+54+6.6.7+25.9...........312......8.43...7...6....5.4.8...7...3...9..:541 243 643 545 147 247 447 148 548 149 349 268 668 273 976 177 277 377 681 286 187 387 591 595 199

		return null;
	}
}
