namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Square Type 4</b> technique.
/// </summary>
public sealed class UniqueMatrixType4Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in CellMap cells,
	short digitsMask,
	int digit1,
	int digit2,
	scoped in CellMap conjugateHouse
) : UniqueMatrixStep(conclusions, views, cells, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 4;

	/// <summary>
	/// Indicates the first digit used in the conjugate.
	/// </summary>
	public int Digit1 { get; } = digit1;

	/// <summary>
	/// Indicates the second digit used in the conjugate.
	/// </summary>
	public int Digit2 { get; } = digit2;

	/// <summary>
	/// Indicates the cells that describes the generalized conjugate pair.
	/// </summary>
	public CellMap ConjugateHouse { get; } = conjugateHouse;

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
