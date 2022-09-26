namespace Sudoku.Solving.Logical.Implementations.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Type 3</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Pattern"><inheritdoc/></param>
/// <param name="ExtraDigitsMask">Indicates the extra digits used to form the subset.</param>
/// <param name="ExtraCells">Indicates the extra cells used.</param>
/// <param name="IsNaked">Indicates whether the subset is a naked subset.</param>
[StepDisplayingFeature(StepDisplayingFeature.VeryRare | StepDisplayingFeature.DifficultyRatingNotStable)]
internal sealed record QiuDeadlyPatternType3Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in QiuDeadlyPattern Pattern,
	short ExtraDigitsMask,
	scoped in CellMap ExtraCells,
	bool IsNaked
) : QiuDeadlyPatternStep(Conclusions, Views, Pattern), IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => base.Difficulty;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[] { (PhasedDifficultyRatingKinds.Size, PopCount((uint)ExtraDigitsMask) * .1M) };

	/// <inheritdoc/>
	public override int Type => 3;

	[ResourceTextFormatter]
	internal string DigitsStr() => DigitMaskFormatter.Format(ExtraDigitsMask, FormattingMode.Normal);

	[ResourceTextFormatter]
	internal string CellsStr() => ExtraCells.ToString();

	[ResourceTextFormatter]
	internal string SubsetName() => R[$"SubsetNamesSize{ExtraCells.Count + 1}"]!;
}
