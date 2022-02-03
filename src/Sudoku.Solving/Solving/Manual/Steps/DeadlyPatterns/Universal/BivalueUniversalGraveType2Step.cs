using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Bivalue Universal Grave Type 2</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit">Indicates the digit used.</param>
/// <param name="Cells">Indicates the cells used.</param>
public sealed record BivalueUniversalGraveType2Step(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	int Digit,
	in Cells Cells
) : BivalueUniversalGraveStep(Conclusions, Views)
{
	/// <summary>
	/// The table of extra difficulty values.
	/// </summary>
	private static readonly decimal[] ExtraDifficulty =
	{
		.1M, .2M, .2M, .3M, .3M, .3M, .4M, .4M, .4M, .4M,
		.5M, .5M, .5M, .5M, .5M, .6M, .6M, .6M, .6M, .6M
	};


	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty + ExtraDifficulty[Cells.Count - 1];

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueUniversalGraveType2;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	[FormatItem]
	internal string ExtraDigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit + 1).ToString();
	}

	[FormatItem]
	internal string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Cells.ToString();
	}
}
