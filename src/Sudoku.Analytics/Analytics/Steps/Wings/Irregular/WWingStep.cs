using System.Numerics;
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
/// Provides with a step that is a <b>W-Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="startCell">Indicates the start cell.</param>
/// <param name="endCell">Indicates the end cell.</param>
/// <param name="conjugatePair">Indicates the conjugate pair connecting with cells <see cref="StartCell"/> and <see cref="EndCell"/>.</param>
public sealed partial class WWingStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[Data] Cell startCell,
	[Data] Cell endCell,
	[Data] scoped ref readonly Conjugate conjugatePair
) : IrregularWingStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.4M;

	/// <inheritdoc/>
	public override decimal BaseLocatingDifficulty => 440;

	/// <inheritdoc/>
	public override Technique Code => Technique.WWing;

	/// <inheritdoc/>
	public override LocatingDifficultyFactor[] LocatingDifficultyFactors
	{
		get
		{
			_ = (GetPairedCellFor(StartCell, out var startHouse), GetPairedCellFor(EndCell, out var endHouse));
			return [
				new(
					LocatingDifficultyFactorNames.HouseType,
					9 * GetHouseTypeScore(ConjugatePair.Houses.SetAt(0).ToHouseType())
				),
				new(
					LocatingDifficultyFactorNames.Connector,
					(GetHouseTypeScore(startHouse.ToHouseType()) + GetHouseTypeScore(endHouse.ToHouseType())) * 6
				)
			];
		}
	}

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [StartCellStr, EndCellStr, ConjStr]), new(ChineseLanguage, [StartCellStr, EndCellStr, ConjStr])];

	private string StartCellStr => Options.Converter.CellConverter([StartCell]);

	private string EndCellStr => Options.Converter.CellConverter([EndCell]);

	private string ConjStr => Options.Converter.ConjugateConverter([ConjugatePair]);


	/// <summary>
	/// Try to get the score for house type.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int GetHouseTypeScore(HouseType houseType)
		=> houseType switch { HouseType.Block => 1, HouseType.Row => 3, HouseType.Column => 6 };

	/// <summary>
	/// Try to get the paired cell by the specified start or end cell.
	/// </summary>
	private Cell GetPairedCellFor(Cell startOrEndCell, out House house)
	{
		if (ConjugatePair.Map is not [var a, var b])
		{
			throw new InvalidOperationException("The conjugate pair map is invalid.");
		}

		if ((CellMap.Empty + a + startOrEndCell).InOneHouse(out house))
		{
			return a;
		}

		if ((CellMap.Empty + b + startOrEndCell).InOneHouse(out house))
		{
			return b;
		}

		return house = -1;
	}
}
