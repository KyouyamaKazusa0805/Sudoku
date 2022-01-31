using Sudoku.Data;
using Sudoku.Presentation;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Bivalue Universal Grave Type 1</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
public sealed record BivalueUniversalGraveType1Step(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views
) : BivalueUniversalGraveStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueUniversalGraveType1;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;
}
