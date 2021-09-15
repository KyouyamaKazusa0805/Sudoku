namespace Sudoku.Solving.Manual.Wings.Regular;

/// <summary>
/// Provides a usage of <b>regular wing</b> technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Pivot">The pivot cell.</param>
/// <param name="PivotCandidatesCount">The number of candidates that is in the pivot.</param>
/// <param name="DigitsMask">The mask of all digits used.</param>
/// <param name="Cells">The cells used.</param>
public sealed record class RegularWingStepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Pivot,
	int PivotCandidatesCount, short DigitsMask, IReadOnlyList<int> Cells
) : WingStepInfo(Conclusions, Views)
{
	/// <summary>
	/// The difficulty rating.
	/// </summary>
	private static readonly decimal[] DifficultyRating = { 0, 0, 0, 0, 4.6M, 4.8M, 5.1M, 5.4M, 5.7M, 6.0M };


	/// <summary>
	/// Indicates whether the structure is incomplete.
	/// </summary>
	public bool IsIncomplete => Size == PivotCandidatesCount + 1;

	/// <summary>
	/// Indicates the size of this regular wing.
	/// </summary>
	/// <remarks>
	/// All names are:
	/// <list type="table">
	/// <item>
	/// <term>2</term>
	/// <description>XY-Wing</description>
	/// </item>
	/// <item>
	/// <term>3</term>
	/// <description>XYZ-Wing</description>
	/// </item>
	/// <item>
	/// <term>4</term>
	/// <description>WXYZ-Wing</description>
	/// </item>
	/// <item>
	/// <term>5</term>
	/// <description>VWXYZ-Wing</description>
	/// </item>
	/// <item>
	/// <term>6</term>
	/// <description>UVWXYZ-Wing</description>
	/// </item>
	/// <item>
	/// <term>7</term>
	/// <description>TUVWXYZ-Wing</description>
	/// </item>
	/// <item>
	/// <term>8</term>
	/// <description>STUVWXYZ-Wing</description>
	/// </item>
	/// <item>
	/// <term>9</term>
	/// <description>RSTUVWXYZ-Wing</description>
	/// </item>
	/// </list>
	/// </remarks>
	public int Size => PopCount((uint)DigitsMask);

	/// <inheritdoc/>
	public override decimal Difficulty => Size switch
	{
		3 => IsIncomplete ? 4.2M : 4.4M,
		>= 4 and < 9 => IsIncomplete ? DifficultyRating[Size] + .1M : DifficultyRating[Size]
	};

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => Size switch
	{
		>= 3 and <= 4 => DifficultyLevel.Hard,
		> 4 and < 9 => DifficultyLevel.Fiendish
	};

	/// <inheritdoc/>
	public override Technique TechniqueCode => InternalName switch
	{
		"XY-Wing" => Technique.XyWing,
		"XYZ-Wing" => Technique.XyzWing,
		"WXYZ-Wing" => Technique.WxyzWing,
		"VWXYZ-Wing" => Technique.VwxyzWing,
		"UVWXYZ-Wing" => Technique.UvwxyzWing,
		"TUVWXYZ-Wing" => Technique.TuvwxyzWing,
		"STUVWXYZ-Wing" => Technique.StuvwxyzWing,
		"RSTUVWXYZ-Wing" => Technique.RstuvwxyzWing,
		"Incomplete WXYZ-Wing" => Technique.IncompleteWxyzWing,
		"Incomplete VWXYZ-Wing" => Technique.IncompleteVwxyzWing,
		"Incomplete UVWXYZ-Wing" => Technique.IncompleteUvwxyzWing,
		"Incomplete TUVWXYZ-Wing" => Technique.IncompleteTuvwxyzWing,
		"Incomplete STUVWXYZ-Wing" => Technique.IncompleteStuvwxyzWing,
		"Incomplete RSTUVWXYZ-Wing" => Technique.IncompleteRstuvwxyzWing
	};

	/// <summary>
	/// Indicates the internal name.
	/// </summary>
	private string InternalName => Size switch
	{
		3 => IsIncomplete ? "XY-Wing" : "XYZ-Wing",
		>= 4 and < 9 => IsIncomplete ? $"Incomplete {RegularWingNames[Size]}" : RegularWingNames[Size]
	};

	[FormatItem]
	private string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(DigitsMask).ToString();
	}

	[FormatItem]
	private string PivotCellStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Cells { Pivot }.ToString();
	}

	[FormatItem]
	private string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Cells(Cells).ToString();
	}
}
