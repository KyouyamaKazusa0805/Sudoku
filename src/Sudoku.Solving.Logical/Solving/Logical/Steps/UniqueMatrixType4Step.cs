namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Square Type 4</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="Digit1">Indicates the digit 1 used.</param>
/// <param name="Digit2">Indicates the digit 2 used.</param>
/// <param name="ConjugateHouse">Indicates the cells used as the conjugate house.</param>
internal sealed record UniqueMatrixType4Step(
	Conclusion[] Conclusions,
	View[]? Views,
	scoped in CellMap Cells,
	short DigitsMask,
	int Digit1,
	int Digit2,
	scoped in CellMap ConjugateHouse
) : UniqueMatrixStep(Conclusions, Views, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override int Type => 4;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitsStr, CellsStr, ConjStr, Digit1Str, Digit2Str } },
			{ "zh", new[] { ConjStr, Digit1Str, Digit2Str, DigitsStr, CellsStr } }
		};

	private string ConjStr => ConjugateHouse.ToString();

	private string Digit1Str => (Digit1 + 1).ToString();

	private string Digit2Str => (Digit2 + 1).ToString();
}
