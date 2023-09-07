using Sudoku.Analytics.Categorization;
using Sudoku.Rendering;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Naked Single</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="cell"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
public sealed class NakedSingleStep(Conclusion[] conclusions, View[]? views, Cell cell, Digit digit) : SingleStep(conclusions, views, cell, digit)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 2.3M;

	/// <inheritdoc/>
	public override Technique Code => Technique.NakedSingle;
}
