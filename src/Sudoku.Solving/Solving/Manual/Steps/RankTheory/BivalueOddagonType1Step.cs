using Sudoku.Collections;
using Sudoku.Data;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps.RankTheory;

/// <summary>
/// Provides with a step that is a <b>Bi-value Oddagon Type 1</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Loop"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="ExtraCell">Indicates the extra cell.</param>
public sealed record BivalueOddagonType1Step(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	in Cells Loop,
	int Digit1,
	int Digit2,
	int ExtraCell
) : BivalueOddagonStep(Conclusions, Views, Loop, Digit1, Digit2)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 5.0M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueOddagonType1;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.ReplacedByOtherTechniques;

	[FormatItem]
	private string CellStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Coordinate((byte)ExtraCell).ToString();
	}

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
	private string LoopStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Loop.ToString();
	}
}
