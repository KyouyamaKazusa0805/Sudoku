namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Borescoper's Deadly Pattern Type 1</b> technique.
/// </summary>
public sealed class BorescoperDeadlyPatternType1Step(Conclusion[] conclusions, View[]? views, scoped in CellMap map, short digitsMask) :
	BorescoperDeadlyPatternStep(conclusions, views, map, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 1;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { DigitsStr, CellsStr } }, { "zh", new[] { DigitsStr, CellsStr } } };
}
