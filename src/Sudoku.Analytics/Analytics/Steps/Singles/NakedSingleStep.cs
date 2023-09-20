using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Rendering;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Naked Single</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cell"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
public sealed class NakedSingleStep(Conclusion[] conclusions, View[]? views, StepSearcherOptions options, Cell cell, Digit digit) :
	SingleStep(conclusions, views, options, cell, digit)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 2.3M;

	/// <inheritdoc/>
	public override Technique Code => Technique.NakedSingle;
}
