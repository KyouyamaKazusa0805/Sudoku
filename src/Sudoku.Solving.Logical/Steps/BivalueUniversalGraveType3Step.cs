namespace Sudoku.Solving.Logics.Implementations.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave Type 3</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="TrueCandidates">Indicates the true candidates.</param>
/// <param name="DigitsMask">Indicates the digits used.</param>
/// <param name="Cells">Indicates the cells used.</param>
/// <param name="IsNaked">Indicates whether the specified subset is naked subset.</param>
[StepDisplayingFeature(StepDisplayingFeature.DifficultyRatingNotStable)]
internal sealed record BivalueUniversalGraveType3Step(
	ConclusionList Conclusions,
	ViewList Views,
	IReadOnlyList<int> TrueCandidates,
	short DigitsMask,
	scoped in CellMap Cells,
	bool IsNaked
) : BivalueUniversalGraveStep(Conclusions, Views), IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => base.Difficulty;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[]
		{
			(PhasedDifficultyRatingKinds.Size, Size * .1M),
			(PhasedDifficultyRatingKinds.Hidden, IsNaked ? 0 : .1M)
		};

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueUniversalGraveType3;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	/// <summary>
	/// Indicates the size of the subset.
	/// </summary>
	private int Size => PopCount((uint)DigitsMask);

	[ResourceTextFormatter]
	internal string TrueCandidatesStr() => new Candidates(TrueCandidates).ToString();

	[ResourceTextFormatter]
	internal string SubsetTypeStr() => R[IsNaked ? "NakedKeyword" : "HiddenKeyword"]!;

	[ResourceTextFormatter]
	internal string SizeStr() => R[$"SubsetNames{Size}"]!;

	[ResourceTextFormatter]
	internal string SizeStrZhCn() => R[$"SubsetNames{Size}"]!;

	[ResourceTextFormatter]
	internal string ExtraDigitsStr() => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	[ResourceTextFormatter]
	internal string CellsStr() => Cells.ToString();
}
