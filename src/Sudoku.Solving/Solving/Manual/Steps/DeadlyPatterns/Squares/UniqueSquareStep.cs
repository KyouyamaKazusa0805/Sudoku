using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Square</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells">Indicates the cells used.</param>
/// <param name="DigitsMask">Indicates the digits used.</param>
public abstract record UniqueSquareStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	in Cells Cells,
	short DigitsMask
) : DeadlyPatternStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 5.3M;

	/// <summary>
	/// Indicates the type of the current technique step.
	/// </summary>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.DeadlyPattern;

	/// <inheritdoc/>
	public sealed override TechniqueTags TechniqueTags => TechniqueTags.DeadlyPattern;

	/// <inheritdoc/>
	public sealed override Technique TechniqueCode => Enum.Parse<Technique>($"UniqueSquareType{Type}");

	/// <inheritdoc/>
	public sealed override Rarity Rarity => Rarity.HardlyEver;

	/// <summary>
	/// Indicates the digits string.
	/// </summary>
	[FormatItem]
	internal string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(DigitsMask).ToString();
	}

	/// <summary>
	/// Indicates the cells string.
	/// </summary>
	[FormatItem]
	internal string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Cells.ToString();
	}
}
