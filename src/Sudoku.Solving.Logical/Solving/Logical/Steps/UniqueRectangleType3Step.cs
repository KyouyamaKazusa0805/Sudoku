namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle Type 3</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="ExtraCells">Indicates the extra cells used.</param>
/// <param name="ExtraDigitsMask">Indicates the mask that contains all extra digits used.</param>
/// <param name="House">Indicates the house used.</param>
/// <param name="IsAvoidable"><inheritdoc/></param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
/// <param name="IsNaked">
/// <para>
/// Indicates whether the current instance uses a naked subset instead of a hidden subset to form the type 3.
/// </para>
/// <para>
/// In the default case, due to not having implemented the hidden subset cases,
/// the argument always keeps the value <see langword="true"/>. The possible values are:
/// <list type="table">
/// <item>
/// <term><see langword="true"/></term>
/// <description>The type 3 is with a naked subset.</description>
/// </item>
/// <item>
/// <term><see langword="false"/></term>
/// <description>The type 3 is with a hidden subset.</description>
/// </item>
/// </list>
/// </para>
/// </param>
[StepDisplayingFeature(StepDisplayingFeature.DifficultyRatingNotStable)]
internal sealed record UniqueRectangleType3Step(
	ConclusionList Conclusions,
	ViewList Views,
	int Digit1,
	int Digit2,
	scoped in CellMap Cells,
	scoped in CellMap ExtraCells,
	short ExtraDigitsMask,
	int House,
	bool IsAvoidable,
	int AbsoluteOffset,
	bool IsNaked = true
) : UniqueRectangleStep(
	Conclusions,
	Views,
	IsAvoidable ? Technique.AvoidableRectangleType3 : Technique.UniqueRectangleType3,
	Digit1,
	Digit2,
	Cells,
	IsAvoidable,
	AbsoluteOffset
)
{
	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[]
		{
			new(ExtraDifficultyCaseNames.Hidden, IsNaked ? 0 : .1M),
			new(ExtraDifficultyCaseNames.Size, PopCount((uint)ExtraDigitsMask) * .1M)
		};

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { D1Str, D2Str, CellsStr, DigitsStr, OnlyKeyword, CellsStr, HouseStr } },
			{ "zh", new[] { D1Str, D2Str, CellsStr, DigitsStr, OnlyKeywordZhCn, HouseStr, CellsStr, AppearLimitKeyword } }
		};

	private string DigitsStr => DigitMaskFormatter.Format(ExtraDigitsMask, FormattingMode.Normal);

	private string OnlyKeyword => IsNaked ? string.Empty : "only ";

	private string OnlyKeywordZhCn => R["Only"]!;

	private string HouseStr => HouseFormatter.Format(1 << House);

	private string AppearLimitKeyword => R["Appear"]!;
}
