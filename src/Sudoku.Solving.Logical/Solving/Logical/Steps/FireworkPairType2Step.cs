namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework Pair Type 2</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="DigitsMask">The digits used.</param>
/// <param name="Pattern1">The first pattern used.</param>
/// <param name="Pattern2">The second pattern used.</param>
/// <param name="ExtraCell">The extra cell used.</param>
internal sealed record FireworkPairType2Step(
	ConclusionList Conclusions,
	ViewList Views,
	short DigitsMask,
	scoped in Firework Pattern1,
	scoped in Firework Pattern2,
	int ExtraCell
) : FireworkStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .3M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.FireworkPairType2;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { Firework1Str, Firework2Str, DigitsStr, ExtraCellStr } },
			{ "zh", new[] { Firework1Str, Firework2Str, DigitsStr, ExtraCellStr } }
		};

	private string ExtraCellStr => RxCyNotation.ToCellString(ExtraCell);

	private string DigitsStr => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	private string Firework1Str
	{
		get
		{
			var cells = Pattern1.Map.ToString();
			var digits = DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);
			return $"{cells}({digits})";
		}
	}

	private string Firework2Str
	{
		get
		{
			var cells = Pattern2.Map.ToString();
			var digits = DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);
			return $"{cells}({digits})";
		}
	}
}
