namespace Sudoku.Solving.Manual.Alses;

/// <summary>
/// Provides a usage of <b>empty rectangle intersection pair</b> (ERIP) technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="StartCell">The start cell.</param>
/// <param name="EndCell">The end cell.</param>
/// <param name="Region">The region that empty rectangle forms.</param>
/// <param name="Digit1">The digit 1.</param>
/// <param name="Digit2">The digit 2.</param>
public sealed record class EripStepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
	int StartCell, int EndCell, int Region, int Digit1, int Digit2
) : AlsStepInfo(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 6.0M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.Erip;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.Erip;

	[FormatItem]
	private string Digit1Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit1 + 1).ToString();
	}

	[FormatItem]
	private string Digit2Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit2 + 1).ToString();
	}

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
	private string RegionStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new RegionCollection(Region).ToString();
	}
}
