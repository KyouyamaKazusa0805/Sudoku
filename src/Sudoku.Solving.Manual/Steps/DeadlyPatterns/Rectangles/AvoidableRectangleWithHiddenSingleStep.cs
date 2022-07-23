namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is an <b>Avoidable Rectangle with Hidden Single</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="BaseCell"></param>
/// <param name="TargetCell"></param>
/// <param name="House"></param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
public sealed record AvoidableRectangleWithHiddenSingleStep(
	ConclusionList Conclusions,
	ViewList Views,
	int Digit1,
	int Digit2,
	scoped in Cells Cells,
	int BaseCell,
	int TargetCell,
	int House,
	int AbsoluteOffset
) : UniqueRectangleStep(
	Conclusions,
	Views,
	(Technique)((int)Technique.AvoidableRectangleHiddenSingleBlock + (int)House.ToHouse()),
	Digit1,
	Digit2,
	Cells,
	true,
	AbsoluteOffset
)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 4.7M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.UniqueRectanglePlus;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	[FormatItem]
	internal string BaseCellStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Cells.Empty + BaseCell).ToString();
	}

	[FormatItem]
	internal string TargetCellStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Cells.Empty + TargetCell).ToString();
	}

	[FormatItem]
	internal string HouseStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new HouseCollection(House).ToString();
	}

	[FormatItem]
	internal string Digit1Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit1 + 1).ToString();
	}
}
