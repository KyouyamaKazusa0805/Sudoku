namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Type 2</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Pattern"><inheritdoc/></param>
/// <param name="ExtraDigit">Indicates the extra digit used.</param>
internal sealed record QiuDeadlyPatternType2Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in QiuDeadlyPattern Pattern,
	int ExtraDigit
) : QiuDeadlyPatternStep(Conclusions, Views, Pattern), IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => base.Difficulty;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[] { (PhasedDifficultyRatingKinds.ExtraDigit, .1M) };

	/// <inheritdoc/>
	public override int Type => 2;

	[FormatItem]
	internal string ExtraDigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ExtraDigit + 1).ToString();
	}
}
