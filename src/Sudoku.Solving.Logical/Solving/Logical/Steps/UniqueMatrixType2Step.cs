namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Square Type 2</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="ExtraDigit">Indicates the extra digit used.</param>
internal sealed record UniqueMatrixType2Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Cells,
	short DigitsMask,
	int ExtraDigit
) : UniqueMatrixStep(Conclusions, Views, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override int Type => 2;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.ExtraDigit, .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitsStr, CellsStr, ExtraDigitStr } },
			{ "zh", new[] { ExtraDigitStr, CellsStr, DigitsStr } }
		};

	private string ExtraDigitStr => (ExtraDigit + 1).ToString();
}
