namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework Quadruple</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells">The cells used.</param>
/// <param name="DigitsMask">The digits used.</param>
internal sealed record FireworkQuadrupleStep(ConclusionList Conclusions, ViewList Views, scoped in CellMap Cells, short DigitsMask) :
	FireworkStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .4M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.FireworkQuadruple;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { CellsStr, DigitsStr } }, { "zh", new[] { CellsStr, DigitsStr } } };

	private string CellsStr => Cells.ToString();

	private string DigitsStr => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);
}
