namespace Sudoku.Solving.Manual.Steps.DeadlyPatterns.Rectangles;

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
/// of regions <see cref="Block"/> and <see cref="Line"/>
/// </param>
/// <param name="IsCannibalistic">Indicates whether the Sue de Coq structure is a cannibalism.</param>
/// <param name="IsolatedDigitsMask">Indicates the mask that contains all isolated digtis.</param>
/// <param name="BlockCells">Indicates the cells in the block of the Sue de Coq structure.</param>
/// <param name="LineCells">Indicates the cells in the line of the Sue de Coq structure.</param>
/// <param name="IntersectionCells">
/// Indicates the cells in the intersection from regions <see cref="Block"/> and <see cref="Line"/>.
/// </param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
public sealed record UniqueRectangleWithSueDeCoqStep(
	in ImmutableArray<Conclusion> Conclusions,
	in ImmutableArray<PresentationData> Views,
	int Digit1,
	int Digit2,
	in Cells Cells,
	bool IsAvoidable,
	int Block,
	int Line,
	short BlockMask,
	short LineMask,
	short IntersectionMask,
	bool IsCannibalistic,
	short IsolatedDigitsMask,
	in Cells BlockCells,
	in Cells LineCells,
	in Cells IntersectionCells,
	int AbsoluteOffset
) : UniqueRectangleStep(
	Conclusions, Views, IsAvoidable ? Technique.ArSdc : Technique.UrSdc,
	Digit1, Digit2, Cells, IsAvoidable, AbsoluteOffset
)
{
	/// <inheritdoc/>
	public override decimal Difficulty =>
		5.0M // Base difficulty.
		+ (LineCells | BlockCells).Count * .1M // Sue de Coq base difficulty.
		+ (!IsCannibalistic && IsolatedDigitsMask != 0 ? .1M : 0) // Isolated difficulty.
		+ (IsCannibalistic ? .1M : 0) // Cannibalism difficulty.
		+ (IsAvoidable ? .1M : 0); // Avoidable Rectangle difficulty.

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.UrPlus;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.HardlyEver;

	[FormatItem]
	private string MergedCellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (LineCells | BlockCells).ToString();
	}

	[FormatItem]
	private string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection((short)(LineMask | BlockMask)).ToString();
	}
}
