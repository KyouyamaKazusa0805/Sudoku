using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;
using static Sudoku.Solving.Manual.Constants;

namespace Sudoku.Solving.Manual.Steps;

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
public sealed partial record ComplexFishStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	int Digit,
	int BaseSetsMask,
	int CoverSetsMask,
	in Cells Exofins,
	in Cells Endofins,
	bool IsFranken,
	bool? IsSashimi
) : FishStep(Conclusions, Views, Digit, BaseSetsMask, CoverSetsMask), IDistinctableStep<ComplexFishStep>
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
	public override decimal Difficulty =>
		BasicDiff[Size] // Base difficulty.
		+ IsSashimi switch { false => FinnedDiff[Size], true => SashimiDiff[Size], _ => 0 } // Sashimi difficulty.
		+ (IsFranken ? FrankenShapeDiffExtra[Size] : MutantShapeDiffExtra[Size]); // Shape difficulty.

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel =>
		Size switch
		{
			2 => DifficultyLevel.Hard,
			3 or 4 => DifficultyLevel.Fiendish,
			_ => DifficultyLevel.Nightmare
		};

	/// <inheritdoc/>
	public override Technique TechniqueCode => GetComplexFishTechniqueCodeFromName(InternalName);

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.ComplexFish;

	/// <inheritdoc/>
	public override Rarity Rarity =>
		(Size, ShapeModifier, FinModifier) switch
		{
			(Size: 3, _, _) => Rarity.Sometimes
		};

	/// <summary>
	/// Indicates the base hash code.
	/// </summary>
	private int BaseHashCode => Digit << 17 & 0xABC0DEF;

	/// <summary>
	/// Indicates the base set hash code.
	/// </summary>
	private int BaseSetHashCode => new RegionCollection(BaseRegions).GetHashCode();

	/// <summary>
	/// Indicates the cover set hash code.
	/// </summary>
	private int CoverSetHashCode => new RegionCollection(CoverRegions).GetHashCode();

	/// <summary>
	/// The internal name.
	/// </summary>
	private string InternalName
	{
		get
		{
			string? fin = FinModifier == FinModifiers.Normal ? null : $"{FinModifier} ";
			string? shape = ShapeModifier == ShapeModifiers.Basic ? null : $"{ShapeModifier}";
			return $"{fin}{shape}{FishNames[Size]}";
		}
	}

	/// <summary>
	/// Indicates the fin modifier.
	/// </summary>
	private FinModifiers FinModifier =>
		IsSashimi switch
		{
			true => FinModifiers.Sashimi,
			false => FinModifiers.Finned,
			_ => FinModifiers.Normal
		};

	/// <summary>
	/// The shape modifier.
	/// </summary>
	private ShapeModifiers ShapeModifier => IsFranken ? ShapeModifiers.Franken : ShapeModifiers.Mutant;

	/// <summary>
	/// Indicates the base regions.
	/// </summary>
	private int[] BaseRegions
	{
		get
		{
			int[] result = new int[PopCount((uint)BaseSetsMask)];
			int i = 0;
			foreach (int region in BaseSetsMask)
			{
				result[i++] = region;
			}

			return result;
		}
	}

	/// <summary>
	/// Indicates the cover regions.
	/// </summary>
	private int[] CoverRegions
	{
		get
		{
			int[] result = new int[PopCount((uint)CoverSetsMask)];
			int i = 0;
			foreach (int region in CoverSetsMask)
			{
				result[i++] = region;
			}

			return result;
		}
	}

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
		get => new RegionCollection(BaseRegions).ToString();
	}

	[FormatItem]
	private string CoverSetsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new RegionCollection(CoverRegions).ToString();
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
	public static bool Equals(ComplexFishStep left, ComplexFishStep right) =>
		left.Digit == right.Digit
		&& left.BaseSetsMask == right.BaseSetsMask
		&& left.CoverSetsMask == right.CoverSetsMask
		&& left.Exofins == right.Exofins
		&& left.Endofins == right.Endofins;
}
