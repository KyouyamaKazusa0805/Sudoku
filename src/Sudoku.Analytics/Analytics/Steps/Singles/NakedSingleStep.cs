namespace Sudoku.Analytics.Steps;

public partial class NakedSingleStep
{
	/// <inheritdoc/>
	public override int BaseDifficulty => Options.IsDirectMode ? 23 : 10;

	/// <inheritdoc/>
	public override Technique Code => Technique.NakedSingle;
}
