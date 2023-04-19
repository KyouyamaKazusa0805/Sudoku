namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bivalue Oddagon Type 2</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="loopCells"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="extraDigit">Indicates the extra digit used.</param>
public sealed partial class BivalueOddagonType2Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in CellMap loopCells,
	int digit1,
	int digit2,
	[PrimaryConstructorParameter] int extraDigit
) : BivalueOddagonStep(conclusions, views, loopCells, digit1, digit2)
{
	/// <inheritdoc/>
	public override int Type => 2;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => new[] { (ExtraDifficultyCaseNames.ExtraDigit, .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { ExtraDigitStr, LoopStr } }, { "zh", new[] { LoopStr, ExtraDigitStr } } };

	private string ExtraDigitStr => (ExtraDigit + 1).ToString();
}
