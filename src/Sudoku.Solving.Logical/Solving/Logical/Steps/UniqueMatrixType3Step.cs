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
internal sealed record UniqueMatrixType3Step(
	Conclusion[] Conclusions,
	View[]? Views,
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

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitsStr, CellsStr, ExtraDigitStr, ExtraCellsStr, SubsetName } },
			{ "zh", new[] { ExtraDigitStr, ExtraCellsStr, SubsetName, DigitsStr, CellsStr } }
		};

	private string ExtraCellsStr => ExtraCells.ToString();

	private string ExtraDigitStr => DigitMaskFormatter.Format(ExtraDigitsMask, FormattingMode.Normal);

	private string SubsetName => R[$"SubsetNamesSize{ExtraCells.Count + 1}"]!;
}
