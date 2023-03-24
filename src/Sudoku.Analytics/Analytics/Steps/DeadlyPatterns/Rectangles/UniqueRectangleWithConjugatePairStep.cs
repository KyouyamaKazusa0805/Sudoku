namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle with Conjugate Pair(s)</b> technique.
/// </summary>
public class UniqueRectangleWithConjugatePairStep(
	Conclusion[] conclusions,
	View[]? views,
	Technique code,
	int digit1,
	int digit2,
	scoped in CellMap cells,
	bool isAvoidable,
	Conjugate[] conjugatePairs,
	int absoluteOffset
) : UniqueRectangleStep(conclusions, views, code, digit1, digit2, cells, isAvoidable, absoluteOffset)
{
	/// <inheritdoc/>
	public sealed override decimal BaseDifficulty => 4.4M;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public sealed override TechniqueGroup Group => TechniqueGroup.UniqueRectanglePlus;

	/// <summary>
	/// Indicates the conjugate pairs used.
	/// </summary>
	public Conjugate[] ConjugatePairs { get; } = conjugatePairs;

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
