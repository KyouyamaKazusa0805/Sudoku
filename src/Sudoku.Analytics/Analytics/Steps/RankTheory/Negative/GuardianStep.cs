using System.Algorithm;
using System.Runtime.CompilerServices;
using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using static Sudoku.Analytics.Strings.StringsAccessor;

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
	[Data] Digit digit,
	[Data] scoped ref readonly CellMap loopCells,
	[Data] scoped ref readonly CellMap guardians
) : NegativeRankStep(conclusions, views, options), IEquatableStep<GuardianStep>
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 5.5M;

	/// <inheritdoc/>
	public override Technique Code => Technique.BrokenWing;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> [new(ExtraDifficultyCaseNames.Size, Sequences.A004526(LoopCells.Count + Sequences.A004526(Guardians.Count)) * .1M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [CellsStr, GuardianSingularOrPlural, GuardianStr]),
			new(ChineseLanguage, [CellsStr, GuardianSingularOrPlural, GuardianStr])
		];

	private string CellsStr => Options.Converter.CellConverter(LoopCells);

	private string GuardianSingularOrPlural => GetString(Guardians.Count == 1 ? "GuardianSingular" : "GuardianPlural")!;

	private string GuardianStr => Options.Converter.CellConverter(Guardians);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEquatableStep<GuardianStep>.operator ==(GuardianStep left, GuardianStep right)
		=> (left.Digit, left.LoopCells, left.Guardians) == (right.Digit, right.LoopCells, right.Guardians);
}
