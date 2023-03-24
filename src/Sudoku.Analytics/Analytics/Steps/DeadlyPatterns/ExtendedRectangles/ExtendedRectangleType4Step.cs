namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle Type 4</b> technique.
/// </summary>
public sealed class ExtendedRectangleType4Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in CellMap cells,
	short digitsMask,
	scoped in Conjugate conjugatePair
) : ExtendedRectangleStep(conclusions, views, cells, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 4;

	/// <summary>
	/// Indicates the conjugate pair used.
	/// </summary>
	public Conjugate ConjugatePair { get; } = conjugatePair;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new[] { base.ExtraDifficultyCases[0], new(ExtraDifficultyCaseNames.ConjugatePair, .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitsStr, CellsStr, ConjStr } },
			{ "zh", new[] { DigitsStr, CellsStr, ConjStr } }
		};

	private string ConjStr => ConjugatePair.ToString();
}
