namespace Sudoku.Solving.Logics.Implementations.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Polygon Type 4</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Map"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="ConjugateHouse">Indicates the cells that forms the conjugate house.</param>
/// <param name="ExtraMask">Indicates the extra digits mask.</param>
internal sealed record UniquePolygonType4Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Map,
	short DigitsMask,
	scoped in CellMap ConjugateHouse,
	short ExtraMask
) : UniquePolygonStep(Conclusions, Views, Map, DigitsMask)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 5.5M;

	/// <inheritdoc/>
	public override int Type => 4;

	[ResourceTextFormatter]
	internal string ExtraCombStr() => DigitMaskFormatter.Format(ExtraMask, FormattingMode.Normal);

	[ResourceTextFormatter]
	internal string ConjHouseStr() => ConjugateHouse.ToString();
}
