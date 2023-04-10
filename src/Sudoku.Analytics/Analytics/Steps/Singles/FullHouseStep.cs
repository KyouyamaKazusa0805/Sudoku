namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Full House</b> technique.
/// </summary>
public sealed class FullHouseStep(Conclusion[] conclusions, View[]? views, int cell, int digit) : SingleStep(conclusions, views, cell, digit)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 1.0M;

	/// <inheritdoc/>
	public override Technique Code => Technique.FullHouse;
}
