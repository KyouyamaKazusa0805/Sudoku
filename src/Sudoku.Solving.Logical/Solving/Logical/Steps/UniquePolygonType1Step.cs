﻿namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Polygon Type 1</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Map"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
internal sealed record UniquePolygonType1Step(ConclusionList Conclusions, ViewList Views, scoped in CellMap Map, short DigitsMask) :
	UniquePolygonStep(Conclusions, Views, Map, DigitsMask)
{
	/// <inheritdoc/>
	public override int Type => 1;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { DigitsStr, CellsStr } }, { "zh", new[] { DigitsStr, CellsStr } } };
}
