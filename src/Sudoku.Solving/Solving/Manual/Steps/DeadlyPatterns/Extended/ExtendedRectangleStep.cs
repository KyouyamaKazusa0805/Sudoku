using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells">Indicates the cells used.</param>
/// <param name="DigitsMask">Indicates the mask that contains the digits used.</param>
public abstract record ExtendedRectangleStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	in Cells Cells,
	short DigitsMask
) : DeadlyPatternStep(Conclusions, Views)
{
	/// <summary>
	/// Indicates the type of the step. The value must be between 1 and 4.
	/// </summary>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public override decimal Difficulty => 4.5M + ((Cells.Count >> 1) - 2) * .1M;

	/// <inheritdoc/>
	public sealed override Technique TechniqueCode => Enum.Parse<Technique>($"ExtendedRectangleType{Type}");

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.ExtendedRectangle;

	/// <inheritdoc/>
	public sealed override TechniqueTags TechniqueTags => base.TechniqueTags;

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
