namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave Type 4</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="DigitsMask">Indicates the digits used.</param>
/// <param name="Cells">Indicates the cells used.</param>
/// <param name="ConjugatePair">Indicates the conjugate pair used.</param>
internal sealed record BivalueUniversalGraveType4Step(
	ConclusionList Conclusions,
	ViewList Views,
	short DigitsMask,
	scoped in Cells Cells,
	scoped in Conjugate ConjugatePair
) : BivalueUniversalGraveStep(Conclusions, Views), IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => base.Difficulty;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[] { (PhasedDifficultyRatingKinds.ConjugatePair, .1M) };

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueUniversalGraveType4;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	[FormatItem]
	internal string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);
	}

	[FormatItem]
	internal string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Cells.ToString();
	}

	[FormatItem]
	internal string ConjStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ConjugatePair.ToString();
	}
}
