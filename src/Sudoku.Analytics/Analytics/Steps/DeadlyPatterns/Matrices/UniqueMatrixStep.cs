namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Matrix</b> technique.
/// </summary>
public abstract class UniqueMatrixStep(Conclusion[] conclusions, View[]? views, scoped in CellMap cells, short digitsMask) :
	DeadlyPatternStep(conclusions, views)
{
	/// <inheritdoc/>
	public sealed override decimal BaseDifficulty => 5.3M;

	/// <summary>
	/// Indicates the type of the current technique step.
	/// </summary>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <summary>
	/// Indicates the mask that describes all digits used in this pattern.
	/// </summary>
	public short DigitsMask { get; } = digitsMask;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public sealed override TechniqueGroup Group => TechniqueGroup.DeadlyPattern;

	/// <inheritdoc/>
	public sealed override Technique Code => Enum.Parse<Technique>($"UniqueMatrixType{Type}");

	/// <summary>
	/// Indicates the cells used in this pattern.
	/// </summary>
	public CellMap Cells { get; } = cells;

	private protected string DigitsStr => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	private protected string CellsStr => Cells.ToString();
}
