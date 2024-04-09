namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Guardian</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit">Indicates the digit used.</param>
/// <param name="loopCells">Indicates the cells of the loop used.</param>
/// <param name="guardians">Indicates the guardian cells.</param>
public sealed partial class GuardianStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] Digit digit,
	[PrimaryConstructorParameter] scoped ref readonly CellMap loopCells,
	[PrimaryConstructorParameter] scoped ref readonly CellMap guardians
) : NegativeRankStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 55;

	/// <inheritdoc/>
	public override Technique Code => Technique.BrokenWing;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [CellsStr, GuardianSingularOrPlural, GuardianStr]),
			new(ChineseLanguage, [CellsStr, GuardianSingularOrPlural, GuardianStr])
		];

	/// <inheritdoc/>
	public override FactorCollection Factors => [new GuardianFactor()];

	private string CellsStr => Options.Converter.CellConverter(LoopCells);

	private string GuardianSingularOrPlural => ResourceDictionary.Get(Guardians.Count == 1 ? "GuardianSingular" : "GuardianPlural", ResultCurrentCulture);

	private string GuardianStr => Options.Converter.CellConverter(Guardians);


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Step? other)
		=> other is GuardianStep comparer && (Digit, LoopCells, Guardians) == (comparer.Digit, comparer.LoopCells, comparer.Guardians);

	/// <inheritdoc/>
	public override int CompareTo(Step? other)
	{
		if (other is not GuardianStep comparer)
		{
			return 1;
		}

		var r1 = Math.Abs(LoopCells.Count - comparer.LoopCells.Count);
		if (r1 != 0)
		{
			return r1;
		}

		return Math.Abs(Guardians.Count - comparer.Guardians.Count);
	}
}
