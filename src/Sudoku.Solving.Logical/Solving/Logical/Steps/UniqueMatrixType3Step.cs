﻿namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Square Type 3</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="ExtraCells">Indicates the extra cells used.</param>
/// <param name="ExtraDigitsMask">
/// Indicates the extra digits that forms a subset with <paramref name="DigitsMask"/>.
/// </param>
[StepDisplayingFeature(StepDisplayingFeature.DifficultyRatingNotStable)]
internal sealed record UniqueMatrixType3Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Cells,
	short DigitsMask,
	short ExtraDigitsMask,
	scoped in CellMap ExtraCells
) : UniqueMatrixStep(Conclusions, Views, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override int Type => 3;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.ExtraDigit, PopCount((uint)ExtraDigitsMask) * .1M) };


	[ResourceTextFormatter]
	internal string ExtraCellsStr() => ExtraCells.ToString();

	[ResourceTextFormatter]
	internal string ExtraDigitStr() => DigitMaskFormatter.Format(ExtraDigitsMask, FormattingMode.Normal);

	[ResourceTextFormatter]
	internal string SubsetName() => R[$"SubsetNamesSize{ExtraCells.Count + 1}"]!;
}
