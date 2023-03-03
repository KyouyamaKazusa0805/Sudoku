﻿namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle External Turbot Fish</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="GuardianCells">Indicates the cells that the guardians lie in.</param>
/// <param name="CellPair">Indicates the cell pair.</param>
/// <param name="TurbotFishDigit">Indicates the digit that the turbot fish used.</param>
/// <param name="BaseHouse">Indicates the base house used.</param>
/// <param name="TargetHouse">Indicates the target house used.</param>
/// <param name="IsIncomplete">Indicates whether the rectangle is incomplete.</param>
/// <param name="IsAvoidable">Indicates whether the structure is based on avoidable rectangle.</param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
internal sealed record UniqueRectangleExternalTurbotFishStep(
	Conclusion[] Conclusions,
	View[]? Views,
	int Digit1,
	int Digit2,
	scoped in CellMap Cells,
	scoped in CellMap GuardianCells,
	scoped in CellMap CellPair,
	int TurbotFishDigit,
	int BaseHouse,
	int TargetHouse,
	bool IsIncomplete,
	bool IsAvoidable,
	int AbsoluteOffset
) : UniqueRectangleStep(
	Conclusions,
	Views,
	(IsAvoidable, BaseHouse / 9, TargetHouse / 9) switch
	{
		(true, 0, _) or (true, _, 0) => Technique.AvoidableRectangleExternalTurbotFish,
		(true, 1, 1) or (true, 2, 2) => Technique.AvoidableRectangleExternalSkyscraper,
		(true, 1, 2) or (true, 2, 1) => Technique.AvoidableRectangleExternalTwoStringKite,
		(false, 0, _) or (false, _, 0) => Technique.UniqueRectangleExternalTurbotFish,
		(false, 1, 1) or (false, 2, 2) => Technique.UniqueRectangleExternalSkyscraper,
		(false, 1, 2) or (false, 2, 1) => Technique.UniqueRectangleExternalTwoStringKite
	},
	Digit1,
	Digit2,
	Cells,
	IsAvoidable,
	AbsoluteOffset
)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .1M;

	/// <inheritdoc/>
	public override string? Format => R["TechniqueFormat_UniqueRectangleStep"];

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[]
		{
			new(ExtraDifficultyCaseNames.Guardian, A004526(GuardianCells.Count) * .1M),
			new(ExtraDifficultyCaseNames.Avoidable, IsAvoidable ? .1M : 0),
			new(ExtraDifficultyCaseNames.Incompleteness, IsIncomplete ? .1M : 0)
		};

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { D1Str, D2Str, CellsStr, GuardianCellsStr, CellPairStr, TurbotFishDigitStr } },
			{ "zh", new[] { D1Str, D2Str, CellsStr, GuardianCellsStr, CellPairStr, TurbotFishDigitStr } }
		};

	private string TurbotFishDigitStr => (TurbotFishDigit + 1).ToString();

	private string GuardianCellsStr => GuardianCells.ToString();

	private string CellPairStr => CellPair.ToString();
}
