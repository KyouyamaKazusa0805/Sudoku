namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>RW's Deadly Pattern</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Pattern">Indicates the pattern.</param>
/// <param name="DigitsMask">Indicates the digits used.</param>
/// <param name="ChuteIndex">Indicates the global chute index.</param>
internal sealed record RwDeadlyPatternStep(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Pattern,
	short DigitsMask,
	int ChuteIndex
) : DeadlyPatternStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 7.5M;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.RwDeadlyPattern;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.HardlyEver;

	[ResourceTextFormatter]
	internal string CellsStr() => RxCyNotation.ToCellsString(Pattern);

	[ResourceTextFormatter]
	internal string DigitsStr() => DigitMaskFormatter.Format(DigitsMask);
}
