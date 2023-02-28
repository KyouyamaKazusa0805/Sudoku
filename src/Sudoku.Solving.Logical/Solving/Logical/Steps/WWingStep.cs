namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>W-Wing</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="StartCell">Indicates the start cell.</param>
/// <param name="EndCell">Indicates the end cell.</param>
/// <param name="ConjugatePair">
/// Indicates the conjugate pair connecting with cells <see cref="StartCell"/> and <see cref="EndCell"/>.
/// </param>
internal sealed record WWingStep(ConclusionList Conclusions, ViewList Views, int StartCell, int EndCell, scoped in Conjugate ConjugatePair) :
	IrregularWingStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.4M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.WWing;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Often;


	[ResourceTextFormatter]
	internal string StartCellStr() => RxCyNotation.ToCellString(StartCell);

	[ResourceTextFormatter]
	internal string EndCellStr() => RxCyNotation.ToCellString(EndCell);

	[ResourceTextFormatter]
	internal string ConjStr() => ConjugatePair.ToString();
}
