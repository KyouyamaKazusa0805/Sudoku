namespace Sudoku.Analytics.Steps;

public partial class SubsetStep
{
	/// <inheritdoc/>
	public override int BaseDifficulty => 30;

	/// <inheritdoc/>
	public int Size => PopCount((uint)DigitsMask);
}
