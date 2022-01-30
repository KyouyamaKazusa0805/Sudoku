using Sudoku.Collections;
using Sudoku.Data;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps.DeadlyPatterns.Rectangles;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle Type 2</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="TechniqueCode2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="IsAvoidable"><inheritdoc/></param>
/// <param name="ExtraDigit">Indicates the extra digit used.</param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
public sealed record UniqueRectangleType2Step(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	int Digit1,
	int Digit2,
	Technique TechniqueCode2,
	in Cells Cells,
	bool IsAvoidable,
	int ExtraDigit,
	int AbsoluteOffset
) : UniqueRectangleStep(Conclusions, Views, TechniqueCode2, Digit1, Digit2, Cells, IsAvoidable, AbsoluteOffset)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 4.6M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Often;

	[FormatItem]
	private string ExtraDigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ExtraDigit + 1).ToString();
	}
}
