namespace Sudoku.Solving.Manual.Fishes;

/// <summary>
/// Provides a usage of <b>Hobiwan's fish</b> technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Digit">The digit used.</param>
/// <param name="BaseSets">The base sets.</param>
/// <param name="CoverSets">The cover sets.</param>
/// <param name="Exofins">All exo-fins.</param>
/// <param name="Endofins">All endo-fins.</param>
/// <param name="IsFranken">Indicates whether the current structure is a Franken fish.</param>
/// <param name="IsSashimi">
/// Indicates whether the fish instance is sashimi.
/// The value can be:
/// <list type="table">
/// <item>
/// <term><see langword="true"/></term><description>Sashimi finned fish.</description>
/// </item>
/// <item>
/// <term><see langword="false"/></term><description>Normal finned fish.</description>
/// </item>
/// <item>
/// <term><see langword="null"/></term><description>Normal fish.</description>
/// </item>
/// </list>
/// </param>
[AutoGetHashCode(nameof(BaseHashCode), nameof(BaseSetHashCode), nameof(CoverSetHashCode), nameof(Exofins), nameof(Endofins))]
public sealed partial record ComplexFishStepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit,
	IReadOnlyList<int> BaseSets, IReadOnlyList<int> CoverSets, in Cells Exofins,
	in Cells Endofins, bool IsFranken, bool? IsSashimi
) : FishStepInfo(Conclusions, Views, Digit, BaseSets, CoverSets)
{
	/// <summary>
	/// The basic difficulty rating table.
	/// </summary>
	private static readonly decimal[] BasicDiff = { 0, 0, 3.2M, 3.8M, 5.2M, 6.0M, 6.0M, 6.6M, 7.0M };

	/// <summary>
	/// The finned difficulty rating table.
	/// </summary>
	private static readonly decimal[] FinnedDiff = { 0, 0, .2M, .2M, .2M, .3M, .3M, .3M, .4M };

	/// <summary>
	/// The sashimi difficulty rating table.
	/// </summary>
	private static readonly decimal[] SashimiDiff = { 0, 0, .3M, .3M, .4M, .4M, .5M, .6M, .7M };

	/// <summary>
	/// The Franken shape extra difficulty rating table.
	/// </summary>
	private static readonly decimal[] FrankenShapeDiffExtra = { 0, 0, .2M, 1.2M, 1.2M, 1.3M, 1.3M, 1.3M, 1.4M };

	/// <summary>
	/// The mutant shape extra difficulty rating table.
	/// </summary>
	private static readonly decimal[] MutantShapeDiffExtra = { 0, 0, .3M, 1.4M, 1.4M, 1.5M, 1.5M, 1.5M, 1.6M };


	/// <inheritdoc/>
	public override decimal Difficulty => BaseDifficulty + SashimiExtraDifficulty + ShapeExtraDifficulty;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => Size switch
	{
		2 => DifficultyLevel.Hard,
		3 or 4 => DifficultyLevel.Fiendish,
		_ => DifficultyLevel.Nightmare
	};

	/// <inheritdoc/>
	public override Technique TechniqueCode => GetComplexFishTechniqueCodeFromName(InternalName);

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.ComplexFish;

	/// <summary>
	/// Indicates the base difficulty.
	/// </summary>
	private decimal BaseDifficulty => BasicDiff[Size];

	/// <summary>
	/// Indicates the extra difficulty on sashimi judgement.
	/// </summary>
	private decimal SashimiExtraDifficulty =>
		IsSashimi switch { false => FinnedDiff[Size], true => SashimiDiff[Size], null => 0 };

	/// <summary>
	/// Indicates the extra difficulty on shape.
	/// </summary>
	private decimal ShapeExtraDifficulty =>
		IsFranken ? FrankenShapeDiffExtra[Size] : MutantShapeDiffExtra[Size];

	/// <summary>
	/// Indicates the base hash code.
	/// </summary>
	private int BaseHashCode => Digit << 17 & 0xABC0DEF;

	/// <summary>
	/// Indicates the base set hash code.
	/// </summary>
	private int BaseSetHashCode => new RegionCollection(BaseSets).GetHashCode();

	/// <summary>
	/// Indicates the cover set hash code.
	/// </summary>
	private int CoverSetHashCode => new RegionCollection(CoverSets).GetHashCode();

	/// <summary>
	/// The internal name.
	/// </summary>
	private string InternalName
	{
		get
		{
			string? fin = FinModifier == FinModifiers.Normal ? null : $"{FinModifier} ";
			string? shape = ShapeModifier == ShapeModifiers.Basic ? null : $"{ShapeModifier }";
			return $"{fin}{shape}{FishNames[Size]}";
		}
	}

	/// <summary>
	/// Indicates the fin modifier.
	/// </summary>
	private FinModifiers FinModifier => IsSashimi switch
	{
		true => FinModifiers.Sashimi,
		false => FinModifiers.Finned,
		_ => FinModifiers.Normal
	};

	/// <summary>
	/// The shape modifier.
	/// </summary>
	private ShapeModifiers ShapeModifier => IsFranken ? ShapeModifiers.Franken : ShapeModifiers.Mutant;

	[FormatItem]
	private string DigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit + 1).ToString();
	}

	[FormatItem]
	private string BaseSetsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new RegionCollection(BaseSets).ToString();
	}

	[FormatItem]
	private string CoverSetsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new RegionCollection(CoverSets).ToString();
	}

	[FormatItem]
	private string ExofinsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Exofins.IsEmpty ? string.Empty : $"f{Exofins} ";
	}

	[FormatItem]
	private string EndofinsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Endofins.IsEmpty ? string.Empty : $"ef{Endofins} ";
	}


	/// <inheritdoc/>
	public bool Equals(ComplexFishStepInfo? other) =>
		other is not null
		&& Digit == other.Digit
		&& new RegionCollection(BaseSets) == new RegionCollection(other.BaseSets)
		&& new RegionCollection(CoverSets) == new RegionCollection(other.CoverSets)
		&& Exofins == other.Exofins
		&& Endofins == other.Endofins;
}
