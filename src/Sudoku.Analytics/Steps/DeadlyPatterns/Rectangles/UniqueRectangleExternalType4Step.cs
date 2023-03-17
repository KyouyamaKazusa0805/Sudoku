namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle External Type 4</b> technique.
/// </summary>
public sealed class UniqueRectangleExternalType4Step(
	Conclusion[] conclusions,
	View[]? views,
	int digit1,
	int digit2,
	scoped in CellMap cells,
	scoped in CellMap guardianCells,
	Conjugate conjugatePair,
	bool isIncomplete,
	bool isAvoidable,
	int absoluteOffset
) : UniqueRectangleStep(
	conclusions,
	views,
	isAvoidable ? Technique.AvoidableRectangleExternalType4 : Technique.UniqueRectangleExternalType4,
	digit1,
	digit2,
	cells,
	isAvoidable,
	absoluteOffset
)
{
	/// <summary>
	/// Indicates whether the rectangle is incomplete.
	/// </summary>
	public bool IsIncomplete { get; } = isIncomplete;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .2M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override TechniqueGroup Group => TechniqueGroup.UniqueRectanglePlus;

	/// <summary>
	/// Indicates the conjugate pair used.
	/// </summary>
	public Conjugate ConjugatePair { get; } = conjugatePair;

	/// <summary>
	/// Indicates the cells that the guardians lie in.
	/// </summary>
	public CellMap GuardianCells { get; } = guardianCells;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[]
		{
			new(ExtraDifficultyCaseNames.Avoidable, IsAvoidable ? .1M : 0),
			new(ExtraDifficultyCaseNames.Incompleteness, IsIncomplete ? .1M : 0)
		};

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { D1Str, D2Str, CellsStr, ConjugatePairStr } },
			{ "zh", new[] { D1Str, D2Str, CellsStr, ConjugatePairStr } }
		};

	private string ConjugatePairStr => ConjugatePair.ToString();
}
