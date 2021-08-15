namespace Sudoku.Solving.Manual.Uniqueness.Extended;

/// <summary>
/// Provides a usage of <b>extended rectangle</b> (XR) technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Cells">All cells.</param>
/// <param name="DigitsMask">All digits mask.</param>
public abstract record XrStepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Cells Cells, short DigitsMask
) : UniquenessStepInfo(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 4.5M + (Cells.Count >> 1 - 2) * .1M;

	/// <inheritdoc/>
	public sealed override string? Acronym => "XR";

	/// <inheritdoc/>
	public abstract override Technique TechniqueCode { get; }

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.Xr;

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
