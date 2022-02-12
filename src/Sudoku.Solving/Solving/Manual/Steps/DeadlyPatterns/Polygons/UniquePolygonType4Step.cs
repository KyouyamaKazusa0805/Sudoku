using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Polygon Type 4</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Map"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="ConjugateRegion">Indicates the cells that forms the conjugate region.</param>
/// <param name="ExtraMask">Indicates the extra digits mask.</param>
public sealed record UniquePolygonType4Step(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	in Cells Map,
	short DigitsMask,
	in Cells ConjugateRegion,
	short ExtraMask
) : UniquePolygonStep(Conclusions, Views, Map, DigitsMask)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 5.5M;

	/// <inheritdoc/>
	public override int Type => 4;

	[FormatItem]
	internal string ExtraCombStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(ExtraMask).ToString();
	}

	[FormatItem]
	internal string ConjRegionStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ConjugateRegion.ToString();
	}
}
