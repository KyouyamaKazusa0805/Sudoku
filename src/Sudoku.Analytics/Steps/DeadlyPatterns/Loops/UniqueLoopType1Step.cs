namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Loop Type 1</b> technique.
/// </summary>
public sealed class UniqueLoopType1Step(Conclusion[] conclusions, View[]? views, int digit1, int digit2, scoped in CellMap loop) :
	UniqueLoopStep(conclusions, views, digit1, digit2, loop)
{
	/// <inheritdoc/>
	public override int Type => 1;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { Digit1Str, Digit2Str, LoopStr } },
			{ "zh", new[] { Digit1Str, Digit2Str, LoopStr } }
		};
}
