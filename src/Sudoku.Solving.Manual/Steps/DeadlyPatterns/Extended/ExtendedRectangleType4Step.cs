namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle Type 4</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="ConjugatePair">Indicates the conjugate pair used.</param>
public sealed record class ExtendedRectangleType4Step(
	ConclusionList Conclusions, ViewList Views, scoped in Cells Cells,
	short DigitsMask, scoped in Conjugate ConjugatePair) :
	ExtendedRectangleStep(Conclusions, Views, Cells, DigitsMask),
	IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override int Type => 4;

	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public new decimal BaseDifficulty => base.Difficulty;

	/// <inheritdoc/>
	public new (string Name, decimal Value)[] ExtraDifficultyValues => new[] { ("Conjugate pair", .1M) };

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	[FormatItem]
	internal string ConjStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ConjugatePair.ToString();
	}
}
