namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Loop Type 4</b> technique.
/// </summary>
public sealed class UniqueLoopType4Step(
	Conclusion[] conclusions,
	View[]? views,
	int digit1,
	int digit2,
	scoped in CellMap loop,
	scoped in Conjugate conjugatePair
) : UniqueLoopStep(conclusions, views, digit1, digit2, loop)
{
	/// <inheritdoc/>
	public override int Type => 4;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .1M;

	/// <summary>
	/// Indicates the conjugate pair used.
	/// </summary>
	public Conjugate ConjugatePair { get; } = conjugatePair;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { Digit1Str, Digit2Str, LoopStr, ConjStr } },
			{ "zh", new[] { Digit1Str, Digit2Str, LoopStr, ConjStr } }
		};

	private string ConjStr => ConjugatePair.ToString();
}
