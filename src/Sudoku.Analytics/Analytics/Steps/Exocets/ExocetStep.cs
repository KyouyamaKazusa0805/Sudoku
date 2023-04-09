namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Exocet</b> technique.
/// </summary>
public abstract class ExocetStep(View[]? views, Exocet exocet, short digitsMask, ExocetElimination[] eliminations) :
	Step((from e in eliminations from c in e.Conclusions select c).ToArray(), views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 9.4M;

	/// <summary>
	/// Indicates the mask of digits used.
	/// </summary>
	public short DigitsMask { get; } = digitsMask;

	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

	/// <summary>
	/// Indicates the exocet pattern used.
	/// </summary>
	public Exocet Exocet { get; } = exocet;

	private protected string DigitsStr => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	private protected string BaseCellsStr => BaseMap.ToString();

	private protected string TargetCellsStr => TargetMap.ToString();

	/// <summary>
	/// Indicates the map of the base cells.
	/// </summary>
	private CellMap BaseMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Exocet.BaseCellsMap;
	}

	/// <summary>
	/// Indicates the map of the target cells.
	/// </summary>
	private CellMap TargetMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Exocet.TargetCellsMap;
	}
}
