namespace Sudoku.Solving.Manual.Steps.DeadlyPatterns.Rectangles;

/// <summary>
/// Provides with a step that is a <b>Avoidable Rectangle with Hidden Single</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="BaseCell"></param>
/// <param name="TargetCell"></param>
/// <param name="Region"></param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
public sealed record AvoidableRectangleWithHiddenSingleStep(
	in ImmutableArray<Conclusion> Conclusions,
	in ImmutableArray<PresentationData> Views,
	int Digit1,
	int Digit2,
	in Cells Cells,
	int BaseCell,
	int TargetCell,
	int Region,
	int AbsoluteOffset
) : UniqueRectangleStep(
	Conclusions, Views, Region switch
	{
		>= 0 and < 9 => Technique.ArHiddenSingleBlock,
		>= 9 and < 18 => Technique.ArHiddenSingleRow,
		>= 18 and < 27 => Technique.ArHiddenSingleColumn
	}, Digit1, Digit2, Cells, true, AbsoluteOffset
)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 4.7M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.UrPlus;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	[FormatItem]
	private string BaseCellStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Cells { BaseCell }.ToString();
	}

	[FormatItem]
	private string TargetCellStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Cells { TargetCell }.ToString();
	}

	[FormatItem]
	private string RegionStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new RegionCollection(Region).ToString();
	}

	[FormatItem]
	private string Digit1Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit1 + 1).ToString();
	}
}
