namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Complex Fish</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit"><inheritdoc/></param>
/// <param name="BaseSetsMask"><inheritdoc/></param>
/// <param name="CoverSetsMask"><inheritdoc/></param>
/// <param name="Exofins">The exo-fins.</param>
/// <param name="Endofins">The endo-fins.</param>
/// <param name="IsFranken">Indicates whether the fish is a Franken fish.</param>
/// <param name="IsSashimi">Indicates whether the fish is a Sashimi fish.</param>
internal sealed record ComplexFishStep(
	Conclusion[] Conclusions,
	View[]? Views,
	int Digit,
	int BaseSetsMask,
	int CoverSetsMask,
	scoped in CellMap Exofins,
	scoped in CellMap Endofins,
	bool IsFranken,
	bool? IsSashimi
) : FishStep(Conclusions, Views, Digit, BaseSetsMask, CoverSetsMask), IDistinctableStep<ComplexFishStep>
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 3.2M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel
		=> Size switch { 2 => DifficultyLevel.Hard, 3 or 4 => DifficultyLevel.Fiendish, _ => DifficultyLevel.Nightmare };

	/// <inheritdoc/>
	public override Technique TechniqueCode => GetComplexFishTechniqueCodeFromName(InternalName);

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.ComplexFish;

	/// <inheritdoc/>
	public override Rarity Rarity
		=> (Size, ShapeModifier, FinModifier) switch
		{
			(3, _, _) => Rarity.Sometimes,
			(4, ComplexFishShapeKind.Franken, _) => Rarity.Sometimes,
			(4, ComplexFishShapeKind.Mutant, _) => Rarity.Seldom,
			(5, ComplexFishShapeKind.Franken, ComplexFishFinKind.Sashimi) => Rarity.Seldom,
			(5, ComplexFishShapeKind.Mutant, _) => Rarity.Seldom,
			_ => Rarity.HardlyEver
		};

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[]
		{
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
		};

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitStr, BaseSetsStr, CoverSetsStr, ExofinsStr, EndofinsStr } },
			{ "zh", new[] { BaseSetsStr, CoverSetsStr, DigitStr, ExofinsStr, EndofinsStr } }
		};

	/// <summary>
	/// Indicates the base houses.
	/// </summary>
	private int[] BaseHouses
	{
		get
		{
			var result = new int[PopCount((uint)BaseSetsMask)];
			var i = 0;
			foreach (var house in BaseSetsMask)
			{
				result[i++] = house;
			}

			return result;
		}
	}

	/// <summary>
	/// Indicates the cover houses.
	/// </summary>
	private int[] CoverHouses
	{
		get
		{
			var result = new int[PopCount((uint)CoverSetsMask)];
			var i = 0;
			foreach (var house in CoverSetsMask)
			{
				result[i++] = house;
			}

			return result;
		}
	}

	private string DigitStr => (Digit + 1).ToString();

	private string BaseSetsStr => HouseFormatter.Format(BaseHouses);

	private string CoverSetsStr => HouseFormatter.Format(CoverHouses);

	private string ExofinsStr => Exofins ? $"f{Exofins} " : string.Empty;

	private string EndofinsStr => Endofins ? $"ef{Endofins} " : string.Empty;

	/// <summary>
	/// The internal name.
	/// </summary>
	[DebuggerHidden]
	private string InternalName
	{
		get
		{
			var fin = FinModifier == ComplexFishFinKind.Normal ? null : $"{FinModifier} ";
			var shape = ShapeModifier == ComplexFishShapeKind.Basic ? null : $"{ShapeModifier} ";
			var sizeName = Size switch
			{
				2 => "X-Wing",
				3 => "Swordfish",
				4 => "Jellyfish",
				5 => "Squirmbag",
				6 => "Whale",
				7 => "Leviathan"
			};
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
	public static bool Equals(ComplexFishStep left, ComplexFishStep right)
		=> left.Digit == right.Digit
		&& left.BaseSetsMask == right.BaseSetsMask && left.CoverSetsMask == right.CoverSetsMask
		&& left.Exofins == right.Exofins && left.Endofins == right.Endofins;
}
