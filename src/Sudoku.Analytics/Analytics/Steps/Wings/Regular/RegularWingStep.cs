using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Regular Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="pivot">Indicates the cell that blossomed its petals.</param>
/// <param name="pivotCandidatesCount">Indicates the number of digits in the pivot cell.</param>
/// <param name="digitsMask">Indicates a mask that contains all digits used.</param>
/// <param name="petals">Indicates the petals used.</param>
/// <param name="distanceValueSumFromPetals">The total distinct value from all petals to the pivot.</param>
public sealed partial class RegularWingStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[Data] Cell pivot,
	[Data] int pivotCandidatesCount,
	[Data] Mask digitsMask,
	[Data] scoped ref readonly CellMap petals,
	[Data(DataMemberKinds.Field, Accessibility = "private readonly")] double distanceValueSumFromPetals
) : WingStep(conclusions, views, options)
{
	/// <summary>
	/// Indicates whether the pattern is incomplete.
	/// </summary>
	public bool IsIncomplete => Size == PivotCandidatesCount + 1;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.2M;

	/// <inheritdoc/>
	public override decimal BaseLocatingDifficulty => 420;

	/// <summary>
	/// Indicates the size of the wing. The size indicates the number of candidates that the pivot cell holds.
	/// </summary>
	/// <remarks>
	/// All names are:
	/// <list type="table">
	/// <item>
	/// <term>3</term>
	/// <description>XY-Wing or XYZ-Wing</description>
	/// </item>
	/// <item>
	/// <term>4</term>
	/// <description>WXYZ-Wing</description>
	/// </item>
	/// <item>
	/// <term>5</term>
	/// <description>VWXYZ-Wing</description>
	/// </item>
	/// <item>
	/// <term>6</term>
	/// <description>UVWXYZ-Wing</description>
	/// </item>
	/// <item>
	/// <term>7</term>
	/// <description>TUVWXYZ-Wing</description>
	/// </item>
	/// <item>
	/// <term>8</term>
	/// <description>STUVWXYZ-Wing</description>
	/// </item>
	/// <item>
	/// <term>9</term>
	/// <description>RSTUVWXYZ-Wing</description>
	/// </item>
	/// </list>
	/// </remarks>
	public int Size => PopCount((uint)DigitsMask);

	/// <inheritdoc/>
	public override Technique Code => TechniqueMarshal.MakeRegularWingTechniqueCode(TechniqueMarshal.GetRegularWingEnglishName(Size, IsIncomplete));

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors
		=> [
			new(
				ExtraDifficultyFactorNames.WingSize,
				Size switch { 3 => 0, 4 => .2M, 5 => .4M, 6 => .7M, 7 => 1.0M, 8 => 1.3M, 9 => 1.6M, _ => 2.0M }
			),
			new(
				ExtraDifficultyFactorNames.Incompleteness,
				(Code, IsIncomplete) switch { (Technique.XyWing, _) => 0, (Technique.XyzWing, _) => .2M, (_, true) => .1M, _ => 0 }
			)
		];

	/// <inheritdoc/>
	public override LocatingDifficultyFactor[] LocatingDifficultyFactors
		=> [
			new(LocatingDifficultyFactorNames.Petals, Petals.Count * 20),
			new(LocatingDifficultyFactorNames.Size, PivotCandidatesCount * 20),
			new(LocatingDifficultyFactorNames.Distance, (decimal)Math.Round(_distanceValueSumFromPetals * 6, 2))
		];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitsStr, PivotCellStr, CellsStr]), new(ChineseLanguage, [DigitsStr, PivotCellStr, CellsStr])];

	private string DigitsStr => Options.Converter.DigitConverter(DigitsMask);

	private string PivotCellStr => Options.Converter.CellConverter([Pivot]);

	private string CellsStr => Options.Converter.CellConverter(Petals);
}
