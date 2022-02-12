using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Polygon Type 3</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Map"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="ExtraCells">Indicates the extra cells used for forming the subset.</param>
/// <param name="ExtraDigitsMask">Indicates the extra digits used for forming the subset.</param>
public sealed record UniquePolygonType3Step(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	in Cells Map,
	short DigitsMask,
	in Cells ExtraCells,
	short ExtraDigitsMask
) : UniquePolygonStep(Conclusions, Views, Map, DigitsMask)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 5.2M + ExtraCells.Count * .1M;

	/// <inheritdoc/>
	public override int Type => 3;

	[FormatItem]
	internal string ExtraDigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(ExtraDigitsMask).ToString();
	}

	[FormatItem]
	internal string ExtraCellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ExtraCells.ToString();
	}
}
