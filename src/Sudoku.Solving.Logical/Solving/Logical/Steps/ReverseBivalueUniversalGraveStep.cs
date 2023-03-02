﻿namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Reverse Bi-value Universal Grave</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells">Indicates the cells used.</param>
/// <param name="DigitsMask">Indicates the digits used.</param>
internal abstract record ReverseBivalueUniversalGraveStep(ConclusionList Conclusions, ViewList Views, scoped in CellMap Cells, short DigitsMask) :
	DeadlyPatternStep(Conclusions, Views)
{
	/// <summary>
	/// Indicates the type of the step.
	/// </summary>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.DeadlyPattern;

	/// <inheritdoc/>
	public sealed override TechniqueTags TechniqueTags => TechniqueTags.DeadlyPattern;

	/// <inheritdoc/>
	public sealed override Rarity Rarity => Rarity.HardlyEver;

	private protected string DigitsStr => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	private protected string CellsStr => Cells.ToString();
}
