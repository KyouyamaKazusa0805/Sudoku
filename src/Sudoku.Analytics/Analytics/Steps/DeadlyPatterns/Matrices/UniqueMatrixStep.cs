namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Matrix</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cells">Indicates the cells used in this pattern.</param>
/// <param name="digitsMask">Indicates the mask that describes all digits used in this pattern.</param>
public abstract partial class UniqueMatrixStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[Data] scoped ref readonly CellMap cells,
	[Data] Mask digitsMask
) : DeadlyPatternStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public sealed override decimal BaseDifficulty => 5.3M;

	/// <summary>
	/// Indicates the type of the current technique step.
	/// </summary>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public sealed override Technique Code => Enum.Parse<Technique>($"UniqueMatrixType{Type}");

	private protected string DigitsStr => Options.Converter.DigitConverter(DigitsMask);

	private protected string CellsStr => Options.Converter.CellConverter(Cells);
}
