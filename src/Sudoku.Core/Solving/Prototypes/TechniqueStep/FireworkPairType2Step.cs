namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework Pair Type 2</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="DigitsMask">The digits used.</param>
/// <param name="Pattern1">The first pattern used.</param>
/// <param name="Pattern2">The second pattern used.</param>
/// <param name="ExtraCell">The extra cell used.</param>
internal sealed partial record FireworkPairType2Step(
	ConclusionList Conclusions,
	ViewList Views,
	short DigitsMask,
	scoped in FireworkPattern Pattern1,
	scoped in FireworkPattern Pattern2,
	int ExtraCell
) : FireworkStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty + .3M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.FireworkPairType2;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	[ResourceTextFormatter]
	private partial string ExtraCellStr() => RxCyNotation.ToCellString(ExtraCell);

	[ResourceTextFormatter]
	private partial string DigitsStr() => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	[ResourceTextFormatter]
	private partial string Firework1Str()
	{
		string cells = Pattern1.Map.ToString();
		string digits = DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);
		return $"{cells}({digits})";
	}

	[ResourceTextFormatter]
	private partial string Firework2Str()
	{
		string cells = Pattern2.Map.ToString();
		string digits = DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);
		return $"{cells}({digits})";
	}
}
