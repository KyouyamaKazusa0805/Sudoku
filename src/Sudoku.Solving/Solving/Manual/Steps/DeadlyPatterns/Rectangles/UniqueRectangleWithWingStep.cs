namespace Sudoku.Solving.Manual.Steps.DeadlyPatterns.Rectangles;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle with Wing</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="TechniqueCode2"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="IsAvoidable"><inheritdoc/></param>
/// <param name="Pivots">Indictaes the pivots used.</param>
/// <param name="Petals">Indicates the petals used.</param>
/// <param name="ExtraDigitsMask">Indicates the mask that contains all extra digits.</param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
public sealed record class UniqueRectangleWithWingStep(
	in ImmutableArray<Conclusion> Conclusions,
	in ImmutableArray<PresentationData> Views,
	Technique TechniqueCode2,
	int Digit1,
	int Digit2,
	in Cells Cells,
	bool IsAvoidable,
	in Cells Pivots,
	in Cells Petals,
	short ExtraDigitsMask,
	int AbsoluteOffset
) : UniqueRectangleStep(Conclusions, Views, TechniqueCode2, Digit1, Digit2, Cells, IsAvoidable, AbsoluteOffset)
{
	/// <inheritdoc/>
	public override decimal Difficulty =>
		4.4M // Base difficulty.
		+ (IsAvoidable ? .1M : 0) // Avoidable difficulty.
		+ TechniqueCode switch
		{
			Technique.UrXyWing or Technique.ArXyWing => .2M,
			Technique.UrXyzWing or Technique.ArXyzWing => .3M,
			Technique.UrWxyzWing or Technique.ArWxyzWing => .5M
		}; // Wing difficulty.

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.UrPlus;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	[FormatItem]
	private string PivotsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Pivots.ToString();
	}

	[FormatItem]
	private string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(ExtraDigitsMask).ToString();
	}
}
