namespace Sudoku.Solving.Implementations.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave Type 1</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
internal sealed record BivalueUniversalGraveType1Step(ConclusionList Conclusions, ViewList Views) :
	BivalueUniversalGraveStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueUniversalGraveType1;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;
}
