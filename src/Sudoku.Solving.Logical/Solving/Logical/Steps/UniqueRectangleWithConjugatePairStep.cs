namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle with Conjugate Pair(s)</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="TechniqueCode2"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="IsAvoidable"><inheritdoc/></param>
/// <param name="ConjugatePairs">Indicates the conjugate pairs used.</param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
internal record UniqueRectangleWithConjugatePairStep(
	ConclusionList Conclusions,
	ViewList Views,
	Technique TechniqueCode2,
	int Digit1,
	int Digit2,
	scoped in CellMap Cells,
	bool IsAvoidable,
	Conjugate[] ConjugatePairs,
	int AbsoluteOffset
) : UniqueRectangleStep(
	Conclusions,
	Views,
	TechniqueCode2,
	Digit1,
	Digit2,
	Cells,
	IsAvoidable,
	AbsoluteOffset
)
{
	/// <inheritdoc/>
	public sealed override decimal BaseDifficulty => 4.4M;

	/// <inheritdoc/>
	public override string? Format => base.Format;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.UniqueRectanglePlus;

	/// <inheritdoc/>
	public sealed override Rarity Rarity => Rarity.Often;

	/// <inheritdoc/>
	public sealed override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.ConjugatePair, ConjugatePairs.Length * .2M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { D1Str, D2Str, CellsStr, Prefix, Suffix, ConjPairsStr } },
			{ "zh", new[] { D1Str, D2Str, CellsStr, ConjPairsStr } }
		};

	private string ConjPairsStr
	{
		get
		{
			const string separator = ", ";

			scoped var sb = new StringHandler(100);
			sb.AppendRangeWithSeparator(ConjugatePairs, separator);

			return sb.ToStringAndClear();
		}	
	}

	private string Prefix => ConjugatePairs.Length == 1 ? "a " : string.Empty;

	private string Suffix => ConjugatePairs.Length == 1 ? string.Empty : "s";
}
