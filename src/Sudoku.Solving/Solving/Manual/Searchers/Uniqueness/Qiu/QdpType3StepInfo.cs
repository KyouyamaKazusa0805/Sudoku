namespace Sudoku.Solving.Manual.Uniqueness.Qiu;

/// <summary>
/// Provides a usage of <b>Qiu's deadly pattern type 3</b> (QDP) technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Pattern">The pattern.</param>
/// <param name="ExtraDigitsMask">The extra digits mask.</param>
/// <param name="ExtraCells">The extra cells.</param>
public sealed record QdpType3StepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Pattern Pattern,
	short ExtraDigitsMask, IReadOnlyList<int> ExtraCells
) : QdpStepInfo(Conclusions, Views, Pattern)
{
	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty + PopCount((uint)ExtraDigitsMask) * .1M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.QdpType3;

	[FormatItem]
	private string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(ExtraDigitsMask).ToString();
	}

	[FormatItem]
	private string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Cells(ExtraCells).ToString();
	}

	[FormatItem]
	private string SubsetName
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => TextResources.Current[$"SubsetNamesSize{(ExtraCells.Count + 1).ToString()}"];
	}
}
