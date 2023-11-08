using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Concepts;
using Sudoku.Rendering;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>XYZ-Ring</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="pivot">Indicates the pivot cell.</param>
/// <param name="leafCell1">Indicates the leaf cell 1.</param>
/// <param name="leafCell2">Indicates the leaf cell 2.</param>
/// <param name="conjugatePair">Indicates the conjugate pair used.</param>
/// <param name="isType2">Indicates whether the type is type 2.</param>
public sealed partial class XyzRingStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[DataMember] Cell pivot,
	[DataMember] Cell leafCell1,
	[DataMember] Cell leafCell2,
	[DataMember] Conjugate conjugatePair,
	[DataMember] bool isType2
) : AlmostLockedSetsStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => IsType2 ? 5.0M : 5.2M;

	/// <inheritdoc/>
	public override Technique Code => IsType2 ? Technique.XyzRingType2 : Technique.XyzRingType1;
}
