namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is an <b>Empty Rectangle Intersection Pair</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="StartCell">Indicates the start cell used.</param>
/// <param name="EndCell">Indicates the end cell used.</param>
/// <param name="House">The house that forms the dual empty rectangle.</param>
/// <param name="Digit1">Indicates the digit 1 used in this pattern.</param>
/// <param name="Digit2">Indicates the digit 2 used in this pattern.</param>
internal sealed partial record EmptyRectangleIntersectionPairStep(
	ConclusionList Conclusions,
	ViewList Views,
	int StartCell,
	int EndCell,
	int House,
	int Digit1,
	int Digit2
) : AlmostLockedSetsStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 6.0M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.EmptyRectangleIntersectionPair;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.EmptyRectangleIntersectionPair;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	[ResourceTextFormatter]
	private partial string Digit1Str() => (Digit1 + 1).ToString();

	[ResourceTextFormatter]
	private partial string Digit2Str() => (Digit2 + 1).ToString();

	[ResourceTextFormatter]
	private partial string StartCellStr() => RxCyNotation.ToCellString(StartCell);

	[ResourceTextFormatter]
	private partial string EndCellStr() => RxCyNotation.ToCellString(EndCell);

	[ResourceTextFormatter]
	private partial string HouseStr() => HouseFormatter.Format(1 << House);
}
