namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle with Wing</b> technique.
/// </summary>
public sealed class UniqueRectangleWithWingStep(
	Conclusion[] conclusions,
	View[]? views,
	Technique code,
	int digit1,
	int digit2,
	scoped in CellMap cells,
	bool isAvoidable,
	scoped in CellMap branches,
	scoped in CellMap petals,
	Mask extraDigitsMask,
	int absoluteOffset
) : UniqueRectangleStep(
	conclusions,
	views,
	code,
	digit1,
	digit2,
	cells,
	isAvoidable,
	absoluteOffset
)
{
	/// <summary>
	/// Indicates the mask that contains all extra digits.
	/// </summary>
	public Mask ExtraDigitsMask { get; } = extraDigitsMask;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <summary>
	/// Indicates the branches used.
	/// </summary>
	public CellMap Branches { get; } = branches;

	/// <summary>
	/// Indicates the petals used.
	/// </summary>
	public CellMap Petals { get; } = petals;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[]
		{
			new(ExtraDifficultyCaseNames.Avoidable, IsAvoidable ? .1M : 0),
			new(
				ExtraDifficultyCaseNames.WingSize,
				Code switch
				{
					Technique.UniqueRectangleXyWing or Technique.AvoidableRectangleXyWing => .2M,
					Technique.UniqueRectangleXyzWing or Technique.AvoidableRectangleXyzWing => .3M,
					Technique.UniqueRectangleWxyzWing or Technique.AvoidableRectangleWxyzWing => .5M
				}
			)
		};

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { D1Str, D2Str, CellsStr, BranchesStr, DigitsStr } },
			{ "zh", new[] { D1Str, D2Str, CellsStr, BranchesStr, DigitsStr } }
		};

	private string BranchesStr => Branches.ToString();

	private string DigitsStr => DigitMaskFormatter.Format(ExtraDigitsMask, FormattingMode.Normal);
}
