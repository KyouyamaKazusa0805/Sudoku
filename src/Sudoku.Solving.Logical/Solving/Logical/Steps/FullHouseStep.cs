namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Full House</b> technique.
/// </summary>
/// <param name="Cell"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Digit"><inheritdoc/></param>
internal sealed record FullHouseStep(Conclusion[] Conclusions, View[]? Views, int Cell, int Digit) :
	SingleStep(Conclusions, Views, Cell, Digit)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 1.0M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.FullHouse;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Always;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?>? FormatInterpolatedParts => null;
}
