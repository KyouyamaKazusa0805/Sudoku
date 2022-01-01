namespace Sudoku.Solving.Manual.Steps.DeadlyPatterns.Rectangles;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle with Unknown Covering</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="TargetCell">Indicates the target cell.</param>
/// <param name="ExtraDigit">Indicates the extra digit used.</param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
public sealed record UniqueRectangleWithUnknownCoveringStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	int Digit1,
	int Digit2,
	in Cells Cells,
	int TargetCell,
	int ExtraDigit,
	int AbsoluteOffset
) : UniqueRectangleStep(
	Conclusions, Views, Technique.UniqueRectangleUnknownCovering,
	Digit1, Digit2, Cells, false, AbsoluteOffset
)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 4.9M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.UniqueRectanglePlus;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	[FormatItem]
	private string TargetCellStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Cells { TargetCell }.ToString();
	}

	[FormatItem]
	private string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection((short)(1 << Digit1 | 1 << Digit2)).ToString(ResourceDocumentManager.Shared["orKeywordWithSpaces"]);
	}

	[FormatItem]
	private string ExtraDigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ExtraDigit + 1).ToString();
	}
}
