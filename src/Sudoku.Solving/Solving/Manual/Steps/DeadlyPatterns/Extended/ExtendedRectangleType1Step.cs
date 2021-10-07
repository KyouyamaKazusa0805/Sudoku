namespace Sudoku.Solving.Manual.Steps.DeadlyPatterns.Extended;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle Type 1</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
public sealed record ExtendedRectangleType1Step(
	in ImmutableArray<Conclusion> Conclusions,
	in ImmutableArray<PresentationData> Views,
	in Cells Cells,
	short DigitsMask
) : ExtendedRectangleStep(Conclusions, Views, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override int Type => 1;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.XrType1;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;
}
