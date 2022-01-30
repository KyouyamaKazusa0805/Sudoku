using Sudoku.Collections;
using Sudoku.Data;
using Sudoku.Presentation;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps.DeadlyPatterns.Polygons;

/// <summary>
/// Provides with a step that is a <b>Unique Polygon Type 1</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Map"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
public sealed record UniquePolygonType1Step(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	in Cells Map,
	short DigitsMask
) : UniquePolygonStep(Conclusions, Views, Map, DigitsMask)
{
	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.UniquePolygonType1;
}
