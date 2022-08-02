namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework Pair Type 2</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells">The cells used.</param>
/// <param name="DigitsMask">The digits used.</param>
/// <param name="Pattern1">The first pattern used.</param>
/// <param name="Pattern2">The second pattern used.</param>
/// <param name="ExtraCell">The extra cell used.</param>
public sealed record FireworkPairType2Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in Cells Cells,
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

	[FormatItem]
	internal string ExtraCellStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => RxCyNotation.ToCellString(ExtraCell);
	}

	[FormatItem]
	internal string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(DigitsMask).ToString();
	}

	[FormatItem]
	internal string Firework1Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			string cells = Pattern1.Map.ToString();
			string digits = new DigitCollection(DigitsMask).ToString();
			return $"{cells}({digits})";
		}
	}

	[FormatItem]
	internal string Firework2Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			string cells = Pattern2.Map.ToString();
			string digits = new DigitCollection(DigitsMask).ToString();
			return $"{cells}({digits})";
		}
	}
}
