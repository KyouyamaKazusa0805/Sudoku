using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Rendering;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Complex Death Blossom</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
public sealed class ComplexDeathBlossomStep(Conclusion[] conclusions, View[]? views, StepSearcherOptions options) :
	AlmostLockedSetsStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 8.7M;

	/// <inheritdoc/>
	public override Technique Code => Technique.ComplexDeathBlossom;
}
