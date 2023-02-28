namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Reverse Unique Rectangle</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
internal abstract record ReverseUniqueRectangleStep(ConclusionList Conclusions, ViewList Views, scoped in CellMap Cells, short DigitsMask) :
	ReverseBivalueUniversalGraveStep(Conclusions, Views, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 6.0M;

	/// <inheritdoc/>
	public sealed override Technique TechniqueCode => Enum.Parse<Technique>($"ReverseUniqueRectangleType{Type}");
}
