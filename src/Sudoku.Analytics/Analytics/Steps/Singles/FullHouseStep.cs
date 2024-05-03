namespace Sudoku.Analytics.Steps;

public partial class FullHouseStep
{
	/// <inheritdoc/>
	public override int BaseDifficulty => 10;

	/// <inheritdoc/>
	public override Technique Code => Technique.FullHouse;
}
