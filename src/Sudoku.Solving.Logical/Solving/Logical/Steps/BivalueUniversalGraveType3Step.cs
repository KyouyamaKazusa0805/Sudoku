namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave Type 3</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="TrueCandidates">Indicates the true candidates.</param>
/// <param name="DigitsMask">Indicates the digits used.</param>
/// <param name="Cells">Indicates the cells used.</param>
/// <param name="IsNaked">Indicates whether the specified subset is naked subset.</param>
internal sealed record BivalueUniversalGraveType3Step(
	ConclusionList Conclusions,
	ViewList Views,
	IReadOnlyList<int> TrueCandidates,
	short DigitsMask,
	scoped in CellMap Cells,
	bool IsNaked
) : BivalueUniversalGraveStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueUniversalGraveType3;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[]
		{
			new(ExtraDifficultyCaseNames.Size, Size * .1M),
			new(ExtraDifficultyCaseNames.Hidden, IsNaked ? 0 : .1M)
		};

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?>? FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { TrueCandidatesStr, SubsetTypeStr, SizeStr, ExtraDigitsStr, CellsStr } },
			{ "zh", new[] { TrueCandidatesStr, SubsetTypeStr, SizeStr, CellsStr, ExtraDigitsStr } }
		};

	/// <summary>
	/// Indicates the size of the subset.
	/// </summary>
	private int Size => PopCount((uint)DigitsMask);

	private string TrueCandidatesStr => (CandidateMap.Empty + TrueCandidates).ToString();

	private string SubsetTypeStr => R[IsNaked ? "NakedKeyword" : "HiddenKeyword"]!;

	private string SizeStr => R[$"SubsetNamesSize{Size}"]!;

	private string ExtraDigitsStr => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	private string CellsStr => Cells.ToString();
}
