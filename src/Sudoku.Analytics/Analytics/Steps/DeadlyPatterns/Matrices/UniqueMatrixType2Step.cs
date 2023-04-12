namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Square Type 2</b> technique.
/// </summary>
public sealed class UniqueMatrixType2Step(Conclusion[] conclusions, View[]? views, scoped in CellMap cells, Mask digitsMask, int extraDigit) :
	UniqueMatrixStep(conclusions, views, cells, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 2;

	/// <summary>
	/// Indicates the extra digit used.
	/// </summary>
	public int ExtraDigit { get; } = extraDigit;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => new[] { (ExtraDifficultyCaseNames.ExtraDigit, .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitsStr, CellsStr, ExtraDigitStr } },
			{ "zh", new[] { ExtraDigitStr, CellsStr, DigitsStr } }
		};

	private string ExtraDigitStr => (ExtraDigit + 1).ToString();
}
