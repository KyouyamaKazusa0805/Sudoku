﻿namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells">Indicates the cells used.</param>
/// <param name="DigitsMask">Indicates the mask that contains the digits used.</param>
internal abstract record ExtendedRectangleStep(ConclusionList Conclusions, ViewList Views, scoped in CellMap Cells, short DigitsMask) :
	DeadlyPatternStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public sealed override decimal BaseDifficulty => 4.5M;

	/// <summary>
	/// Indicates the type of the step. The value must be between 1 and 4.
	/// </summary>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	public sealed override Technique TechniqueCode => Enum.Parse<Technique>($"ExtendedRectangleType{Type}");

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.ExtendedRectangle;

	/// <inheritdoc/>
	public sealed override TechniqueTags TechniqueTags => base.TechniqueTags;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.Size, (A004526(Cells.Count) - 2) * .1M) };


	/// <summary>
	/// Indicates the digits string.
	/// </summary>
	[ResourceTextFormatter]
	internal string DigitsStr() => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	/// <summary>
	/// Indicates the cells string.
	/// </summary>
	[ResourceTextFormatter]
	internal string CellsStr() => Cells.ToString();
}
