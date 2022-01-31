using Sudoku.Collections;
using Sudoku.Data;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>3-demensional Sue de Coq</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="RowDigitsMask">The row digits mask.</param>
/// <param name="ColumnDigitsMask">The column digits mask.</param>
/// <param name="BlockDigitsMask">The block digits mask.</param>
/// <param name="RowCells">The row cells map.</param>
/// <param name="ColumnCells">The column cells map.</param>
/// <param name="BlockCells">The block cells map.</param>
public sealed record SueDeCoq3DemensionStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	short RowDigitsMask,
	short ColumnDigitsMask,
	short BlockDigitsMask,
	Cells RowCells,
	Cells ColumnCells,
	Cells BlockCells
) : RankTheoryStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 5.5M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.SueDeCoq3Dimension;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.Als;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.SueDeCoq;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.HardlyEver;

	[FormatItem]
	private string Cells1Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => RowCells.ToString();
	}

	[FormatItem]
	private string Digits1Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(RowDigitsMask).ToString();
	}

	[FormatItem]
	private string Cells2Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ColumnCells.ToString();
	}

	[FormatItem]
	private string Digits2Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(ColumnDigitsMask).ToString();
	}

	[FormatItem]
	private string Cells3Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => BlockCells.ToString();
	}

	[FormatItem]
	private string Digits3Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(BlockDigitsMask).ToString();
	}
}
