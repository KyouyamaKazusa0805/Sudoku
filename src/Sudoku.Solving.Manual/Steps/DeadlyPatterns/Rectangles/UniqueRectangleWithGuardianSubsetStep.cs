namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle with Guardians (External Subset)</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="GuardianCells">Indicates the cells that the guardians lie in.</param>
/// <param name="SubsetCells">The extra cells that forms the subset.</param>
/// <param name="SubsetDigitsMask">Indicates the digits that the subset are used.</param>
/// <param name="IsIncomplete">Indicates whether the rectangle is incomplete.</param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
public sealed record UniqueRectangleWithGuardianSubsetStep(
	ConclusionList Conclusions,
	ViewList Views,
	int Digit1,
	int Digit2,
	scoped in Cells Cells,
	scoped in Cells GuardianCells,
	scoped in Cells SubsetCells,
	short SubsetDigitsMask,
	bool IsIncomplete,
	int AbsoluteOffset
) :
	UniqueRectangleStep(
		Conclusions,
		Views,
		Technique.UniqueRectangleExternalType3,
		Digit1,
		Digit2,
		Cells,
		false,
		AbsoluteOffset
	),
	IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => 4.6M;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[]
		{
			("Digits", PopCount((uint)SubsetDigitsMask) * .1M),
			("Incompleteness", IsIncomplete ? .1M : 0)
		};

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.UniqueRectanglePlus;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.HardlyEver;

	[FormatItem]
	internal string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(SubsetDigitsMask).ToString();
	}

	[FormatItem]
	internal string SubsetCellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => SubsetCells.ToString();
	}
}
