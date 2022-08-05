namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave Type 3</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="TrueCandidates">Indicates the true candidates.</param>
/// <param name="DigitsMask">Indicates the digits used.</param>
/// <param name="Cells">Indicates the cells used.</param>
/// <param name="IsNaked">Indicates whether the specified subset is naked subset.</param>
internal sealed record BivalueUniversalGraveType3Step(
	ConclusionList Conclusions,
	ViewList Views,
	IReadOnlyList<int> TrueCandidates,
	short DigitsMask,
	scoped in Cells Cells,
	bool IsNaked
) : BivalueUniversalGraveStep(Conclusions, Views), IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => base.Difficulty;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[] { ("Subset", Size * .1M), ("Hidden subset", IsNaked ? 0 : .1M) };

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueUniversalGraveType3;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	/// <summary>
	/// Indicates the size of the subset.
	/// </summary>
	private int Size => PopCount((uint)DigitsMask);

	[FormatItem]
	internal string TrueCandidatesStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Candidates(TrueCandidates).ToString();
	}

	[FormatItem]
	internal string SubsetTypeStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => R[IsNaked ? "NakedKeyword" : "HiddenKeyword"]!;
	}

	[FormatItem]
	internal string SizeStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => SubsetNames[Size].ToLower(null);
	}

	[FormatItem]
	internal string SizeStrZhCn
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => R[$"SubsetNames{Size}"]!;
	}

	[FormatItem]
	internal string ExtraDigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(DigitsMask).ToString();
	}

	[FormatItem]
	internal string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Cells.ToString();
	}
}
