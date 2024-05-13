namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Fish</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit">Indicates the digit used.</param>
/// <param name="baseSetsMask">Indicates the mask that contains the base sets.</param>
/// <param name="coverSetsMask">Indicates the mask that contains the cover sets.</param>
/// <param name="fins">Indicates the fins used.</param>
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
/// <param name="isSiamese">Indicates whether the pattern is a Siamese Fish.</param>
public abstract partial class FishStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] Digit digit,
	[PrimaryConstructorParameter] HouseMask baseSetsMask,
	[PrimaryConstructorParameter] HouseMask coverSetsMask,
	[PrimaryConstructorParameter] ref readonly CellMap fins,
	[PrimaryConstructorParameter] bool? isSashimi,
	[PrimaryConstructorParameter] bool isSiamese = false
) : Step(conclusions, views, options), ISizeTrait
{
	/// <inheritdoc/>
	/// <remarks>
	/// The name of the corresponding names are:
	/// <list type="table">
	/// <item><term>2</term><description>X-Wing</description></item>
	/// <item><term>3</term><description>Swordfish</description></item>
	/// <item><term>4</term><description>Jellyfish</description></item>
	/// <item><term>5</term><description>Squirmbag (or Starfish)</description></item>
	/// <item><term>6</term><description>Whale</description></item>
	/// <item><term>7</term><description>Leviathan</description></item>
	/// </list>
	/// Other fishes of sizes not appearing in above don't have well-known names.
	/// </remarks>
	public int Size => PopCount((uint)BaseSetsMask);

	/// <summary>
	/// The internal notation.
	/// </summary>
	private protected string InternalNotation => Pattern.ToString(Options.Converter);

	/// <summary>
	/// Creates a <see cref="Fish"/> instance via the current data.
	/// </summary>
	private Fish Pattern
		=> new(
			Digit,
			BaseSetsMask,
			CoverSetsMask,
			in this is NormalFishStep { Fins: var f }
				? ref f
				: ref this is ComplexFishStep { Exofins: var f2 } ? ref f2 : ref @ref.NullRef<CellMap>(),
			in this is NormalFishStep
				? ref CellMap.Empty
				: ref this is ComplexFishStep { Endofins: var f3 } ? ref f3 : ref @ref.NullRef<CellMap>()
		);


	/// <inheritdoc cref="Step.ToString(CultureInfo?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public new string ToString(CultureInfo? culture = null)
		=> Pattern.ToString(culture is null ? GlobalizedConverter.InvariantCultureConverter : GlobalizedConverter.GetConverter(culture));
}
