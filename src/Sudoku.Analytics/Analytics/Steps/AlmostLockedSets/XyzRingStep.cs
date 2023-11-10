using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
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
/// <param name="conjugateHouse">Indicates the conjugate house used.</param>
/// <param name="isType2">Indicates whether the type is type 2.</param>
/// <param name="isGrouped">Indicates whether the conjugate pair is grouped one.</param>
public sealed partial class XyzRingStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[DataMember] Cell pivot,
	[DataMember] Cell leafCell1,
	[DataMember] Cell leafCell2,
	[DataMember] House conjugateHouse,
	[DataMember] bool isType2,
	[DataMember] bool isGrouped
) : AlmostLockedSetsStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => IsType2 ? 5.0M : 5.2M;

	/// <inheritdoc/>
	public override Technique Code
		=> (IsGrouped, IsType2) switch
		{
			(true, true) => Technique.GroupedXyzRingType2,
			(true, _) => Technique.GroupedXyzRingType1,
			(_, true) => Technique.XyzRingType2,
			_ => Technique.XyzRingType1
		};
}
