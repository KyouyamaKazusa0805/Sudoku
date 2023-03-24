namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Loop Type 2</b> technique.
/// </summary>
public sealed class UniqueLoopType2Step(Conclusion[] conclusions, View[]? views, int digit1, int digit2, scoped in CellMap loop, int extraDigit) :
	UniqueLoopStep(conclusions, views, digit1, digit2, loop)
{
	/// <inheritdoc/>
	public override int Type => 2;

	/// <summary>
	/// Indicates the extra digit used.
	/// </summary>
	public int ExtraDigit { get; } = extraDigit;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .1M;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { Digit1Str, Digit2Str, LoopStr, ExtraDigitStr } },
			{ "zh", new[] { Digit1Str, Digit2Str, LoopStr, ExtraDigitStr } }
		};

	private string ExtraDigitStr => (ExtraDigit + 1).ToString();
}
