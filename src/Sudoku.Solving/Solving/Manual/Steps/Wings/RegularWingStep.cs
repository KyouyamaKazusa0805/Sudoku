using Sudoku.Collections;
using Sudoku.Data;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;

namespace Sudoku.Solving.Manual.Steps.Wings;

/// <summary>
/// Provides with a step that is a <b>Regular Wing</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Pivot">Indicates the cell that blossomed its petals.</param>
/// <param name="PivotCandidatesCount">Indicates the number of digits in the pivot cell.</param>
/// <param name="DigitsMask">Indicates a mask that contins all digits used.</param>
/// <param name="Petals">Indicates the petals used.</param>
public sealed record RegularWingStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	int Pivot,
	int PivotCandidatesCount,
	short DigitsMask,
	in Cells Petals
) : WingStep(Conclusions, Views)
{
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
		3 => 4.2M,
		4 => 4.4M,
		5 => 4.6M,
		6 => 4.9M,
		7 => 5.2M,
		8 => 5.5M,
		9 => 5.9M
	} + (IsIncomplete ? Size == 3 ? 0.2M : 0.1M : 0);

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

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => Size switch
	{
		3 or 4 => DifficultyLevel.Hard,
		5 => DifficultyLevel.Fiendish,
		> 5 => DifficultyLevel.Nightmare
	};

	/// <inheritdoc/>
	public override Rarity Rarity => Size switch
	{
		2 => Rarity.Often,
		3 or 4 => Rarity.Seldom,
		5 => Rarity.HardlyEver,
		> 5 => Rarity.OnlyForSpecialPuzzles
	};

	/// <summary>
	/// Indicates the internal name.
	/// </summary>
	private string InternalName => Size switch
	{
		3 => IsIncomplete ? "XY-Wing" : "XYZ-Wing",
		>= 4 and < 9 when Size switch
		{
			4 => "WXYZ-Wing",
			5 => "VWXYZ-Wing",
			6 => "UVWXYZ-Wing",
			7 => "TUVWXYZ-Wing",
			8 => "STUVWXYZ-Wing",
			9 => "RSTUVWXYZ-Wing"
		} is var name => IsIncomplete ? $"Incomplete {name}" : name
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
		get => new Coordinate((byte)Pivot).ToString();
	}

	[FormatItem]
	private string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Petals.ToString();
	}
}
