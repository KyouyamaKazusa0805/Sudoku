namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Loop Type 3</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Loop"><inheritdoc/></param>
/// <param name="SubsetDigitsMask">
/// Indicates the mask that contains the subset digits used in this instance.
/// </param>
/// <param name="SubsetCells">Indicates the subset cells.</param>
[StepDisplayingFeature(StepDisplayingFeature.DifficultyRatingNotStable)]
internal sealed record UniqueLoopType3Step(
	ConclusionList Conclusions,
	ViewList Views,
	int Digit1,
	int Digit2,
	scoped in CellMap Loop,
	short SubsetDigitsMask,
	scoped in CellMap SubsetCells
) : UniqueLoopStep(Conclusions, Views, Digit1, Digit2, Loop)
{
	/// <inheritdoc/>
	public override int Type => 3;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new[] { base.ExtraDifficultyCases[0], new(ExtraDifficultyCaseNames.Size, SubsetCells.Count * .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { Digit1Str, Digit2Str, LoopStr, SubsetName, DigitsStr, SubsetCellsStr } },
			{ "zh", new[] { Digit1Str, Digit2Str, LoopStr, SubsetName, DigitsStr, SubsetCellsStr } }
		};

	private string SubsetCellsStr => SubsetCells.ToString();

	private string DigitsStr => DigitMaskFormatter.Format(SubsetDigitsMask, FormattingMode.Normal);

	private string SubsetName => R[$"SubsetNamesSize{SubsetCells.Count + 1}"]!;
}
