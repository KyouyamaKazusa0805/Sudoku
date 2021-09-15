namespace Sudoku.Solving.Manual.Wings.Irregular;

/// <summary>
/// Provides a usage of <b>W-Wing</b> technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="StartCell">The start cell.</param>
/// <param name="EndCell">The end cell.</param>
/// <param name="ConjugatePair">The conjugate pair.</param>
public sealed record class WWingStepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
	int StartCell, int EndCell, in ConjugatePair ConjugatePair
) : IrregularWingStepInfo(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 4.4M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.WWing;

	[FormatItem]
	private string StartCellStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Cells { StartCell }.ToString();
	}

	[FormatItem]
	private string EndCellStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Cells { EndCell }.ToString();
	}

	[FormatItem]
	private string ConjStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ConjugatePair.ToString();
	}
}
