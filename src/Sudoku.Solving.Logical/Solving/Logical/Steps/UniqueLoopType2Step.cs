namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Loop Type 2</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Loop"><inheritdoc/></param>
/// <param name="ExtraDigit">Indicates the extra digit.</param>
internal sealed record UniqueLoopType2Step(
	ConclusionList Conclusions,
	ViewList Views,
	int Digit1,
	int Digit2,
	scoped in CellMap Loop,
	int ExtraDigit
) : UniqueLoopStep(Conclusions, Views, Digit1, Digit2, Loop)
{
	/// <inheritdoc/>
	public override int Type => 2;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .1M;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { Digit1Str, Digit2Str, LoopStr, ExtraDigitStr } },
			{ "zh", new[] { Digit1Str, Digit2Str, LoopStr, ExtraDigitStr } }
		};

	private string ExtraDigitStr => (ExtraDigit + 1).ToString();
}
