namespace Sudoku.Solving.Logical.Steps;

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
[StepDisplayingFeature(StepDisplayingFeature.DifficultyRatingNotStable | StepDisplayingFeature.ConstructedTechnique)]
internal sealed record UniqueRectangleWithSueDeCoqStep(
	ConclusionList Conclusions,
	ViewList Views,
	int Digit1,
	int Digit2,
	scoped in CellMap Cells,
	bool IsAvoidable,
	int Block,
	int Line,
	short BlockMask,
	short LineMask,
	short IntersectionMask,
	bool IsCannibalistic,
	short IsolatedDigitsMask,
	scoped in CellMap BlockCells,
	scoped in CellMap LineCells,
	scoped in CellMap IntersectionCells,
	int AbsoluteOffset
) : UniqueRectangleStep(
	Conclusions,
	Views,
	IsAvoidable ? Technique.AvoidableRectangleSueDeCoq : Technique.UniqueRectangleSueDeCoq,
	Digit1,
	Digit2,
	Cells,
	IsAvoidable,
	AbsoluteOffset
)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .5M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.UniqueRectanglePlus;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.HardlyEver;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[]
		{
			new(ExtraDifficultyCaseNames.Size, (LineCells | BlockCells).Count * .1M),
			new(ExtraDifficultyCaseNames.Isolated, !IsCannibalistic && IsolatedDigitsMask != 0 ? .1M : 0),
			new(ExtraDifficultyCaseNames.Cannibalism, IsCannibalistic ? .1M : 0),
			new(ExtraDifficultyCaseNames.Avoidable, IsAvoidable ? .1M : 0)
		};


	[ResourceTextFormatter]
	internal string MergedCellsStr() => (LineCells | BlockCells).ToString();

	[ResourceTextFormatter]
	internal string DigitsStr() => DigitMaskFormatter.Format((short)(LineMask | BlockMask), FormattingMode.Normal);
}
