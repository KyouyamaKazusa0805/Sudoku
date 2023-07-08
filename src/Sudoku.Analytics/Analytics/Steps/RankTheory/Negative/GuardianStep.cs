namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Guardian</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digit">Indicates the digit used.</param>
/// <param name="loopCells">Indicates the cells of the loop used.</param>
/// <param name="guardians">Indicates the guardian cells.</param>
public sealed partial class GuardianStep(
	Conclusion[] conclusions,
	View[]? views,
	[PrimaryConstructorParameter] Digit digit,
	[PrimaryConstructorParameter] scoped in CellMap loopCells,
	[PrimaryConstructorParameter] scoped in CellMap guardians
) : NegativeRankStep(conclusions, views), IEquatableStep<GuardianStep>
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 5.5M;

	/// <inheritdoc/>
	public override Technique Code => Technique.BrokenWing;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new[] { (ExtraDifficultyCaseNames.Size, A004526(LoopCells.Count + A004526(Guardians.Count)) * .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ EnglishLanguage, new[] { CellsStr, GuardianSingularOrPlural, GuardianStr } },
			{ ChineseLanguage, new[] { CellsStr, GuardianSingularOrPlural, GuardianStr } }
		};

	private string CellsStr => LoopCells.ToString();

	private string GuardianSingularOrPlural => GetString(Guardians.Count == 1 ? "GuardianSingular" : "GuardianPlural")!;

	private string GuardianStr => Guardians.ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEquatableStep<GuardianStep>.operator ==(GuardianStep left, GuardianStep right)
		=> (left.Digit, left.LoopCells, left.Guardians) == (right.Digit, right.LoopCells, right.Guardians);
}
