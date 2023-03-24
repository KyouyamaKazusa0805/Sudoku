namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle Type 1</b> technique.
/// </summary>
public sealed class ExtendedRectangleType1Step(Conclusion[] conclusions, View[]? views, scoped in CellMap cells, short digitsMask) :
	ExtendedRectangleStep(conclusions, views, cells, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 1;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitsStr, CellsStr } },
			{ "zh", new[] { DigitsStr, CellsStr } }
		};
}
