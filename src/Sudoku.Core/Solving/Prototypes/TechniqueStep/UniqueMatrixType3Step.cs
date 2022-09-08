namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Square Type 3</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="ExtraCells">Indicates the extra cells used.</param>
/// <param name="ExtraDigitsMask">
/// Indicates the extra digits that forms a subset with <paramref name="DigitsMask"/>.
/// </param>
[StepDisplayingFeature(StepDisplayingFeature.DifficultyRatingNotStable)]
internal sealed partial record UniqueMatrixType3Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Cells,
	short DigitsMask,
	short ExtraDigitsMask,
	scoped in CellMap ExtraCells
) : UniqueMatrixStep(Conclusions, Views, Cells, DigitsMask), IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => base.Difficulty;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[] { (PhasedDifficultyRatingKinds.ExtraDigit, PopCount((uint)ExtraDigitsMask) * .1M) };

	/// <inheritdoc/>
	public override int Type => 3;

	[ResourceTextFormatter]
	private partial string ExtraCellsStr() => ExtraCells.ToString();

	[ResourceTextFormatter]
	private partial string ExtraDigitStr() => DigitMaskFormatter.Format(ExtraDigitsMask, FormattingMode.Normal);

	[ResourceTextFormatter]
	private partial string SubsetName() => R[$"SubsetNamesSize{ExtraCells.Count + 1}"]!;
}
