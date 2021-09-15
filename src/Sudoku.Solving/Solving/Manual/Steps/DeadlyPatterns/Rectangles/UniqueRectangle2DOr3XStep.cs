namespace Sudoku.Solving.Manual.Steps.DeadlyPatterns.Rectangles;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle 2D (or 3X)</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="TechniqueCode2"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="IsAvoidable"><inheritdoc/></param>
/// <param name="XDigit">Indicates the digit X.</param>
/// <param name="YDigit">Indicates the digit Y.</param>
/// <param name="XyCell">Indicates the cell XY.</param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
public sealed record class UniqueRectangle2DOr3XStep(
	in ImmutableArray<Conclusion> Conclusions,
	in ImmutableArray<PresentationData> Views,
	Technique TechniqueCode2,
	int Digit1,
	int Digit2,
	in Cells Cells,
	bool IsAvoidable,
	int XDigit,
	int YDigit,
	int XyCell,
	int AbsoluteOffset
) : UniqueRectangleStep(Conclusions, Views, TechniqueCode2, Digit1, Digit2, Cells, IsAvoidable, AbsoluteOffset)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 4.7M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.UrPlus;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.HardlyEver;

	[FormatItem]
	private string XDigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (XDigit + 1).ToString();
	}

	[FormatItem]
	private string YDigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (YDigit + 1).ToString();
	}

	[FormatItem]
	private string XYCellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Cells { XyCell }.ToString();
	}
}
