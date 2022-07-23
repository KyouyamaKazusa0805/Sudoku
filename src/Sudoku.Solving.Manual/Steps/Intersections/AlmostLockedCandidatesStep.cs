namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is an <b>Almost Locked Candidates</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="DigitsMask">Indicates the mask that contains the digits used.</param>
/// <param name="BaseCells">Indicates the base cells.</param>
/// <param name="TargetCells">Indicates the target cells.</param>
/// <param name="HasValueCell">Indicates whether the step contains value cells.</param>
public sealed record AlmostLockedCandidatesStep(
	ConclusionList Conclusions,
	ViewList Views,
	short DigitsMask,
	scoped in Cells BaseCells,
	scoped in Cells TargetCells,
	bool HasValueCell
) :
	IntersectionStep(Conclusions, Views),
	IStepWithSize,
	IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public int Size => PopCount((uint)DigitsMask);

	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => Size switch { 2 => 4.5M, 3 => 5.2M, 4 => 5.7M };

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[] { ("Extra", HasValueCell ? Size switch { 2 or 3 => .1M, 4 => .2M } : 0) };

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override Technique TechniqueCode => (Technique)((int)Technique.AlmostLockedPair + Size - 2);

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.AlmostLockedCandidates;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	[FormatItem]
	internal string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(DigitsMask).ToString();
	}

	[FormatItem]
	internal string BaseCellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => BaseCells.ToString();
	}

	[FormatItem]
	internal string TargetCellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => TargetCells.ToString();
	}
}
