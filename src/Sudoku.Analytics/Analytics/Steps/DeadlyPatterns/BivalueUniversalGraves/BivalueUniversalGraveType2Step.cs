namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave Type 2</b> technique.
/// </summary>
public sealed class BivalueUniversalGraveType2Step(Conclusion[] conclusions, View[]? views, int digit, scoped in CellMap cells) :
	BivalueUniversalGraveStep(conclusions, views)
{
	/// <summary>
	/// Indicates the extra digit.
	/// </summary>
	public int ExtraDigit { get; } = digit;

	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGraveType2;

	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	public CellMap Cells { get; } = cells;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => new[] { (ExtraDifficultyCaseNames.ExtraDigit, A002024(Cells.Count) * .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { ExtraDigitStr, CellsStr } }, { "zh", new[] { CellsStr, ExtraDigitStr } } };

	private string ExtraDigitStr => (digit + 1).ToString();

	private string CellsStr => Cells.ToString();
}
