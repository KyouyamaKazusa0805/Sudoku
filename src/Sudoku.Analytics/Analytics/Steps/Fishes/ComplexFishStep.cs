namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Complex Fish</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="baseSetsMask"><inheritdoc/></param>
/// <param name="coverSetsMask"><inheritdoc/></param>
/// <param name="exofins">
/// Indicates the fins, positioned outside the fish.
/// They will be defined if cover sets cannot be fully covered, with the number of cover sets being equal to the number of base sets.
/// </param>
/// <param name="endofins">
/// Indicates the fins, positioned inside the fish. Generally speaking, they will be defined if they are in multiple base sets.
/// </param>
/// <param name="isFranken">
/// Indicates whether the fish is a Franken fish. If <see langword="true"/>, a Franken fish; otherwise, a Mutant fish.
/// </param>
/// <param name="isSashimi">
/// <para>Indicates whether the fish is a Sashimi fish.</para>
/// <para>
/// All cases are as below:
/// <list type="table">
/// <item>
/// <term><see langword="true"/></term>
/// <description>The fish is a sashimi finned fish.</description>
/// </item>
/// <item>
/// <term><see langword="false"/></term>
/// <description>The fish is a normal finned fish.</description>
/// </item>
/// <item>
/// <term><see langword="null"/></term>
/// <description>The fish doesn't contain any fin.</description>
/// </item>
/// </list>
/// </para>
/// </param>
public sealed partial class ComplexFishStep(
	Conclusion[] conclusions,
	View[]? views,
	Digit digit,
	HouseMask baseSetsMask,
	HouseMask coverSetsMask,
	[PrimaryConstructorParameter] scoped in CellMap exofins,
	[PrimaryConstructorParameter] scoped in CellMap endofins,
	[PrimaryConstructorParameter] bool isFranken,
	[PrimaryConstructorParameter] bool? isSashimi
) : FishStep(conclusions, views, digit, baseSetsMask, coverSetsMask), IEquatableStep<ComplexFishStep>
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 3.2M;

	/// <inheritdoc/>
	public override Technique Code => FishStepSearcherHelper.GetComplexFishTechniqueCodeFromName(InternalName);

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> [
			new(ExtraDifficultyCaseNames.Size, Size switch { 2 => 0, 3 => .6M, 4 => 2.0M, 5 => 3.3M, 6 => 4.5M, 7 => 5.6M, _ => 6.6M }),
			new(
				ExtraDifficultyCaseNames.Sashimi,
				IsSashimi switch
				{
					false => Size switch { 2 or 3 or 4 => .2M, 5 or 6 or 7 => .3M, _ => .4M },
					true => Size switch { 2 or 3 => .3M, 4 or 5 => .4M, 6 => .5M, 7 => .6M, _ => .7M },
					_ => 0
				}
			),
			new(
				ExtraDifficultyCaseNames.FishShape,
				IsFranken
					? Size switch { 2 => 0, 3 or 4 => 1.1M, 5 or 6 or 7 => 1.2M, _ => 1.3M }
					: Size switch { 2 => 0, 3 or 4 => 1.4M, 5 or 6 => 1.6M, 7 => 1.7M, _ => 2.0M }
			)
		];

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ EnglishLanguage, [DigitStr, BaseSetsStr, CoverSetsStr, ExofinsStr, EndofinsStr] },
			{ ChineseLanguage, [BaseSetsStr, CoverSetsStr, DigitStr, ExofinsStr, EndofinsStr] }
		};

	/// <summary>
	/// Indicates the base houses.
	/// </summary>
	private House[] BaseHouses => [.. BaseSetsMask.GetAllSets()];

	/// <summary>
	/// Indicates the cover houses.
	/// </summary>
	private House[] CoverHouses => [.. CoverSetsMask.GetAllSets()];

	private string DigitStr => (Digit + 1).ToString();

	private string BaseSetsStr => HouseFormatter.Format(BaseHouses);

	private string CoverSetsStr => HouseFormatter.Format(CoverHouses);

	private string ExofinsStr => Exofins ? $"f{Exofins} " : string.Empty;

	private string EndofinsStr => Endofins ? $"ef{Endofins} " : string.Empty;

	/// <summary>
	/// The internal name.
	/// </summary>
	private string InternalName
	{
		get
		{
			var fin = FinModifier == ComplexFishFinKind.Normal ? null : $"{FinModifier} ";
			var shape = ShapeModifier == ComplexFishShapeKind.Basic ? null : $"{ShapeModifier} ";
			var sizeName = TechniqueFact.GetFishEnglishName(Size);
			return $"{fin}{shape}{sizeName}";
		}
	}

	/// <summary>
	/// Indicates the fin modifier.
	/// </summary>
	private ComplexFishFinKind FinModifier
		=> IsSashimi switch { true => ComplexFishFinKind.Sashimi, false => ComplexFishFinKind.Finned, _ => ComplexFishFinKind.Normal };

	/// <summary>
	/// The shape modifier.
	/// </summary>
	private ComplexFishShapeKind ShapeModifier => IsFranken ? ComplexFishShapeKind.Franken : ComplexFishShapeKind.Mutant;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEquatableStep<ComplexFishStep>.operator ==(ComplexFishStep left, ComplexFishStep right)
		=> left.Digit == right.Digit
		&& left.BaseSetsMask == right.BaseSetsMask && left.CoverSetsMask == right.CoverSetsMask
		&& left.Exofins == right.Exofins && left.Endofins == right.Endofins;
}
