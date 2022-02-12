using Sudoku.Collections;
using Sudoku.Presentation;

namespace Sudoku.Solving.Manual.Steps;

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
	public override int Type => 1;
}
