namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Borescoper's Deadly Pattern</b> technique.
/// </summary>
public abstract class BorescoperDeadlyPatternStep(Conclusion[] conclusions, View[]? views, scoped in CellMap map, Mask digitsMask) :
	DeadlyPatternStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 5.3M;

	/// <summary>
	/// Indicates the type of the technique.
	/// </summary>
	public abstract int Type { get; }

	/// <summary>
	/// Indicates the mask of used digits.
	/// </summary>
	public Mask DigitsMask { get; } = digitsMask;

	/// <inheritdoc/>
	public sealed override string Format => R[$"TechniqueFormat_UniquePolygonType{Type}Step"]!;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public sealed override Technique Code => Enum.Parse<Technique>($"BorescoperDeadlyPatternType{Type}");

	/// <summary>
	/// The map that contains the cells used for this technique.
	/// </summary>
	public CellMap Map { get; } = map;

	private protected string DigitsStr => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	private protected string CellsStr => Map.ToString();
}
