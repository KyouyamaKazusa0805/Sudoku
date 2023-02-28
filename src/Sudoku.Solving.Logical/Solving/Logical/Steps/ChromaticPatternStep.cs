namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Chromatic Pattern</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Blocks">The blocks used.</param>
/// <param name="Pattern">The cells used.</param>
/// <param name="DigitsMask">The digits mask.</param>
internal abstract record ChromaticPatternStep(ConclusionList Conclusions, ViewList Views, int[] Blocks, scoped in CellMap Pattern, short DigitsMask) :
	NegativeRankStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 6.5M;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.TrivalueOddagon;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public sealed override Rarity Rarity => Rarity.OnlyForSpecialPuzzles;


	[ResourceTextFormatter]
	internal string BlocksStr() => string.Join(", ", from block in Blocks select $"{block + 1}");

	[ResourceTextFormatter]
	internal string CellsStr() => Pattern.ToString();

	[ResourceTextFormatter]
	internal string DigitsStr() => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);
}
