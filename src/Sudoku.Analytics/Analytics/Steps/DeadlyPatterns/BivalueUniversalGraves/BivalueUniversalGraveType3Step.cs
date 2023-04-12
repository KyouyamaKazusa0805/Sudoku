namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave Type 3</b> technique.
/// </summary>
public sealed class BivalueUniversalGraveType3Step(
	Conclusion[] conclusions,
	View[]? views,
	IReadOnlyList<int> trueCandidates,
	Mask subsetDigitsMask,
	scoped in CellMap cells,
	bool isNaked
) : BivalueUniversalGraveStep(conclusions, views)
{
	/// <summary>
	/// Indicates whether the subset is naked.
	/// </summary>
	public bool IsNaked { get; } = isNaked;

	/// <summary>
	/// Indicates the mask of subset digits.
	/// </summary>
	public Mask SubsetDigitsMask { get; } = subsetDigitsMask;

	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGraveType3;

	/// <summary>
	/// Indicates the subset cells used.
	/// </summary>
	public CellMap Cells { get; } = cells;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[]
		{
			new(ExtraDifficultyCaseNames.Size, Size * .1M),
			new(ExtraDifficultyCaseNames.Hidden, isNaked ? 0 : .1M)
		};

	/// <summary>
	/// Indicates the true candidates used.
	/// </summary>
	public IReadOnlyList<int> TrueCandidates { get; } = trueCandidates;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { TrueCandidatesStr, SubsetTypeStr, SizeStr, ExtraDigitsStr, CellsStr } },
			{ "zh", new[] { TrueCandidatesStr, SubsetTypeStr, SizeStr, CellsStr, ExtraDigitsStr } }
		};

	/// <summary>
	/// Indicates the size of the subset.
	/// </summary>
	private int Size => PopCount((uint)SubsetDigitsMask);

	private string TrueCandidatesStr => (CandidateMap.Empty + TrueCandidates).ToString();

	private string SubsetTypeStr => R[IsNaked ? "NakedKeyword" : "HiddenKeyword"]!;

	private string SizeStr => R[$"SubsetNamesSize{Size}"]!;

	private string ExtraDigitsStr => DigitMaskFormatter.Format(SubsetDigitsMask, FormattingMode.Normal);

	private string CellsStr => Cells.ToString();
}
