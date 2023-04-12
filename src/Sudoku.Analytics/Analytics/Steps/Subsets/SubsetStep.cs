namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Subset</b> technique.
/// </summary>
public abstract class SubsetStep(Conclusion[] conclusions, View[]? views, int house, scoped in CellMap cells, Mask digitsMask) :
	Step(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 3.0M;

	/// <summary>
	/// The house that structure lies in.
	/// </summary>
	public int House { get; } = house;

	/// <summary>
	/// Indicates the number of cells used.
	/// Due to the technique logic, you can also treat the result value of this property as the number of digits used.
	/// </summary>
	public int Size => PopCount((uint)DigitsMask);

	/// <summary>
	/// Indicates the mask that contains all digits used.
	/// </summary>
	public Mask DigitsMask { get; } = digitsMask;

	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <summary>
	/// Indicates all cells used.
	/// </summary>
	public CellMap Cells { get; } = cells;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Moderate;
}
