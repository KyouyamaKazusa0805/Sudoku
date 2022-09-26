namespace Sudoku.Solving.Logical.Implementations.Steps;

/// <summary>
/// Provides with a step that is a <b>Hidden Subset</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="House"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
internal sealed record HiddenSubsetStep(
	ConclusionList Conclusions,
	ViewList Views,
	int House,
	scoped in CellMap Cells,
	short DigitsMask
) : SubsetStep(Conclusions, Views, House, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override decimal Difficulty => Size switch { 2 => 3.4M, 3 => 4.0M, 4 => 5.4M };

	/// <inheritdoc/>
	public override Technique TechniqueCode
		=> Size switch { 2 => Technique.HiddenPair, 3 => Technique.HiddenTriple, 4 => Technique.HiddenQuadruple };

	[ResourceTextFormatter]
	internal string DigitStr() => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	[ResourceTextFormatter]
	internal string HouseStr() => HouseFormatter.Format(1 << House);
}
