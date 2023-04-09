namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle</b> technique.
/// </summary>
public abstract class ExtendedRectangleStep(Conclusion[] conclusions, View[]? views, scoped in CellMap cells, short digitsMask) :
	DeadlyPatternStep(conclusions, views)
{
	/// <inheritdoc/>
	public sealed override decimal BaseDifficulty => 4.5M;

	/// <summary>
	/// Indicates the type of the step. The value must be between 1 and 4.
	/// </summary>
	public abstract int Type { get; }

	/// <summary>
	/// Indicates the mask of digits used.
	/// </summary>
	public short DigitsMask { get; } = digitsMask;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	public sealed override Technique Code => Enum.Parse<Technique>($"ExtendedRectangleType{Type}");

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <summary>
	/// Indicates the cells used in this pattern.
	/// </summary>
	public CellMap Cells { get; } = cells;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.Size, (A004526(Cells.Count) - 2) * .1M) };

	private protected string DigitsStr => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	private protected string CellsStr => Cells.ToString();
}
