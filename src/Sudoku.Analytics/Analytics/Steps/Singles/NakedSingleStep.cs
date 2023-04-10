namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Naked Single</b> technique.
/// </summary>
public sealed class NakedSingleStep(Conclusion[] conclusions, View[]? views, int cell, int digit) : SingleStep(conclusions, views, cell, digit)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 2.3M;

	/// <inheritdoc/>
	public override Technique Code => Technique.NakedSingle;
}
