using Sudoku.Analytics.Categorization;
using Sudoku.Rendering;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bivalue Universal Grave Type 1</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
public sealed class BivalueUniversalGraveType1Step(Conclusion[] conclusions, View[]? views) : BivalueUniversalGraveStep(conclusions, views)
{
	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGraveType1;
}
