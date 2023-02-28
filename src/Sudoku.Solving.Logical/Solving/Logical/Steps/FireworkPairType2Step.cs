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


	[ResourceTextFormatter]
	internal string ExtraCellStr() => RxCyNotation.ToCellString(ExtraCell);

	[ResourceTextFormatter]
	internal string DigitsStr() => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	[ResourceTextFormatter]
	internal string Firework1Str()
	{
		var cells = Pattern1.Map.ToString();
		var digits = DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);
		return $"{cells}({digits})";
	}

	[ResourceTextFormatter]
	internal string Firework2Str()
	{
		var cells = Pattern2.Map.ToString();
		var digits = DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);
		return $"{cells}({digits})";
	}
}
