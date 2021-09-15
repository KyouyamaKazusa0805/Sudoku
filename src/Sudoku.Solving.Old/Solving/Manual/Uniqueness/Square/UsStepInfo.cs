namespace Sudoku.Solving.Manual.Uniqueness.Square;

/// <summary>
/// Provides a usage of <b>unique square</b> (US) technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Cells">The cells.</param>
/// <param name="DigitsMask">The digits mask.</param>
public abstract record class UsStepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Cells Cells, short DigitsMask
) : UniquenessStepInfo(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 5.3M;

	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.DeadlyPattern;

	/// <inheritdoc/>
	public abstract override Technique TechniqueCode { get; }

	/// <summary>
	/// Indicates the digits string.
	/// </summary>
	[FormatItem]
	protected string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(DigitsMask).ToString();
	}

	/// <summary>
	/// Indicates the cells string.
	/// </summary>
	[FormatItem]
	protected string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Cells.ToString();
	}
}
