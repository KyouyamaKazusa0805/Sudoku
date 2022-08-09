namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle with Sue de Coq</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="IsAvoidable"><inheritdoc/></param>
/// <param name="Block">Indicates the block that the Sue de Coq structure used.</param>
/// <param name="Line">Indicates the line that the Sue de Coq structure used.</param>
/// <param name="BlockMask">
/// Indicates the mask that contains all digits from the block of the Sue de Coq structure.
/// </param>
/// <param name="LineMask">
/// Indicates the mask that contains all digits from the line of the Sue de Coq structure.
/// </param>
/// <param name="IntersectionMask">
/// Indicates the mask that contains all digits from the intersection
/// of houses <see cref="Block"/> and <see cref="Line"/>
/// </param>
/// <param name="IsCannibalistic">Indicates whether the Sue de Coq structure is a cannibalism.</param>
/// <param name="IsolatedDigitsMask">Indicates the mask that contains all isolated digits.</param>
/// <param name="BlockCells">Indicates the cells in the block of the Sue de Coq structure.</param>
/// <param name="LineCells">Indicates the cells in the line of the Sue de Coq structure.</param>
/// <param name="IntersectionCells">
/// Indicates the cells in the intersection from houses <see cref="Block"/> and <see cref="Line"/>.
/// </param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
internal sealed record UniqueRectangleWithSueDeCoqStep(
	ConclusionList Conclusions,
	ViewList Views,
	int Digit1,
	int Digit2,
	scoped in Cells Cells,
	bool IsAvoidable,
	int Block,
	int Line,
	short BlockMask,
	short LineMask,
	short IntersectionMask,
	bool IsCannibalistic,
	short IsolatedDigitsMask,
	scoped in Cells BlockCells,
	scoped in Cells LineCells,
	scoped in Cells IntersectionCells,
	int AbsoluteOffset
) :
	UniqueRectangleStep(
		Conclusions,
		Views,
		IsAvoidable ? Technique.AvoidableRectangleSueDeCoq : Technique.UniqueRectangleSueDeCoq,
		Digit1,
		Digit2,
		Cells,
		IsAvoidable,
		AbsoluteOffset
	),
	IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => 5.0M;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[]
		{
			("Sue de Coq base", (LineCells | BlockCells).Count * .1M),
			("Isolated", !IsCannibalistic && IsolatedDigitsMask != 0 ? .1M : 0),
			("Cannibalism", IsCannibalistic ? .1M : 0),
			("Avoidable rectangle", IsAvoidable ? .1M : 0)
		};

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.UniqueRectanglePlus;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.HardlyEver;

	[FormatItem]
	internal string MergedCellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (LineCells | BlockCells).ToString();
	}

	[FormatItem]
	internal string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => DigitMaskFormatter.Format((short)(LineMask | BlockMask), FormattingMode.Normal);
	}
}
